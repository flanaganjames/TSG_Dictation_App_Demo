/* 
	This is the code for the SLC parser
 */

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"
#include "parser.h"

// data we need to collect
char *complaint = NULL;
char *differential = NULL;

#if 0
	NOTES:  the following lists of words need to really
	be handled by a class for word lists
		A sketch of the data would be:	
// data as arrays of words
struct word_array {
	char **list;
	int count;
};
#endif

char **list_req_hpi;	int n_req_hpi = 0;
char **list_req_exam;	int n_req_exam = 0;
char **list_assess;		int n_assess = 0;
char **list_rec_hpi;	int n_rec_hpi = 0;
char **list_rec_exam;	int n_rec_exam = 0;
char **complete;		int n_complete = 0;
char **comp_req;		int n_comp_req = 0;
char **comp_req_hpi;	int n_c_req_hpi = 0;
char **comp_req_exam;	int n_c_req_exam = 0;
char **comp_assess;		int n_c_assess = 0;
char **comp_rec_hpi;	int n_c_rec_hpi = 0;
char **comp_rec_exam;	int n_c_rec_exam = 0;
char **links;			int n_links = 0;

enum commands_t {complaint_t = 0, state_t, diff_t, add_t,
	req_hpi_t, req_exam_t, assess_t,
	rec_hpi_t, rec_exam_t, recc_hpi_t, recc_exam_t,
	data_hpi_t, data_exam_t, // unused - should be removed
	data_t, link_t, delete_t, del_t,
	end_t, end_tt, reset_t,
	unknown_t};
char *commands_names[] = { "complaint", "state", "diff", "add",
	"req hpi", "req exam", "assess",
	"rec hpi", "rec exam", "recc hpi", "recc exam",
	"data hpi", "data exam", // unused - should be removed
	"data", "link", "delete", "del",
	"end", "end_of_script",	"reset"
};
const int command_count = (sizeof(commands_names)/sizeof(commands_names[0]));


char **freeList(char **list, int *count)
{
	int i;
	for( i = 0;  i < *count; i++)
	{
		free(list[i]);
	}
	free(list);
	*count = 0;
	return list;
}


void clobberState(void)
{
	free(complaint);
	complaint = NULL;
	free(differential);
	differential = NULL;
	list_req_hpi	= freeList(list_req_hpi, &n_req_hpi);
	list_req_exam	= freeList(list_req_exam, &n_req_exam);
	list_assess		= freeList(list_assess, &n_assess);
	list_rec_hpi	= freeList(list_rec_hpi, &n_rec_hpi);
	list_rec_exam	= freeList(list_rec_exam, &n_rec_exam);
	complete		= freeList(complete, &n_complete);
	comp_req		= freeList(comp_req, &n_comp_req);
	comp_req_hpi	= freeList(comp_req_hpi, &n_c_req_hpi);
	comp_req_exam	= freeList(comp_req_exam, &n_c_req_exam);
	comp_assess		= freeList(comp_assess, &n_c_assess);
	comp_rec_hpi	= freeList(comp_rec_hpi, &n_c_rec_hpi);
	comp_rec_exam	= freeList(comp_rec_exam, &n_c_rec_exam);
	links			= freeList(links, &n_links);
}



void S_reset(void)
{
	S_initStatus();
	clobberState();
}

	
	// take an array of words and a string
	// add the words from the string to the end of
	// the array, splitting at commas
	// don't add duplicates, 
	// return the final size of the list
int addwords(char **in, int n, char *add)
{
	char *s, *t;

		// we need to strip the decorations
		// do this in a quick-and-dirty way by copying
		// the input string
	char *ss = (char *) malloc(strlen(add)+1);
	s = ss;
	for (char *p = add;  p && *p;  p++)
	{
		if (*p != '['  &&  *p != ']'  &&  *p != '*')
			*s++ = *p;
	}
	*s = 0;

#if 0
	// casefold the input string
	for (s = add;  *s;  s++)
	{
	 *s = tolower(*s);
	}
#endif

	// now add to the array
	s = ss;
	while (s && *s)
	{
		int l; 
			// peel off the word
		if (t = strchr(s, ','))
			l = t - s;
		else
			l = strlen(s);
		
			// make sure we aren't adding a duplicate
		BOOL match = FALSE;
		for (int i = 0;  i < n;  i++)
		{
			if (strcmp(s, in[i]) == 0)
			{
				match = TRUE;
				break;
			}
		}
		
			// now add it if it's a non-duplicate
		if (!match)
		{
			in[n] = (char *) malloc(l+1);
			strncpy(in[n], s, l);
			*(in[n]+l) = '\0';
			n++;
		}
		s += l;
			// skip blanks & commas to the next word beginning
		while (s && *s && (*s == ' ' || *s == ',')) s++;

	}
	return n;
}


	// how many comma-separated words in this string
int wordcount(char *s)
{
	int n = 0;
	while (*s) 
	{
		n++;
		if ((s = strchr(s, ',')) == NULL) // done
			break;
		s++; // skip this comma
		// don't need to explicitly skip blanks if comma
		// is the only separator
		// while (*s && *s == ' ') s++;
	}
	return n;
}

	// append the given string s to the input string, returning
	// the reallocated full string
char **addString(char **in, int *n, char *s)
{
	int m = wordcount(s);
	int nn = *n;

	if (in == NULL)
	{
		in = (char **) malloc(sizeof(char **) * m);
		nn = addwords(in, 0, s);
	} else {
		in = (char **) realloc(in, sizeof(char **) * (nn+m));
		nn = addwords(in, nn, s);
	}
	*n = nn;
	return in;
}

	// strip off the line-ending characters & trailing blanks
void chomp(char *s)
{
	char *t;
	t = strpbrk(s, "\r\n");
	if (t)  *t-- = '\0';
	while (t && *t == ' ')  *t-- = '\0';
}


void S_parseStatus(void)
{
	char line[MAX_PATH], *s;

#ifdef TESTING
	printf("entering parser...........\n");

	struct stat sb;
	if (stat(STATUS_PATH, &sb) < 0)
		printf("unable to stat!!\n");
	else
		printf("mode 0%03o; times %ld %ld %ld\n", sb.st_mode, 
			sb.st_ctime, sb.st_mtime, sb.st_atime);
#endif
	
	if ((status_file = fopen(STATUS_PATH, "r")) == NULL)
	{
		return;
	}

	while ((s = fgets(line, MAX_PATH, status_file)) != NULL)
	{
		int i;
			
		chomp(s);  // strip the EOL characters
		
#ifdef TESTING
		printf("line: %s\n", line);
#endif
			// recognize the command part of the line
		for (i = 0;  i < command_count; i++ )
		{
			if (_strnicmp(s, commands_names[i], strlen(commands_names[i])) == 0)
			{
				s += strlen(commands_names[i]); // skip the command
				s += strspn(s, " ");  // skip the blanks after command
				break;
			}
		}
		if (s == line) // we didn't recognize the command: assume unknown
		{	
#ifdef TESTING
			printf("unknown: %s\n", line);
#endif
			i = unknown_t;
		}

			// log the command and the arguments
		switch (i) {
		case state_t:
			// it's a new state:  assume a new complaint, too,
			// but don't update the saved complaint name if
			// we already have one
			if (complaint != NULL)  break;
			clobberState();
			complaint = (char *) malloc(strlen(s)+1);
			strcpy(complaint, s);
			break;			
		case complaint_t:
			// if it's a new complaint, clobber the old one
			// and start a new parsed tree
			clobberState();
			complaint = (char *) malloc(strlen(s)+1);
			strcpy(complaint, s);
			break;
		case diff_t:
			if (differential)
				free(differential);
			differential = (char *) malloc(strlen(s)+1);
			strcpy(differential, s);
			break;
		case add_t:
			// this keyword comes from the MU, not the VB script,
			// so ignore for the moment
			// list_req_hpi = addString(list_req_hpi, &n_req_hpi, s);
			break;
		case req_hpi_t:
			list_req_hpi = addString(list_req_hpi, &n_req_hpi, s);
			break;
		case req_exam_t:
			list_req_exam = addString(list_req_exam, &n_req_exam, s);
			break;
		case assess_t:
			list_assess = addString(list_assess, &n_assess, s);
			break;
		case rec_hpi_t:
		case recc_hpi_t:
			list_rec_hpi = addString(list_rec_hpi, &n_rec_hpi, s);
			break;
		case rec_exam_t:
		case recc_exam_t:
			list_rec_exam = addString(list_rec_exam, &n_rec_exam, s);
			break;
		case data_hpi_t:
		case data_exam_t:
		case data_t:
			complete = addString(complete, &n_complete, s);
			break;
		case link_t:
			links = addString(links, &n_links, s);
			break;
		case delete_t:
		case del_t:
			// currently informational and ignored
			break;
		case end_t:
		case end_tt:
			S_finishStatus();
			break;
		case reset_t:
			S_reset();
			break;
		default:
			break;
		}
	}

	fclose(status_file);
}



#ifdef TESTING
void printlist(char *title, char **s, int n)
{
	printf("... %s: ", title);
	for (int i = 0;  i < n;  i++)
		if (strlen(s[i]))
			printf("%s%s", s[i], ((i+1)==n) ? "" : ", ");
		else
			printf(".. ");	
	printf("\n");
}
#endif





char *findword(char *word, char **list, int n)
{
	for (int i = 0;  i < n;  i++)
	{
		if (_stricmp(list[i], word) == 0)
			return list[i];
	}
	return NULL;
}


void S_sortStatus(void)
{
	int i;
	char *s;
	
#ifdef TESTING
	// debug print of the lists before we sort
	printlist("list_req_hpi", list_req_hpi, n_req_hpi);
	printlist("list_req_exam", list_req_exam, n_req_exam);
#endif

		// run through the list of complete exams & hpis, and split them
		// up into the completed list corresponding to the required &
		// recommended lists
		// (don't forget the list of assessments)
		// also, for the required items, populate a combined list, comp_req,
		// in the order we got completion notification
	for (i = 0; i < n_complete; i++)
	{
		if ((s = findword(complete[i], list_req_hpi, n_req_hpi)) != NULL)
		{
			comp_req_hpi = addString(comp_req_hpi, &n_c_req_hpi, complete[i]);
			comp_req = addString(comp_req, &n_comp_req, complete[i]);
			*s = '\0';
		}
		if ((s = findword(complete[i], list_req_exam, n_req_exam)) != NULL)
		{
			comp_req_exam = addString(comp_req_exam, &n_c_req_exam, complete[i]);
			comp_req = addString(comp_req, &n_comp_req, complete[i]);
			*s = '\0';
		}
		if ((s = findword(complete[i], list_assess, n_assess)) != NULL)
		{
			comp_assess = addString(comp_assess, &n_assess, complete[i]);
			comp_req = addString(comp_req, &n_comp_req, complete[i]);
			*s = '\0';
		}
		if ((s = findword(complete[i], list_rec_hpi, n_rec_hpi)) != NULL)
		{
			comp_rec_hpi = addString(comp_rec_hpi, &n_c_rec_hpi, complete[i]);
			*s = '\0';
		}
		if ((s = findword(complete[i], list_rec_exam, n_rec_exam)) != NULL)
		{
			comp_rec_exam = addString(comp_rec_exam, &n_c_rec_exam, complete[i]);
			*s = '\0';
		}

	}
	
#ifdef TESTING
	// debug print of the lists after we've sorted them
	printlist("list_req_hpi", list_req_hpi, n_req_hpi);
	printlist("list_req_exam", list_req_exam, n_req_exam);
	printlist("list_assess", list_assess, n_assess);
	printlist("rec_hpi", list_rec_hpi, n_rec_hpi);
	printlist("rec_exam", list_rec_exam, n_rec_exam);
	printlist("full complete", complete, n_complete);
	printlist("comp_req_hpi", comp_req_hpi, n_c_req_hpi);
	printlist("comp_req_exam", comp_req_exam, n_c_req_exam);
	printlist("comp_assess", comp_assess, n_c_assess);
	printlist("comp_rec_hpi", comp_rec_hpi, n_c_rec_hpi);
	printlist("comp_rec_exam", comp_rec_exam, n_c_rec_exam);
	printlist("links", links, n_links);
#endif
}






