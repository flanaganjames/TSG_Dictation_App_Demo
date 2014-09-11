/**********************************************************

This module handles the validation of the EHR data.  
It does some table lookups for vital signs,
ensures that the necessary exams have been
completed.  
  If there are missing things, or danger signs,
we issue a warning, which is displayed on the 
dashboard.  It passes the warning upstream to
the EHR.
  The main validation routine will return true if a 
warning was raised, otherwise false.

**********************************************************/

#include "stdafx.h"
#include <string.h>

#include "sullivan.h"
#include "parser.h"


	/*
	 * This sends a message to the EHR window, telling
	 * it we've raised a warning
	 */
const static int RAISE_WARNING = WM_USER + 0x0;
const static int LOWER_WARNING = WM_USER + 0x1;
void SendWinMsg(int msg)
{
	int result;
	HWND hwnd;
	if ((hwnd = FindWindow(NULL, L"Electronic Health Record Narrative")) > 0)
	{
		result = SendMessage(hwnd, msg, 0, 0);
	}
}


int vitalParse(char *s)
{
	int n;
	if (s == NULL)  return 0;
	n = strcspn(s, "0123456890");
	return atoi(s+n);
}

float vitalParseF(char *s)
{
	int n;
	float f;
	if (s == NULL)  return 0;
	n = strcspn(s, "0123456890.");
	f = (float) atof(s+n);
	return f;
}


enum t_vital {
	v_low = 0, v_normal, v_high, v_vhigh, v_ignoreh
};
	// define the low end of each range
int R_pulse[]  = {30, 50, 100, 120, -1};
int R_resp[]   = {4, 8, -1, 25, 99};
int R_sbp[]    = {40, 85, -1, 155, 290};
int R_dbp[]    = {-1, 30, -1, 99, 150};
float R_temp[] = {32., 36., -1., 39., 43.};

/*
 * We examine the text of the vital signs we've received,
 *  extracting the numeric values, and then make sure
 *  none of the numeric values we have are outside 
 *  acceptable ranges.
 */
char *vitalSigns(void)
{
		// convert them
	VVS_p = vitalParse(VS_p);
	VVS_r = vitalParse(VS_r);
	VVS_sbp = vitalParse(VS_sbp);
	VVS_dbp = vitalParse(VS_dbp);
	VVS_t = vitalParseF(VS_t);
	if (VVS_t > 70.0) // assume F, convert to C
		VVS_t = (VVS_t - 32) * 5 / 9;

	char *warntext = NULL;
	static char foobar[200];
	

		// we're only going to write code for pulse
		// this should be wrapped into an external routine
	if (VVS_p >= R_pulse[v_low] && VVS_p < R_pulse[v_normal])
		warntext = "pulse low!";
	else if (VVS_p >= R_pulse[v_high] && VVS_p <= R_pulse[v_vhigh])
		warntext = "pulse high!";
	else if (VVS_p > R_pulse[v_vhigh])
		warntext = "pulse very high!";
	if (warntext != NULL)
	{
		_snprintf(foobar, 200, 
			"%s: vital signs are p %d, r %d, t %.1f, bp %d/%d",
			warntext, VVS_p, VVS_r, VVS_t, VVS_sbp, VVS_dbp);
		return foobar;
	} else
		return NULL;
}


	/*
	 * this is the controlling routine for validation
	 */
	/*
	   The flow in the full routine will be: 
	   check for warning,
	   if true, call S_generateWarn(), call SendMessage();
	   if false, unlink WARN_PATH
	 */
static char *warningmsg = NULL;
bool Validate(void)
{
	bool warning = false;
	char *badVS = NULL;
	char *missingExam = NULL;

		// we need to know what we've completed before
		// checking for warnings
	S_sortStatus();

		// clean up from last time, if needed
	if (warningmsg)
	{
		free(warningmsg);
		warningmsg = NULL;
	}

		// check the vital signs
	badVS = vitalSigns();
	warning = (warning) || (badVS != NULL);

		// are we missing required elements?
	int aa = _comp_req.size();
	int bb = _req_exam.size();
	int cc = _req_hpi.size();
	int dd = _assess.size();
	if (_comp_req.size() < (_req_exam.size() + _req_hpi.size() + _assess.size()))
		missingExam = "Incomplete required exam & HPI elements!";
	warning = (warning) || (missingExam != NULL);

		// if we have no warnings, it's the easy case
	if (!warning)
	{
		_unlink(WARN_PATH);
		SendWinMsg(LOWER_WARNING);
		return warning;
	}

		// we need to post warnings, so we have work to do
	size_t n = 3;
	if (badVS)
		n += strlen(badVS);
	if (missingExam)
		n += strlen(missingExam);
	warningmsg = (char *) malloc(n);
	memset(warningmsg, 0, n);
	if (badVS)
	{
		strncat(warningmsg, badVS, strlen(badVS));
		strncat(warningmsg, "  ", 2);
	}
	if (missingExam)
	{
		strncat(warningmsg, missingExam, strlen(missingExam));
	}

		// now post to the dashboard and tell EHR we have warnings
	S_generateWarn(warningmsg);
	SendWinMsg(RAISE_WARNING);
	return warning;
}
