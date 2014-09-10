/**********************************************************

This module handles the validation of the EHR data.  
It does some table lookups for vital signs,
ensures that the necessary exams have been
completed.  
  If there are missing things, or danger signs,
we issue a warning, which is displayed on the 
dashboard.  It passes the warning upstream to
the EHR.
  We return true if a warning was raised, otherwise false.

**********************************************************/

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>

#include "sullivan.h"
#include "parser.h"

	/*
	 * this is the controlling routine for validation
	 */
bool S_validate(void)
{
	/*
	   The flow in the full routine will be: 
	   check for warning,
	   if true, call S_generateWarn(),
	   if false, unlink WARN_PATH
	 */
	// for test purposes at the moment we always throw a warning
	S_generateWarn("test warning");
	return true;
}
