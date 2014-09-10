// SLC.cpp : This is the main module for the Status Logic Component
//


#include "stdafx.h"
#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"

BOOL finished(_TCHAR *);
BOOL reset(_TCHAR *);


FILE *status_file = NULL;
FILE *dashboard_file = NULL;

// int __stdcall WinMain(HINSTANCE hinstance, HINSTANCE hprevinstance, LPSTR cmdline, int nCmdShow)
int _tmain(int argc, _TCHAR* argv[])
{
	if (argc == 1)
	{
		// S_initStatus();
		S_parseStatus();
		S_generateDash();
	} else {
		if (argc == 2 && finished(argv[1]))
		{
			S_finishStatus();
		} else if (argc == 2 && reset(argv[1]))
		{
			S_reset();
		} else {
			S_multiStatus(argc, argv);
			// temporarily removed these to make sure we've got a selection
			// in the status file -- we'll only parse when we have no args
			S_parseStatus();
			S_generateDash();
		}
	}

	return 0;
}


#define FINISHED L"end_of_script"
#define FINISHED_S L"end"
#define RESET L"reset"

BOOL finished(_TCHAR *s)
{
	return (_tcscmp(s, FINISHED) == 0
		|| _tcscmp(s, FINISHED_S) == 0);
}

BOOL reset(_TCHAR *s)
{
	return (_tcscmp(s, RESET) == 0);
}


