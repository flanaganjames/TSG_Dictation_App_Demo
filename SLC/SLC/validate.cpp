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
#include <sys/types.h>

#include "sullivan.h"
#include "parser.h"


	/*
	 * This sends a message to the EHR window, telling
	 * it we've raised a warning
	 */
const static int RAISE_WARNING = WM_USER + 0x0;
const static int LOWER_WARNING = WM_USER + 0x1;
void SendWarning(int msg)
{
	int result;
	HWND hwnd;
	if ((hwnd = FindWindow(NULL, L"Electronic Health Record Narrative")) > 0)
	{
		result = SendMessage(hwnd, msg, 0, 0);
	}
}


	/*
	 * this is the controlling routine for validation
	 */
bool S_validate(void)
{
	/*
	   The flow in the full routine will be: 
	   check for warning,
	   if true, call S_generateWarn(), call SendMessage();
	   if false, unlink WARN_PATH
	 */
	// for test purposes at the moment we always throw a warning
	S_generateWarn("test warning test warning test warning, test warning!!!");
	SendWarning(RAISE_WARNING);
	return true;
}
