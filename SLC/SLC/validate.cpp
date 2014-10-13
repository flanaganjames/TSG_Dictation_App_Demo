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
	 * special case parsing for temperature as a float
	 * ... works for both C & F
	 */
float vitalParseT(char *s)
{
	int n;
	float f;
	if (s == NULL)  return 0;
	n = strcspn(s, "0123456890.");
	f = (float) atof(s+n);
		// doesn't matter whether we're C or F:
		// the table for normals has both scales
	/******
	if (f > 70.0) // assume F, convert to C
	{
		f = (f - 32) * 5 / 9;
	}
	******/
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
	{32.F, 36.F, "Temperature low"},	// C values
	{39.F, 43.F, "Temperature very high"},
	{89.6F, 96.F, "Temperature low"},	// F values
	{102.2F, 109.4F, "Temperature very high"},
};
const int n_temp = (sizeof(RR_temp)/sizeof(rangeF));

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
 void S_checkVitalSigns(void)
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
	if (_VVS_p == 107)
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
	char *consider = "Consider TAD Risk";
	bool found_TAD = false;
	for (i = _req_hpi.begin();  i != _req_hpi.end();  i++)
	{
		if (_strnicmp(*i, consider, strlen(*i)) == 0)
		{
			found_TAD = true;
			break;
		}
	}
	for (i = _req_exam.begin();  i != _req_exam.end();  i++)
	{
		if (_strnicmp(*i, consider, strlen(*i)) == 0)
		{
			found_TAD = true;
			break;
		}
	}
	for (i = _assess.begin();  i != _assess.end();  i++)
	{
		if (_strnicmp(*i, consider, strlen(*i)) == 0)
		{
			found_TAD = true;
			break;
		}
	}
	if (found_TAD)
	{
		D_addWarning("Check TAD Risk!");
	}

		// check the vital signs
	S_checkVitalSigns();

		// now yell about problems
	S_generateWarnBox();
}
