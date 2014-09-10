/*
	This file contains old code we've used for generating the dashboard, which
	is now obsolete.
 */

#if 0
		// boilerplate for to generate overall background color in RTF
		// (this can probably be simpler, but I haven't had time to boil it down)
	fprintf(dash, "{\\*\\background {\\shp{\\*\\shpinst\\shpleft0\\shptop0\\shpright0\\shpbottom0\n");
	fprintf(dash, "\\shpfhdr0\\shpbxmargin\\shpbxignore\\shpbymargin\\shpbyignore\n");
	fprintf(dash, "\\shpwr0\\shpwrk0\\shpfblwtxt1\\shpz0\\shplid1025\n");
	fprintf(dash, "{\\sp{\\sn shapeType}{\\sv 1}}{\\sp{\\sn fFlipH}{\\sv 0}}{\\sp{\\sn fFlipV}\n");
	fprintf(dash, "{\\sv 0}}{\\sp{\\sn fillColor}{\\sv %d}}\n", cc_background);
	fprintf(dash, "{\\sp{\\sn fFilled}{\\sv 1}}{\\sp{\\sn lineWidth}{\\sv 0}}\n");
	fprintf(dash, "{\\sp{\\sn fLine}{\\sv 0}}{\\sp{\\sn bWMode}{\\sv 9}}{\\sp{\\sn fBackground}{\\sv1}}\n");
	fprintf(dash, "{\\sp{\\sn fLayoutInCell}{\\sv 1}}}}}\n\n");
#endif

#if 0
/* 
	Simple text dashboard
 */
void S_generateDash(void)
{
	fprintf(dash, "RSQ Guidance from the Sullivan Group\n");
	fprintf(dash, "for Presenting Complaint %s\n", complaint);

	printdash(dash, "\nResources: ", links, n_links, 4);
	fprintf(dash, "\nRequired Components\n");
	completion_score(dash, (n_c_req_hpi + n_c_req_exam + n_c_assess), 
		(n_req_hpi + n_req_exam + n_assess));
	fprintf(dash, "   Incomplete\n");
	printdash(dash, "      HPI: ", list_req_hpi, n_req_hpi, 10);
	printdash(dash, "      Exam: ", list_req_exam, n_req_exam, 10);
	printdash(dash, "      Assessment: ", list_assess, n_assess, 10);
	fprintf(dash, "   Completed\n");
	printdash(dash, "      HPI: ", comp_req_hpi, n_c_req_hpi, 10);
	printdash(dash, "      Exam: ", comp_req_exam, n_c_req_exam, 10);
	printdash(dash, "      Assessment: ", comp_assess, n_c_assess, 10);
	fprintf(dash, "\nRecommended Components\n");
	completion_score(dash, (n_c_rec_hpi + n_c_rec_exam), (n_rec_hpi + n_rec_exam));
	printdash(dash, "      HPI: ", list_rec_hpi, n_rec_hpi, 10);
	printdash(dash, "      Exam: ", list_rec_exam, n_rec_exam, 10);
	fprintf(dash, "   Completed\n");
	printdash(dash, "      HPI: ", comp_rec_hpi, n_c_rec_hpi, 10);
	printdash(dash, "      Exam: ", comp_rec_exam, n_c_rec_exam, 10);
	fprintf(dash, "\nDifferential: %s\n", 
		differential ? differential : "no differential diagnosis");
}
#endif

#if 0
	// version of the group display that uses highlighted boxes
	//  selected by tab widths
void R_group(char *hdr, char **list, int count, int color)
{
	// example:
	//	{\tx300\tx1960\tx5040
	//\highlight0\tab {\i HPI}\tab\highlight4 Movement\tab\par
	//\highlight0\tab\tab\highlight4 TAD Risk Factors\tab\par
	//\highlight0}
	int in, i;
	fprintf(dash, "{\\tx%d\\tx%d\\tx%d", 3*T_space, 2150 - 2*T_space, T_width);
	for (int i = 0;  i < T_space * 20;  i += T_space)
		fprintf(dash, "\\tx%d", T_width+i); // insurance for wide items
	fprintf(dash, "\n\\highlight%d\\tab {\\i %s}\\tab", c_ignore, hdr);
	for (i = 0, in = 0;  i < count;  i++)
	{
		if (list[i] && strlen(list[i]))
		{
			if (in)
				fprintf(dash, "\\highlight%d\\tab\\tab", c_ignore);
			fprintf(dash, "\\highlight%d %s\\tab\\par\n", color, list[i]);
			in++;
		}
	}
	if (!in)
		fprintf(dash, "\\highlight%d {}\\tab{}\\par\n", color);
	fprintf(dash, "\\highlight%d}\n", c_ignore);
}
#endif


#if 0
/*
	This set of routines sets up the exam lists as tables;
	this is fine in RTF as rendered by Word or WordPad,
	but a System.Windows.Forms.RichTextBox doesn't honor
	table colors, so our tables aren't highlighted as planned
 */
void T_begin(int gap, int left)
{
	fprintf(dash, "{\\trowd\\trgaph%d\\trleft%d\n", gap, left);
}

void T_end(void)
{
	fprintf(dash, "\\row\n}\n");
}


void T_celldef(int extent, BOOL boxed, char *extra)
{
	if (boxed)
	{
		fprintf(dash, "\\clbrdrt\\brdrw15\\brdrs\\clbrdrl\\brdrw15\\brdrs\n");
		fprintf(dash, "\\clbrdrb\\brdrw15\\brdrs\\clbrdrr\\brdrw15\\brdrs\n");
	}
	if (strlen(extra) > 0)
	{
		fprintf(dash, "%s", extra);
	}
	fprintf(dash, "\\cellx%d\n", extent);
}

void T_cell(char *cell)
{
	fprintf(dash, "\\pard\\intbl {%s}\\cell", cell);
}
#endif


#if 0
	// prepare a category header and a list for the dashboard
	// taking care to fold the lines at a point
		// (this is a very brute-force way to do this:
		//  it needs to be completely rethought)
#define LINEWID 45
void printdash(FILE *dash, char *hdr, char **list, int count, int indent)
{
	int i;

	// the indent chunk
	char *blanks = (char *) malloc(indent + 1);
	memset(blanks, 0, indent+1);
	memset(blanks, ' ', indent);

	fprintf(dash, "%s\n", hdr);
	for (i = 0;  i < count;  i++)
	{
		if (strlen(list[i]))
			fprintf(dash, "%s%s\n", blanks, list[i]);
	}
}
#endif


#if 0 // line wrapping version of printdash
void printdash(FILE *dash, char *hdr, char **list, int count, int indent)
{
	int i;

	// do we really have data to print?
	if (count == 0 || list == NULL)
	{
		// even if not, print the header
		fprintf(dash, "%s\n", hdr);
		return;
	}

	// the indent chunk
	char *blanks = (char *) malloc(indent + 2);
	memset(blanks, 0, indent+2);
	memset(blanks, ' ', indent+1);
	*blanks = '\n';
	
	// begin by counting the total length we need
	int ll = strlen(hdr) + 3;
	for (i = 0;  i < count;  i++)  ll += strlen(list[i])+3;
	char *out = (char *) malloc(ll*3);

	// now collect the complete string we want to print
	*out = '\0';
	strncat(out, hdr, strlen(hdr));
	BOOL needcomma = FALSE;
	for (i = 0;  i < count; i++)
	{
		if (needcomma)
			strncat(out, ", ", 2);
		strncat(out, list[i], strlen(list[i]));
		needcomma = (*(list[i]) != '\0');
	}
	strncat(out, "\n", 1);

	// now chunk it into in DASHWIDTH lengths
	char *lines[50];
	int chunks = 0;
	size_t limit;
	for (char *s = out;  *s; )
	{
		char *l;
		limit = (chunks) ? (LINEWID-indent) : LINEWID;
		lines[chunks++] = s;
		l = s;
		if (strlen(s) < limit)
		{	// short final line
			break;
		}
		l = s;
		s += limit;  // max possible length
		while (s > l && *s != ',') s--; // back up to a comma
		if (s > l)
		{
			// we found a comma, so split at the next blank
			while (*s && *s != ' ') s++; // skip to a blank (normally only 1)
			*s++ = '\0';
		} else {
			// oops, we didn't have a comma within limit charactes,
			//  so let's split at a blank
			s += limit;
			while (s > l && *s != ' ') s--;
			if (s > l)
			{
				*s++ = '\0';
			} else {
				// oh, boy, we're screwed: drop an arbitrary break
				s += limit;
				*s++ = '\0';
			}
		}
	}

	// dump it all
	for (i = 0;  i < chunks;  i++)
	{
		if (i > 0)
			fprintf(dash, "%s", blanks);
		fprintf(dash, "%s", lines[i]);
	}

	free(out);
}
#endif


#if 0
void printdash(FILE *dash, char *h, char **s, int n, int indent)
{
	if (n == 0 || s == NULL)
		return;

	// gather the total length we need
	int ll = strlen(h) + 3;
	for (int i = 0;  i < n;  i++)   ll += strlen(s[i])+3;
	char *out = (char *) malloc(ll*3);

	// ll is now the count per line
	ll = strlen(h);
	*out = '\0';
	strncat(out, h, ll);
	for (int i = 0;  i < n;  i++)
	{
		int tl;
		char *t;
		if ((tl = strlen(s[i])) == 0)  continue;
		char e = out[strlen(out)-1];
		if (e != ',' && e != ' ')  
		{
			strcat(out, ", ");
			ll += 2;
		}
		if ((ll + tl) > LINEWID)
		{
			strncat(out, blanks, 1+indent+4);
			ll = 1+indent+4;
		}
			// copy by words into the output string
		for (t = s[i];  t && *t; )
		{
			int tl;
			char *tb = strchr(t, ' ');
			if (tb != NULL) 
			{
				tl = tb - t + 1;
				if( (ll+tl) > LINEWID)
				{
					strncat(out, blanks, 1+indent+4);
					ll = 1+indent+4;
				}
				strncat(out, t, tl+1);
				t = t + tl + 1;
				ll += tl + 1;
			} else {
				// didn't find a blank
				strncat(out, t, strlen(t));
				t = NULL;
				ll += strlen(t);
			}
		}
		// strncat(out, s[i], tl);
		// ll += tl;
	}

	fprintf(dash, "%s\n", out);
	free(out);
}
#endif

#if 0
/*
	This generates the dashboard contents from the data we parsed out 
	of the status file
 */
#include <time.h>

void completion_score(FILE *dash, int documented, int needed)
{
		// calculate the completion score
	int frac = 100 * documented / (needed ? needed : 1);
	if (frac > 100)	frac = 100;
	if (frac < 0)   frac = 0;
	fprintf(dash, " Percent completed: ");
#define BARGRAPH 20
		// begin with a bargraph of the score
	for (int i = 0;  i < BARGRAPH;  i++)
	{
		BOOL state = (frac >= ((i+1)*100/BARGRAPH));
		fprintf(dash, "%c", state ? ':' : '.');
	}
	fprintf(dash, " %d%%\n", frac);
}
#endif

#if 0
void S_generateDash(void)
{
	S_sortStatus();

	// open the dashboard
	FILE *dash = fopen(DASHBOARD_PATH, "w");
	fprintf(dash, "Recommended RSQ Documentation\n\n");
	
	// complaint
	fprintf(dash, "Presenting complaint: %s\n", complaint);

	// score
	completion_score((n_c_req_hpi + n_c_req_exam), (n_req_hpi + n_req_exam), dash);
	
	printdash(dash, "HPI still required: ", list_req_hpi, n_req_hpi, 4);
	printdash(dash, "Exams still required: ", list_req_exam, n_req_exam, 4);
	printdash(dash, "HPI Completed: ", comp_req_hpi, n_c_req_hpi, 4);
	if (n_c_req_exam > 0)
		printdash(dash, "Exams Completed: ", comp_req_exam, n_c_req_exam, 4);
	if (n_rec_hpi > 0)
	{
		printdash(dash, "Recommended HPI: ", list_rec_hpi, n_rec_hpi, 4);
	}
	if (n_rec_exam > 0)
	{
		printdash(dash, "Recommended Exams: ", list_rec_exam, n_rec_exam, 4);
	}
	printdash(dash, "\nResources: ", links, n_links, 4);


	fprintf(dash, "\nDifferential: %s\n", 
		differential ? differential : "no differential diagnosis");

	fclose(dash);
}
#endif


#if 0
	/* generate the progress bar using the table primitives */
	T_begin(0, 0);
	T_celldef(barused, TRUE, "\\clshdng10000");
	T_celldef(barlength, TRUE, "");
	T_celldef(T_width, FALSE, "");
	T_cell("");  T_cell("");  T_cell(pc);
	T_end();
#endif



