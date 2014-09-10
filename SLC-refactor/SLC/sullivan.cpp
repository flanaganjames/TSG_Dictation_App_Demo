#define _CRT_SECURE_NO_WARNINGS
#include "stdafx.h"

#include <Windows.h>
#include <stdio.h>
#include <stdlib.h>

#include "sullivan.h"



/*
	Initializes the status file.
	In parallel, the DMPE scripts will trigger the Dashboard to open
	and start watching for updates.
 */
int S_initStatus(void)
{
	if ((status_file = fopen(STATUS_PATH, "w")) == NULL )
	{
		fprintf(stderr, "SLC: can't create status file <%s>\n", STATUS_PATH);
		return 1;
	}
	fclose(status_file);

	return 0;
}


/*
	Adds to the status file.
	This will update the information for the dashboard into a separate
	file.  When Dashboard.exe sees that this file has been updated, it
	will update the dashboard window.
	This means that knowledge about what information appears in the 
	dashboard has to be encapsulated under this routine.
	......
	This is now called by a wrapper with a pointer *into* the argv
	array.  However, we still assume the chunk we're seeing has 0-based
	indexing, so we need to call it with **argv pointing to the entry
	*before* the one we want to record.
 */
int S_addStatus(int argc, _TCHAR** argv)
{
	int i;
	
	if (_wcsicmp(argv[1], L"complaint") == 0
		|| (_wcsicmp(argv[1], L"state") == 0  &&  complaint == NULL))
	{
		// reset the status file if we have an initial complaint
		S_initStatus();
	}
	
	if ((status_file = fopen(STATUS_PATH, "a")) == NULL)
	{
		fprintf(stderr, "SLC: can't open status file <%s> for append\n", STATUS_PATH);
		return -1;
	}

	for (i = 1; i < argc; i++)
	{
		fprintf(status_file, "%S%c", argv[i], ((i+1) == argc) ? '\n' : ' ');
	}
	fclose(status_file);
	return 0;
}


/*
	This is the wrapper for a long, multi-entry input line
	with the entries separated by "!"
 */
int S_multiStatus(int argc, _TCHAR** argv)
{
	int i, f, l;

	for( f = i = 1;  i < argc;  i++)
	{
		if (wcscmp(argv[i], L"!") == 0)
		{
			l = i;
			S_addStatus((l-f+1), &argv[f-1]);
			f = i+1;
		}
	}
	S_addStatus((argc-f+1), &argv[f-1]);
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


