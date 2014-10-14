// SLC.cpp : This is the main module for the Status Logic Component
//


#include "stdafx.h"
#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <Windows.h>

#include "sullivan.h"

BOOL finished(_TCHAR *);
BOOL reset(_TCHAR *);


FILE *status_file = NULL;

int APIENTRY _tWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine, int nCmdShow)
{
	int argc;
	LPWSTR *argv;

	argv = CommandLineToArgvW(GetCommandLineW(), &argc);

	if (argc == 1)
	{
		// S_initStatus();
		S_parseStatus();
		S_generateDash();
	} else {
			// we used to do special processing for finished & reset here
			// now we handle those like all other commands
		S_addStatus(argc, argv);
		S_parseStatus();
		S_generateDash();
	}

	return 0;
}





