/* 
	This is the code for the SLC parser
 */

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <ctype.h>

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
	// billing lists
list<char *> _bill_hpi, _bill_ros, _bill_pfsh, _bill_exam;
	// resource links
list<char *> _links, _wlinks;
	// vital signs -- one item per category
char *_VS_p, *_VS_r, *_VS_sbp, *_VS_dbp, *_VS_t;
	// vital sign values -- filled in by S_Validate()
int _VVS_p, _VVS_r, _VVS_sbp, _VVS_dbp;
float _VVS_t;
	// possible information on differential diagnosis
char *_differential;

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
	//// data_hpi_t, data_exam_t, // unused - should be removed
	data_t, 
	dataqual_exam_t, dataqual_hpi_t, dataqual_pfsh_t, dataqual_ros_t,
	dataqual_ros2_t,
	dataqual_t,
	bill_t, link_t, wlink_t, wlink_clear_t,
	delete_t, del_t,
	end_t, end_tt, reset_t, 
	vital_p_t, vital_r_t, vital_sbp_t, vital_dbp_t, vital_t_t,
	validate_t, ignore_t,
	unknown_t};
char *command_names[] = { "complaint", "state", "diff", "add",
	"req hpi", "req exam", "assess",
	"rec hpi", "rec exam", "recc hpi", "recc exam",
	//// "data hpi", "data exam", // unused - should be removed
	"data", 
	"dataqual exam", "dataqual hpi", "dataqual pfsh", "dataqual ros",
	"dataqual review of systems",
	"dataqual",
	"bill", "link", "wlink", "wlink_clear",
	"delete", "del",
	"end", "end_of_script",	"reset",
	"VS p", "VS r", "VS sbp", "VS dbp", "VS t",
	"validate", "ignore",
};
const int command_count = (sizeof(command_names)/sizeof(command_names[0]));

	// forward declaration
void addWords(list<char *> &, char *);


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
	_bill_hpi.clear();
	//  this outlines a general way of knowing the hpi billing criteria, 
	//    but we'll hardcode in S_sortStatus()
	// _bill_hpi_base.clear();
	// addWords(_bill_hpi_base, "location, current severity, onset");
	// addWords(_bill_hpi_base, "quality, duration, context");
	// addWords(_bill_hpi_base, "relievers, associated symptoms");
	_bill_ros.clear();
	_bill_pfsh.clear();
	_bill_exam.clear();
	_links.clear();
	_wlinks.clear();
	free(_differential);
	_differential = NULL;
		// don't bother to check if the warning box exists before
		// deleting: the unlink failure is benign
	_unlink(WARN_PATH);
		// clear the vital signs
	_VS_p = _VS_r = _VS_sbp = _VS_dbp = _VS_t = NULL;
	_VVS_p = _VVS_r = _VVS_sbp = _VVS_dbp = 0;
	_VVS_t = 0.0;
}


void S_reset(void)
{
	S_initStatus();
	clobberState();
	S_generateDash();
}


	// allocate a copy of the string
char *scopy(const char *s)
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

	// if we had a line like "Exam Constitutional", we want to
	// also credit it as "Constitutional Exam" as though we'd
	// seen "data Constitutional Exam".
	// This function assumes it's called with just "Constitutional",
	// and that we're only called with one exam keyword
void addWordsExam(char *s)
{
		// this assumes that there's only one exam keyword on line
	const size_t le = strlen("Exam ");
	const size_t ls = strlen(s);
	char *sss = (char *) malloc(ls+le+1);
	strncpy(sss, s, ls+1);
	strncat(sss, " Exam", ls+1);
	addWords(_all_complete, sss);
	free(sss);
}


	// given list class instance and a string containing a list of words, 
	// split the string at commas, and add the words that aren't already
	// in the list to the list
void addWords(list<char *> &in, char *add)
{
	char *s, *t;
	void undecorate(char *, bool);

		// strip out digits -- presumably exam counts
	undecorate(add, true);
		// now add to the array
	s = add;
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

			// peel off the trailing blanks
		char *q = s + l - 1;
		while (*q == ' ')
			*q-- = '\0';
		
			// add if it's not a duplicate
		if (findword(in, s) == in.end())
			in.push_back(scopy(s));
		
			// skip blanks & commas to the next word beginning
		s += l;
		if (t)  s = ++t;
		while (s && *s && (*s == ' ' || *s == ',')) s++;
	}
}



	// strip off the line-ending characters & trailing blanks
void chomp(char *s)
{
	char *t;
	t = strpbrk(s, "\r\n");
	if (t)  *t-- = '\0';
	while (t && *t == ' ')  *t-- = '\0';
}

	// strip decorations out of the input string in place
	// (digits is true if we also want to strip out numbers)
void undecorate(char *in, bool digits)
{
	char *t, *s;

		// skip ahead to the first char we want to strip out
		// (which may short-circuit the whole scan)
	size_t n = strcspn(in, digits ? "\\[]*0123456789" : "\\[]*");
	s = in+n;
	for (t = s;  s && *s;  s++)
	{
			// strip RTF -- backslash to a blank
		if (*s == '\\')
		{
			while (*s != ' '  &&  *s != '\0') { s++; }
			continue;
		}
			// strip the decoration around something like [**foo**];
			// if desired, remove digits, which are exam counts from dialogs
		if (*s != '['  &&  *s != ']'  &&  *s != '*'
			&&  (!digits || !isdigit(*s)))
			*t++ = *s;
	}
	*t = '\0';
}

	// this cleans up the link names to prevent blanks
	// (this may be redundant now)
void convert_blanks(char *s)
{
	char *t = s;
	while (t = strchr(s, ' '))
		*t = '_';
			// don't be too aggressive about stripping blanks
			// if we've got multiple links in one command, e.g.,
			// "link foo bar, baz bar" should convert to "foo_bar"
			// and "baz_bar", not "_baz_bar"
			// [don't need to worry about multiple __, since the
			// way we process the command line ensures single blanks
			// between arguments]
	for (t = s;  *t;  t++)
	{
		if (*t == ',' &&  t[1] == '_')  t[1] = ' ';
	}
}


bool no_complaint(void)
{
	return _complaint.empty();
}


	// is the command complete?  does the command we've
	// recognized actually end where we expect?
	// (prevents us mistaking substring at the beginning of
	//  the command for a full one -- e.g., data vs dataqual)
bool completeCommand(char *s, size_t n)
{
	return (s[n] == '\0' || s[n] == ' ');
}


void S_parseStatus(void)
{
	char line[MAX_PATH], *s;
	size_t le, lr;

#ifdef TESTING
	printf("entering parser...........\n");

	struct stat sb;
	if (stat(STATUS_PATH, &sb) < 0)
		printf("unable to stat!!\n");
	else
		printf("mode 0%03o; times %ld %ld %ld\n", sb.st_mode, 
			sb.st_ctime, sb.st_mtime, sb.st_atime);
#endif
	
	if (S_openStatus("r") < 0)
		return;

	while ((s = fgets(line, MAX_PATH, status_file)) != NULL)
	{
		int i;

		chomp(s);  // strip the EOL characters
		undecorate(s, false);
		
#ifdef TESTING
		printf("line: %s\n", line);
#endif
			// recognize the command part of the line
		for (i = 0;  i < command_count; i++ )
		{
			if (_strnicmp(s, command_names[i], strlen(command_names[i])) == 0
				&& completeCommand(s, strlen(command_names[i])))
			{
				s += strlen(command_names[i]); // skip the command
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
		case add_t:
			// this keyword comes from the MU, not the VB script,
			// so ignore for the moment
			break;
		case assess_t:
			addWords(_assess, s);
			break;
		case bill_t:
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
		case data_t:
			addWords(_all_complete, s);
				// special case:  if this was exam data, recursively 
				// also add the keywords to the _bill_exam list
				// also recursively add to the completed exams list
				// though the latter requires a special routine
			le = strlen("exam ");
			if (_strnicmp(s, "exam ", le) == 0)
			{
				addWords(_bill_exam, s+le);
				addWordsExam(s+le);
			}
				// parallel special case: if this was ROS data,
				// we may not have gotten the same keywords in
				// a "dataqual ROS" command, so add the keywords 
				// to _bill_ros
			lr = strlen("ROS ");
			if (_strnicmp(s, "ROS ", lr) == 0)
				addWords(_bill_ros, s+lr);
				//
				// parallel special case for PFSH data
			lr = strlen("PFSH ");
			if (_strnicmp(s, "PFSH ", lr) == 0)
				addWords(_bill_pfsh, s+lr);
			break;
		case dataqual_exam_t:
			addWords(_bill_exam, s);
			addWordsExam(s);
			break;
		case dataqual_hpi_t:
			addWords(_all_complete, s);
				// we add to the _bill_hpi list under control of the
				// sortHPIbilling() routine later, because HPI has 
				// so many special cases for E/M calculations
			break;
		case dataqual_pfsh_t:
			addWords(_bill_pfsh, s);
			break;
		case dataqual_ros_t:
		case dataqual_ros2_t:
			addWords(_bill_ros, s);
			break;
		case delete_t:
		case del_t:
			// currently informational and ignored
			break;
		case diff_t:
			if (_differential)
				free(_differential);
			_differential = (char *) malloc(strlen(s)+1);
			strcpy(_differential, s);
			break;
		case end_t:
		case end_tt:
			S_finishStatus();
			break;
		case link_t:
			convert_blanks(s);
			addWords(_links, s);
			break;
		case rec_hpi_t:
		case recc_hpi_t:
			addWords(_rec_hpi, s);
			break;
		case rec_exam_t:
		case recc_exam_t:
			addWords(_rec_exam, s);
			break;
		case req_hpi_t:
			addWords(_req_hpi, s);
			break;
		case req_exam_t:
			addWords(_req_exam, s);
			break;
		case reset_t:
			S_reset();
			break;
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
		case vital_p_t:
			_VS_p = scopy(s);
			break;
		case vital_r_t:
			_VS_t = scopy(s);
			break;
		case vital_sbp_t:
			_VS_sbp = scopy(s);
			break;
		case vital_dbp_t:
			_VS_dbp = scopy(s);
			break;
		case vital_t_t:
			_VS_t = scopy(s);
			break;
		case wlink_t:
			convert_blanks(s);
			addWords(_wlinks, s);
			break;
		case wlink_clear_t:
			_wlinks.clear();
			break;
		case validate_t:
		case ignore_t:
			// we now parse, but ignore, validation and ignore requests:
			// we always do validation, not just when requested
			break;
		default:
			break;
		}
	}

	fclose(status_file);
	status_file = NULL;
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

	// list of the HPI elements we count for billing
char *HPI_bill_list[] = {
	"modifiers", "severity",
	"location", "onset", "quality",
	"duration", "context", "associated symptoms",
};
char *HPI_bill_aliases[] = {
	"relievers", "modifiers",
	"aggravators", "modifiers",
	"maximum severity", "severity",
	"current severity", "severity",
	"radiation", "location",
	"movement", "location",
	"progression", "onset",
};
const int n_hpi_aliases = sizeof(HPI_bill_aliases) / sizeof(HPI_bill_aliases[0]) / 2;
const int n_hpi_billing = sizeof(HPI_bill_list) / sizeof(HPI_bill_list[0]);

/*
	This routine checks the name of a completed exam against the list
	of exams that count for HPI credit in the E/M calculation.  There
	are some exams that actually count against other categories in E/M.
	(For example, "relievers" and "aggravators" count as an HPI exam
	for "modifiers".)  That means this routine has a table of aliases
	so we can rename the given exam to a billable one.  In the end,
	if the (possibly renamed) exam is in the HPI_bill_list array, we 
	add it to the _bill_hpi list, where it can count into the HPI line
	on the E/M panel.
 */
void sortHPIBilling(char *s)
{
	list<char *>::iterator ii;
	int j;
	char *t = s;
		// we have some elements that need to count only once,
		// so we set the element name to an alias to store in 
		// the billing list
	for (j = 0;  j < n_hpi_aliases;  j++)
	{
		if (_strnicmp(t, HPI_bill_aliases[j*2], strlen(t)) == 0)
		{
			t = HPI_bill_aliases[j*2+1];
			break;
		}
	}

		// is it a word we might need to add to the billing list
	for (j = 0;  j < n_hpi_billing;  j++)
	{
		if(_strnicmp(t, HPI_bill_list[j], strlen(t)) == 0)
			break;
	}
	if (j == n_hpi_billing)
		return;

		// now see if the word needs to be added to the list
	if ((ii = findword(_bill_hpi, t)) == _bill_hpi.end())
	{
		_bill_hpi.push_back(scopy(t));
	}
}

#if DEBUG
void debug_list(FILE *pp, char *title, list<char *> &in)
{
	list<char *>::iterator i;
	fprintf(pp, "...... %s:  ", title);
	for (i = in.begin();  i != in.end();  i++)
		fprintf(pp, "%s, ", *i);
	fprintf(pp, "\n");
}
#endif


void S_sortStatus(void)
{
	list<char *>::iterator i, ii;
	char *s;

#if DEBUG
	FILE *pp = fopen("status.err", "w");
	debug_list(pp, "_all_complete before", _all_complete);
	debug_list(pp, "_req_hpi", _req_hpi);
	debug_list(pp, "_req_exam", _req_exam);
	debug_list(pp, "_assess", _assess);
	fclose(pp);
#endif
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
			// however, billing for HPI is a special case and
			// so we examine each item to see if it qualifies
			// for adding to the HPI billing list
		sortHPIBilling(s);
	}
}






