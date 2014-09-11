/* 
	This is the code for the SLC parser
 */

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"
#include "parser.h"

// ********************************************************
// data we need to collect
list<char *> _complaint;
	// incomplete required items
list<char *> _req_hpi, _req_exam, _assess;
	// incomplete recommended items
list<char *> _rec_hpi, _rec_exam;
	// items completed
list<char *> _all_complete, _comp_req, _comp_rec;
	// resource links
list<char *> _links;
	// possible information on differential diagnosis
char *differential;
	/*
	 * we really need to have lists of a class containing
	 * a string, since we're making copies of strings all
	 * over the place, and leaking memory like a sieve;
	 * the constructors & destructors would handle memory
	 */


// ********************************************************
// constants for the parser
enum commands_t {complaint_t = 0, state_t, diff_t, add_t,
	req_hpi_t, req_exam_t, assess_t,
	rec_hpi_t, rec_exam_t, recc_hpi_t, recc_exam_t,
	data_hpi_t, data_exam_t, // unused - should be removed
	data_t, link_t, delete_t, del_t,
	end_t, end_tt, reset_t,
	validate_t,
	unknown_t};
char *commands_names[] = { "complaint", "state", "diff", "add",
	"req hpi", "req exam", "assess",
	"rec hpi", "rec exam", "recc hpi", "recc exam",
	"data hpi", "data exam", // unused - should be removed
	"data", "link", "delete", "del",
	"end", "end_of_script",	"reset",
	"validate"
};
const int command_count = (sizeof(commands_names)/sizeof(commands_names[0]));


// ********************************************************

	// delete all the existing data
void clobberState(void)
{
	_complaint.clear();
	_req_hpi.clear();
	_req_exam.clear();
	_assess.clear();
	_rec_hpi.clear();
	_rec_exam.clear();
	_all_complete.clear();
	_comp_req.clear();
	_comp_rec.clear();
	_links.clear();
	free(differential);
	differential = NULL;
		// don't bother to check if it exists before
		// deleting: the unlink failure is benign
	_unlink(WARN_PATH);
}


void S_reset(void)
{
	S_initStatus();
	clobberState();
	S_generateDash();
}


	// allocate a copy of the string
char *scopy(char *s)
{
	char *t = (char *) malloc(strlen(s)+1);
	strncpy(t, s, strlen(s)+1);
	return t;
}


	// is the given string in the list?
	// return location if so
static list<char *>::iterator i;
list<char *>::iterator findword(list<char *> &L, char *s)
{
	for (i = L.begin();  i != L.end();  i++)
	{
		if (_stricmp(s, *i) == 0)
			return i;
	}
	return L.end();
}


	// given list class instance and a string containing a list of words, 
	// split the string at commas, and add the words that aren't already
	// in the list to the list
void addWords(list<char *> &in, char *add)
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
			// peel off the next word
		if (t = strchr(s, ','))
		{
			l = t - s;
			*t = '\0';
		}
		else
			l = strlen(s);
		
			// add if it's not a duplicate
		if (findword(in, s) == in.end())
			in.push_back(scopy(s));
		
			// skip blanks & commas to the next word beginning
		s += l;
		if (t)  s = ++t;
		while (s && *s && (*s == ' ' || *s == ',')) s++;
	}
	free(ss);
}


	// strip off the line-ending characters & trailing blanks
void chomp(char *s)
{
	char *t;
	t = strpbrk(s, "\r\n");
	if (t)  *t-- = '\0';
	while (t && *t == ' ')  *t-- = '\0';
}

bool no_complaint(void)
{
	return _complaint.empty();
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
			// in alphabetical order by enum of the command
		switch (i) {
		case state_t:
			// it's a new state:  assume a new complaint, too,
			// but don't update the saved complaint name if
			// we already have one
			// ..... note that we have a list for complaints,
			//        but ignore all but the first
			if (! _complaint.empty())  break;
			clobberState();
			_complaint.push_back(scopy(s));
			break;			
		case complaint_t:
			// if it's a new complaint command, clobber the old one
			// and start a new parsed tree
			// ..... note that we ignore a second state,
			//       but each new complaint resets and overrides
			//       the existing one
			clobberState();
			_complaint.push_back(scopy(s));
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
			break;
		case req_hpi_t:
			addWords(_req_hpi, s);
			break;
		case req_exam_t:
			addWords(_req_exam, s);
			break;
		case assess_t:
			addWords(_assess, s);
			break;
		case rec_hpi_t:
		case recc_hpi_t:
			addWords(_rec_hpi, s);
			break;
		case rec_exam_t:
		case recc_exam_t:
			addWords(_rec_exam, s);
			break;
		case data_hpi_t:
		case data_exam_t:
		case data_t:
			addWords(_all_complete, s);
			break;
		case link_t:
			addWords(_links, s);
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
		case validate_t:
			S_validate();
			break;
		default:
			break;
		}
	}

	fclose(status_file);
}



#ifdef TESTING
void printlist(char *title, list<char *> L)
{
	printf("... %s: ", title);
	list<char *>::iterator ii;
	for (i = L.begin();  i != L.end();  i++)
		printf("%s, ", *i);	
	printf("\n");
}
#endif



void S_sortStatus(void)
{
	list<char *>::iterator i, ii;
	char *s;

		// run through the combined list of completed exams --
		//  this assumes we are keeping only a combined list
		//  from the input commands -- and splits them up into
		//  the sublists corresponding to the originally specified
		//  required and recommended lists, removing them from
		//  the incomplete exam lists
		// the completed sublists are created as stacks -- LIFO --
		//  so we can present the completed lists with the 
		//  most-recently completed item at the top
		// we don't bother to split the completed lists up by 
		//  hpi, exam, and assessment at this point
	for (i = _all_complete.begin();  i != _all_complete.end();  i++)
	{
		s = *i;
		if ((ii = findword(_req_hpi, s)) != _req_hpi.end())
		{
			_req_hpi.erase(ii);
			_comp_req.push_front(scopy(s));
		}
		if ((ii = findword(_req_exam, s)) != _req_exam.end())
		{
			_req_exam.erase(ii);
			_comp_req.push_front(scopy(s));
		}
		if ((ii = findword(_assess, s)) != _assess.end())
		{
			_assess.erase(ii);
			_comp_req.push_front(scopy(s));
		}
			// now the recommended ones...
		if ((ii = findword(_rec_hpi, s)) != _rec_hpi.end())
		{
			_rec_hpi.erase(ii);
			_comp_rec.push_front(scopy(s));
		}
		if ((ii = findword(_rec_exam, s)) != _rec_exam.end())
		{
			_rec_exam.erase(ii);
			_comp_rec.push_front(scopy(s));
		}
	}
}






