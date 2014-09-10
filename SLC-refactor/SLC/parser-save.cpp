/* 
	This is the code for the SLC parser
 */

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"


// data we need to collect
char *complaint = NULL;
// char *req_hpi = NULL;
// char *req_hpi_comp = NULL;
char *req_exam = NULL;
char *req_exam_comp = NULL;
char *links = NULL;

enum commands_t {state_type = 0, req_hpi, req_exam, data, link, unknown};
char *commands_names[] = { "state", "req hpi", "req exam", "data", "link" };
const int command_count = (sizeof(commands_names)/sizeof(commands_names[0]));


void clobberState(void)
{
	free(complaint);
	// free(req_hpi);
	// free(req_hpi_comp);
	free(req_exam);
	free(req_exam_comp);
	free(links);
}

	// append the given string s to the input string, returning
	// the reallocated full string
char *addString(char *in, char *s)
{
	if (in == NULL)
	{
		in = (char *) malloc(strlen(s)+1);
		strncpy(in, s, strlen(s);
	} else {
		in = (char *) realloc(in, strlen(in)+strlen(s)+2);
		strncat(in, " ", 1);
		strncat(in, s, strlen(s));
	}
	return in;
}


void S_parseStatus(void)
{
	char line[MAX_PATH], *s, *t;
	struct element *ee;

	printf("entering parser...........\n"); //???
	//???? --- this clause is here to confirm we have the file & check its time
	struct stat sb;
	if (stat(STATUS_PATH, &sb) < 0)
		printf("unable to stat!!\n");
	else
		printf("mode 0%03o; times %ld %ld %ld\n", sb.st_mode, 
			sb.st_ctime, sb.st_mtime, sb.st_atime);
	
	if ((status_file = fopen(STATUS_PATH, "r")) == NULL)
	{
		return;
	}

	while ((s = fgets(line, MAX_PATH, status_file)) != NULL)
	{
		int n;
		printf(line); //???
			// find the command part of the line
		for (i = 0;  i < command_count; i++ )
		{
			if (strnicmp(s, commands_names[i], strlen(commands_names[i])) == 0)
			{
				s += strlen(commands_names[i]); // skip the command
				s += strspn(s, " ");  // skip the blanks
				break;
			}
		}
		if (s == line) // we didn't recognize the command
		{
			break;
		}

			// if it's a new state, clobber the old one
			// and start a new parsed tree
		if (i == state_type)
		{
			clobberState();
			complaint = addString(s);
			break;
		}

			// for anything else, we log the command and
			// the arguments
		switch (i) {
		case req_hpi:
		case req_exam:
			req_exam = addString(s);
			break;
		case data:
			req_exam_comp = addString(s);
			break;
		case link:
			links = addString(s);
			break;
		}
	}

	fclose(status_file);
}



/*
	This generates the dashboard contents from the data we parsed out 
	of the status file
 */
#include <time.h>

void S_generateDash(void)
{
	// show the stuff we parsed //???
	printf("complaint was: %s\n", complaint);
	printf("exams we need: %s\n", req_hpi);
	printf("exams we've completed: %s\n", req_hpi_comp);
	printf("references: %s\n", links);
	
	// test code to tickle the dashboard
	// ... this will get replaced with actual status
	FILE *dash = fopen(DASHBOARD_PATH, "w");
	fprintf(dash, "%s\n%ld\n", "test test test", time(0));
	fclose(dash);
}



