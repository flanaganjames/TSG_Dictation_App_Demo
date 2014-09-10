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
char *req_hpi = NULL;
// char *req_hpi_comp = NULL; // completed items collected in req_exam_comp for the moment
char *req_exam = NULL;
char *req_exam_comp = NULL;
char *links = NULL;

// data as arrays of words
char **list_req_hpi;
char **list_req_exam;
char **complete;
char **comp_hpi;
char **comp_exam;

enum commands_t {state_t = 0, req_hpi_t, req_exam_t, data_t, link_t, unknown_t};
char *commands_names[] = { "state", "req hpi", "req exam", "data", "link" };
const int command_count = (sizeof(commands_names)/sizeof(commands_names[0]));


void clobberState(void)
{
	free(complaint);
	complaint = NULL;
	free(req_hpi);
	req_hpi = NULL;
	// free(req_hpi_comp);
	// req_hpi_comp = NULL;
	free(req_exam);
	req_exam = NULL;
	free(req_exam_comp);
	req_exam_comp = NULL;
	free(links);
	links = NULL;
}

	// case folding version of strncat
	// which also elides commas
	// (and, yes, it would be more efficient to do the character
	// folding and normalizing as we took the information off
	// the command line)
char *catstr(char *in, char *add, size_t n)
{
	char *s;
	s = in + strlen(in);
	if (strlen(in) > 0)
		*s++ = ' ';
	while (*add)
	{
		if (*add == ',')
			add++;
		else
			*s++ = tolower(*add++);
	}
	*s = '\0';
	return in;
}

	// append the given string s to the input string, returning
	// the reallocated full string
char *addString(char *in, char *s)
{
	if (in == NULL)
	{
		in = (char *) malloc(strlen(s)+1);
		*in = '\0';
		catstr(in, s, strlen(s)+1);
	} else {
		in = (char *) realloc(in, strlen(in)+strlen(s)+2);
		catstr(in, s, strlen(s)+1);
	}
	return in;
}

void chomp(char *s)
{
	char *t;
	t = strpbrk(s, "\r\n");
	if (t)  *t = '\0';
}


void S_parseStatus(void)
{
	char line[MAX_PATH], *s;

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
		int i;
			
		chomp(s);  // strip the EOL characters
		
		printf("line: %s\n", line); //???
			// recognize the command part of the line
		for (i = 0;  i < command_count; i++ )
		{
			if (_strnicmp(s, commands_names[i], strlen(commands_names[i])) == 0)
			{
				s += strlen(commands_names[i]); // skip the command
				s += strspn(s, " ");  // skip the blanks
				break;
			}
		}
		if (s == line) // we didn't recognize the command
		{
			printf("unknown: %s\n", line); //???
			continue;
		}

			// log the command and the arguments
		switch (i) {
		case state_t:
			// if it's a new state, clobber the old one
			// and start a new parsed tree
			clobberState();
			complaint = addString(complaint, s);
			break;
		case req_hpi_t:
			req_hpi = addString(req_hpi, s);
			break;
		case req_exam_t:
			req_exam = addString(req_exam, s);
			break;
		case data_t:
			req_exam_comp = addString(req_exam_comp, s);
			break;
		case link_t:
			links = addString(links, s);
			break;
		}
	}

	fclose(status_file);
}




void S_sortStatus(void)
{
	list_req = malloc(sizeof(char *) * wordcount(
}



/*
	This generates the dashboard contents from the data we parsed out 
	of the status file
 */
#include <time.h>

void S_generateDash(void)
{
	S_sortStatus();

	// open the dashboard
	FILE *dash = fopen(DASHBOARD_PATH, "w");
	fprintf(dash, "Recommended RSQ Documentation\n\n");
	
	// complaint
	fprintf(dash, "Presenting complaint: %s\n", complaint);

	fprintf(dash, "Still required: ");


	fprintf(dash, "  Completed: ");


	fprintf(dash, "Recommended: ");


	fprintf(dash, "  Completed: ");


	fprintf(dash, "%d%% complete\n", frac);


	// show the stuff we parsed //???
	printf("\n\ncomplaint was: %s\n", complaint);
	printf("HPI we need: %s\n", req_hpi);
	printf("exams we need: %s\n", req_exam);
	printf("exams we've completed: %s\n", req_exam_comp);
	printf("references: %s\n", links);
	
	// test code to tickle the dashboard
	// ... this will get replaced with actual status
	fprintf(dash, "\n\n%s\n%ld\n", "test test test", time(0));
	fclose(dash);
}
