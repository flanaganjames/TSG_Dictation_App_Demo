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


#if 0
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
#endif


	/*
	 * parse a numeric vital sign value out of the given string
	 */
int vitalParse(char *s)
{
	int n;
	if (s == NULL)  return 0;
	n = strcspn(s, "0123456890");
	return atoi(s+n);
}

	/*
	 * special case parsing for temperature:
	 *  as above, but we convert to C, if we've got F
	 *  and return as a float, not an int
	 */
float vitalParseT(char *s)
{
	int n;
	float f;
	if (s == NULL)  return 0;
	n = strcspn(s, "0123456890.");
	f = (float) atof(s+n);
	if (f > 70.0) // assume F, convert to C
	{
		f = (f - 32) * 5 / 9;
	}
	return f;
}


struct _range {
	// open range: x >= low && x < high
		// ignore a limit if < 0;
	int low, high;
	char *warning;
};
struct _rangef {
		// we really should use a template class here
	float low, high;
	char *warning;
};
typedef struct _range range;
typedef struct _rangef rangeF;

	/*
	 * here we have the ranges that generate warnings and 
	 *  the associated warning text
	 */
range RR_pulse[] = { 
	{30, 50, "Pulse low"},
	{100, 120, "Pulse high"},
	{120, -1, "Pulse very high"},
};
const int n_pulse = (sizeof(RR_pulse)/sizeof(range));

range RR_resp[] = {
	{4, 8, "Respiration low"},
	{25, 99, "Respiration very high"},
};
const int n_resp = (sizeof(RR_resp)/sizeof(range));

range RR_sbp[] = {
	{40, 85, "Systolic BP low"},
	{155, 290, "Systolic BP very high"},
};
const int n_sbp = (sizeof(RR_sbp)/sizeof(range));

range RR_dbp[] = {
	{99, 150, "Diastolic BP very high"},
};
const int n_dbp = (sizeof(RR_dbp)/sizeof(range));

rangeF RR_temp[] = {
	{32, 36, "Temperature low"},
	{39, 43, "Temperature very high"},
};
const int n_temp = (sizeof(RR_temp)/sizeof(rangeF));

#if 0
enum t_vital {
	v_low = 0, v_normal, v_high, v_vhigh, v_ignoreh
};
	// define the low end of each range
int R_pulse[]  = {30, 50, 100, 120, -1};
int R_resp[]   = {4, 8, -1, 25, 99};
int R_sbp[]    = {40, 85, -1, 155, 290};
int R_dbp[]    = {-1, 30, -1, 99, 150};
float R_temp[] = {32., 36., -1., 39., 43.};
#endif

void validateVital(int vital, range Range[], int nn)
{
	if (vital == 0)  return;
	for (int i = 0;  i < nn;  i++)
	{
		if ((Range[i].low < 0  &&  vital < Range[i].high)
			|| (vital >= Range[i].low  &&  vital < Range[i].high)
			|| (vital >= Range[i].low  &&  Range[i].high < 0))
		{
				D_addWarning(Range[i].warning);
				break;
		}
	}
};

void validateVitalF(float vital, rangeF Range[], int nn)
{
	if (vital == 0.0)  return;
	for (int i = 0;  i < nn;  i++)
	{
		if ((Range[i].low < 0.  &&  vital < Range[i].high)
			|| (vital >= Range[i].low  &&  vital < Range[i].high)
			|| (vital >= Range[i].low  &&  Range[i].high < 0.))
		{
				D_addWarning(Range[i].warning);
				break;
		}
	}
};

/*
 * We examine the text of the vital signs we've received,
 *  extracting the numeric values, and then make sure
 *  none of the numeric values we have are outside 
 *  acceptable ranges.
 */
 void vitalSigns(void)
{
		// convert them
	_VVS_p = vitalParse(_VS_p);
	_VVS_r = vitalParse(_VS_r);
	_VVS_sbp = vitalParse(_VS_sbp);
	_VVS_dbp = vitalParse(_VS_dbp);
	_VVS_t = vitalParseT(_VS_t);

		// validate the ranges
	validateVital(_VVS_p, RR_pulse, n_pulse);
	validateVital(_VVS_r, RR_resp, n_resp);
	validateVital(_VVS_sbp, RR_sbp, n_sbp);
	validateVital(_VVS_dbp, RR_dbp, n_dbp);
	validateVitalF(_VVS_t, RR_temp, n_temp);

#if 0
		// Easter Egg for Sean
	if (_VVS_p == 115  &&  _VVS_r == 25)
		D_addWarning("Go Bears!!");
#endif

#if 0
	// add all the vital signs for debugging purposes
	char foobar[200];
	_snprintf(foobar, 200, "vital signs are p %d, r %d, t %.1f, bp %d/%d",
		_VVS_p, _VVS_r, _VVS_t, _VVS_sbp, _VVS_dbp);
	D_addWarning(foobar);
#endif
}

 void D_removeWarningBox(void)
 {
	_unlink(WARN_PATH);
 }

 
	/*
	 * this is the controlling routine for validation:
	 *  we validate the vital signs and exams, and then
	 *  go to the routine to generate the warning box
	 *  which only produces a warning box if it's needed
	 */
void S_Validate(void)
{
	bool warning = false;
	char *badVS = NULL;
	char *missingExam = NULL;

		// we need to know what we've completed before
		// checking for warnings
		// (not strictly necessary since we're now always
		//  calling S_Validate from S_generateDash() which
		//  has already done this; calling it a second
		//  time will have no effect)
	S_sortStatus();

		// clear previous warnings, if any
	D_clearWarnings();

		// special case for TAD Risk
	list<char *>::iterator i;
	for (i = _req_exam.begin();  i != _req_exam.end();  i++)
	{
		if (_strnicmp(*i, "TAD risk impression", strlen(*i)) == 0)
		{
			D_addWarning("Check TAD Risk!");
			break;
		}
	}

		// check the vital signs
	vitalSigns();

	/**********
	*********  no longer checking completion of all exam elements
		// are we missing required elements?
	if (_comp_req.size() < (_req_exam.size() + _req_hpi.size() + _assess.size()))
	{
		D_addWarning("Incomplete required exam & HPI elements!");
		warning = true;
	}
	***********/

	S_generateWarnBox();
#if 0
	if (!warning)
	{
		_unlink(WARN_PATH);
		SendWinMsg(LOWER_WARNING);
	} else {
		S_generateWarnBox();
		SendWinMsg(RAISE_WARNING);
	}
	return warning;
#endif
}
