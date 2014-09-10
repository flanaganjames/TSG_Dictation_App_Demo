/* 
	This is the code for the SLC parser
 */

#include "stdafx.h"

#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"


// structure for data we need
struct exam {
	char *complaint;
	struct element *e;
};

struct element {
	int type;
	char *title;
	struct element *next;
};

struct exam *current;


void S_parseStatus(void)
{
	char line[MAX_PATH];

	printf("entering parser...........\n"); //???
	//???? --- this clause is here to confirm we have the file & check its time
	struct stat sb;
	if (stat(STATUS_PATH, &sb) < 0)
		printf("unable to stat!!\n");
	else
		printf("mode 0%03o; times %ld %ld %ld\n", sb.st_mode, 
			sb.st_ctime, sb.st_mtime, sb.st_atime);
	
	if ((current = (struct exam *) malloc(sizeof(struct exam))) == NULL)
		exit(-1);
		
	if ((status_file = fopen(STATUS_PATH, "r")) == NULL)
	{
		current->complaint = "error";
		return;
	}

	while (fgets(line, MAX_PATH, status_file) != NULL)
	{
		printf(line);
	}

	fclose(status_file);
}


#include <time.h>

void S_generateDash(void)
{
	// test code to tickle the dashboard
	// ... this will get replaced with actual status
	FILE *dash = fopen(DASHBOARD_PATH, "w");
	fprintf(dash, "%s\n%ld\n", "test test test", time(0));
	fclose(dash);
}