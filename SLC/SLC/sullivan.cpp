#define _CRT_SECURE_NO_WARNINGS
#include "stdafx.h"

#include <Windows.h>
#include <stdio.h>
#include <stdlib.h>

#include "sullivan.h"



/*
	Initializes the status file to empty.
	In parallel, the DMPE scripts will trigger the Dashboard to open
	and start watching for updates.
	If we are called with keepOpen true, we want to leave the file open
	on completion since we want to continue writing to it.
 */
int S_initStatus(void)
{
	if (S_openStatus("w") < 0)
		return -1;
	return 0;
}

int S_openStatus(char *mode)
{
	if (status_file != NULL)
		fclose(status_file);
	if ((status_file = fopen(STATUS_PATH, mode)) == NULL)
	{
		fprintf(stderr, "SLC: can't open status file <%s> for append\n", 
			STATUS_PATH);
		status_file = NULL;
		return -1;
	}
	return 0;
}


/*
	Add data to the status file from the command line.
	We can be called with a single set of arguments, e.g.,
		complaint Chest Pain Over 40
	or we can be called with stacked arguments separated by "!", e.g.,
		complaint Chest Pain Over 40 ! req HPI Location, Movement
	Each set of arguments needs to be written to the status file on
	a separate line.  That file keeps our state between invocations of
	the SLC.  Commands "complaint" and "state" are special cases which
	reset our status and start a new medical record.
 */
int S_addStatus(int argc, _TCHAR **argv)
{
	int i;
	bool first_word = true;  // true at beginning of line and after a "!"
	_TCHAR *s;

	for (i = 1;  i < argc;  i++)
	{
		s = argv[i];
			// "state" and "complaint" are special cases
		if ( first_word && 
				(_wcsicmp(argv[i], L"complaint") == 0
					|| (_wcsicmp(argv[i], L"state") == 0  && no_complaint())) )
		{
			// reset the status file if we have an initial complaint
			if (S_initStatus() < 0)
				return -1;
		}

		if (status_file == NULL)
			S_openStatus("a");

		if (wcscmp(argv[i], L"!") == 0)
		{
			fprintf(status_file, "\n");
			first_word = true;
		} else {
			fprintf(status_file, "%s%S", 
				first_word ? "" : " ", argv[i]);
			first_word = false;
		}
	}
	fprintf(status_file, "\n");

	fclose(status_file);
	status_file = NULL;
	return 0;
}


/*
	We are done: finish the status file, and close the Dashboard.
 */
int S_finishStatus(void)
{
	_unlink(STATUS_PATH);
	_unlink(DASHBOARD_PATH);
	return 0;
}


