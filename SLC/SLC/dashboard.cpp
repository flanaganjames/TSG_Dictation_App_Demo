/*************************************************************************
This module produces the dashboard.  
The main entry point is S_generatedash.
*************************************************************************/

#include "stdafx.h"
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "sullivan.h"
#include "parser.h"
#include "icons.h"

const int T_width = 72 * 20 * 2;	// 2 inches in RTF twips
const int T_inch = 72 * 20;		// 1 inch in twips
const int T_billingtab = T_inch;  // .5 inches
const int T_space = 5 * 20;  // 5pt space in twips
	// point sizes of various elements (in half points)
const int ps_def = 11*2;
const int ps_title = 11*2;
const int ps_exam = 8*2;
const int ps_billing = 8*2;
const int ps_bill_lvl = 6*2;
const int ps_heading = 11*2;
const int ps_subheading = 9*2+1;
const int ps_link = 10*2;
const int ps_warning = 12*2;
const int ps_warning_link = 11*2;

	// current file to which we're outputting
	//  it's global to this module for convenience
static FILE *outf = NULL;  


enum colors_t { 
	c_ignore = 0, c_black=1, c_white, c_gray, 
	c_hyperlink, c_highlight_req, c_highlight_comp, 
	c_foreground_req, c_foreground_comp, c_background,
	c_sepbar_a, c_sepbar_b, c_heading, c_subheading,
	c_billing, c_billhdr,
	c_warning,
	/*
		IMPORTANT: These next two colors need to always
		end the enumeration.  c_bar_inc is the gray
		for the incomplete part of the bar; c_bar_0 is
		the first of the cascading colors for the bar.
		These are the first two colors in bar_color[][].
	*/
	c_bar_inc, c_bar_0
};
enum rgb_t { c_red = 0, c_green, c_blue };
int rgb_colors[][3] = {
	{0,0,0}, {0,0,0}, {255, 255, 255}, {192,192,192}, 
		// c_ignore, c_black, c_white, c_gray,
	{0,255,0}, {250, 250, 250}, {192,192,192}, 
		// c_hyperlink, c_highlight_req, c_highlight_comp
	{0,0,0}, {160, 160, 160}, {241,217,198},
		// c_foreground_req, c_foreground_comp, c_background
	{192, 192, 192}, {160, 160, 160}, {127,127,127}, {160,160,160},
		// sepbar_a, sepbar_b, heading, subheading
	{0,0,0}, {0,0,0}, // {0, 192, 192},
		// billing_element, billing_element_hdr
	{255,0,0},
		// warning foreground
};
int bar_colors[][3] = {
	// now the list of the colors for the progress bar
	//  ... but no explicit enum names for any past c_bar_0,
	//  ... so these must stay at the end of the colortbl
	{229,229,229},  // c_bar_inc == gray90
		// the colors are a progression through HSL space
		// of (h, 100%, 50%) for (h=0; h<=63; h+=7),
		// but with a deeper green for 100%
	{255,0,0}, {255,29,0}, {255,59,0}, {255,89,0},
	{255,119,0}, {255,148,0}, {255,178,0}, {255,208,0},
	{255,229,0}, {242,255,0}, {0,170,0},
};

	// RTF encoding for the background color
const int cc_background = rgb_colors[c_background][c_red] * 256 * 256
	+ rgb_colors[c_background][c_green] * 256
	+ rgb_colors[c_background][c_blue];
const int color_count = (sizeof(rgb_colors)/sizeof(rgb_colors[0]));
const int bar_count = (sizeof(bar_colors)/sizeof(bar_colors[0]));

void D_openoutput(char *path, char *name)
{
	if (outf != NULL)
	{		// already open!?!?
		fprintf(stderr, "already have an output file!!!!!\n");
		exit(2);
	}
	if ((outf = fopen(path, "w")) == NULL)
	{
		fprintf(stderr, "cannot open %s file!!!!!\n", name);
		exit(1);
	}
}

void D_closeoutput(void)
{
	fclose(outf);
	outf = NULL;
}

void D_prolog(void)
{
	fprintf(outf, "{\\rtf1\\ansi\\ansicpg1252\\lang1033\\deff0");
	fprintf(outf, "\\fs%d\\sl24\\slmult0\\viewbksp1\\margr%d\n", 
		ps_def, T_width + 2*T_space);
		// font table
	fprintf(outf, "{\\fonttbl{\\f0\\fswiss Verdana;}");
	fprintf(outf, "{\\f1\\froman Times New Roman;}}\n");
		// color table
	fprintf(outf, "{\\colortbl;");
	for (int i = 1;  i < color_count;  i++)
	{
		fprintf(outf, "\\red%d\\green%d\\blue%d;", 
			rgb_colors[i][c_red], rgb_colors[i][c_green], rgb_colors[i][c_blue]);
		if ((i%3)==0)  fprintf(outf,"\n");
	}
	if ((color_count%3) == 0)  fprintf(outf, "\n");
	for (int i = 0;  i < bar_count; i++)
	{
		fprintf(outf, "\\red%d\\green%d\\blue%d;", 
			bar_colors[i][c_red], bar_colors[i][c_green], bar_colors[i][c_blue]);
		if ((i%3)==2)  fprintf(outf,"\n");
	}
	fprintf(outf, "}\n");
}

void D_icon(void)
{
	// int wid = 240, ht = 225;
	int wid = 288, ht = 270; 
	fprintf(outf, 
		"{\\pict\\wmetafile8\\picw%d\\pich%d\\picwgoal%d\\pichgoal%d\n",
		tsg_picw, tsg_pich, wid, ht);
	for (int i = 0;  i < tsg_icon_length;  i++)
		fprintf(outf, "%s\n", tsg_icon[i]);
	fprintf(outf, "}");
}

void D_title(void)
{
	fprintf(outf, "{\\pard\\fs%d\n", ps_title);
	D_icon();
	fprintf(outf, "{ }\\b RSQ{\\super \\'ae} Guidance\\sa200\\par}");
	fprintf(outf, "\\fs%d\n", ps_def);
}

void D_epilog()
{
	fprintf(outf, "}\n");
}

void D_separator(int color)
{
	// example:
	// {\pard\fs10\sl20{ }\par}
	// {\pard\fs5\tx5240\highlight4{}\tab\par}
	// {\pard\fs10\sl20{ }\par}
	fprintf(outf, "{\\pard\\fs10\\sl20{ }\\par}\n");
	fprintf(outf, "{\\pard\\fs5\\tx%d\\highlight%d{}\\tab\\par}\n", 
		T_width+2*T_space, color);
	fprintf(outf, "{\\pard\\fs10\\sl20{ }\\par}");
}

void D_showOneLink(char *l, int size, int height)
{
	fprintf(outf, "\\pard\\li%d{\\field", T_space);
		// fldinst is ignored by .Net's RTF, so we don't output it;
		// the abbreviated link is actually interpreted by the
		// dashboard display program -- yes, that's a little
		// Rube Goldberg but allows us to use short link names
	// fprintf(outf, "{\\*\\fldinst{HYPERLINK %s%s}}\n", www, l);
		// we now use the text of the hyperlink to give us the
		// vertical position in the window
	fprintf(outf, "{\\*\\fldinst{HYPERLINK %d }}", height);
	fprintf(outf, "{\\fldrslt{\\ul\\fs%d\\cf%d %s}}}\\par\n",
		size, c_hyperlink, l);
}

void D_hyperlinks(int baseheight)
{
	char *l;
	int n = 0;
		/*
		 * We need to provide the dashboard with the position of the top
		 *  of each line of link, so the dashboard can properly position
		 *  the buttons for the links.  Note that this is the top of the
		 *  line, not the baseline, and it's in half-points, not pixels,
		 *  and the position of the first line is supplied by the caller,
		 *  based on what the caller knows about what's above the links in
		 *  the panel.  The height of the first link is given by the
		 *  argument, and we know each link line is 24hp high.
		 */
	list<char *>::iterator i;
	for (i = _links.begin();  i != _links.end();  i++)
	{
			// is it a local file link?
			// (though this may not work correctly in the RTF)
		if (strncmp(*i, "local:", 6) == 0)
		{
			l = (*i)+6;
		} else {
			l = *i;
		}
		D_showOneLink(l, ps_link, baseheight+n*24);
		n++;
	}
}

void D_heading(char *head, int color)
{
	fprintf(outf, "\\pard\\s0{\\fs%d\\cf%d\\b %s}\\par\n", 
		ps_heading, color, head);
}

void D_subheading(char *head, int color)
{
	fprintf(outf, "{\\pard\\s0{  }\\fs%d\\cf%d\\b %s}\\par\n", 
		ps_subheading, color, head);
}


void D_line(void)
{
	// fprintf(outf, "\\line");
}

	// provide a little vertical space
void D_vertspace(int points)
{
	fprintf(outf, "{\\pard\\fs%d\\sl0\\sa0\\par}\n", points*2);
}


void D_group(char *hdr, list<char *> L, int color)
{
	// example:
	// {\pard\fs16\sl0\sa0\par}
	// {\pard\cf0 { }Movement\par}
	// {\pard\cf0 { }TAD Risk Factors\par}
	//.... note no group hdr is shown in this version
	list<char *>::iterator i;
	for (i = L.begin();  i != L.end();  i++)
	{
		fprintf(outf, "{\\pard\\fs%d\\cf%d { }%s\\par}\n",
			ps_exam, color, *i);
	}
}


void D_complaints(void)
{
	list<char *>::iterator i;
	for (i = _complaint.begin();  i != _complaint.end();  i++)
	{
		fprintf(outf, "{\\pard\\li%d {%s}\\sa60\\par}\n", 
			T_space, *i);
	}
	if (_complaint.size() == 0)
	{
		fprintf(outf, "{\\pard\\li%d {%s}\\sa60\\par}\n", 
			T_space, "(no presenting complaint)");
	}	
}


void D_progressbar(char *title, int documented, int needed)
{
	// example:
	// {\tx477\tx4340\tx4380\tx5040
	// \highlight1{}\tab\highlight0{}\tab\highlight1{}\tab\highlight0{ 11%}\tab\par}
	int percent = 100 * documented / (needed ? needed : 1);
	percent = (percent > 100) ? 100 : 
		((percent < 0) ? 0 : percent);
	int barlength = T_width - 700;
	int barused = barlength * percent / 100;
	int barstop = 40;  // width of end marker/buffer for the graph
	int quanta = int(100/(bar_count-2));
	int barcolor = c_bar_0 + int(percent/quanta);
	char pc[20];
	sprintf_s(pc, 20, " %d%%", percent);

	if (strlen(title) > 0)
	{
		D_heading(title, c_heading);
	}
	if (percent == 0)  barused = barstop;
	// fprintf(outf, "{\\tx%d\\tx%d\\tx%d\\tx%d\n", 
		// barused, barlength, barlength+barstop, T_width);
	if (percent < 100)
	{
		fprintf(outf, "{\\tx%d\\tx%d\\tx%d\n", barused, barlength, T_width);
		fprintf(outf, "\\highlight%d{}\\tab\\highlight%d{}\\tab", 
			barcolor, c_bar_inc);
	} else {
		fprintf(outf, "{\\tx%d\\tx%d\n", barlength, T_width);
		fprintf(outf, "\\highlight%d\\tab", barcolor);
	}
	fprintf(outf, "\\highlight%d{ %d%%}\\tab\\par}\n", c_ignore, percent);
	D_vertspace(5);
}

void D_testColorBars(void)
{
		// for grins, let's check the bar colors:
		//  add some blank space and a number of sample colors
	const int samples = 33;
	const int blanks = 2;

	for (int i = 0;  i < blanks;  i++)  
		fprintf(outf, (i && (i%10)==0) ? "\\line\n" : "\\line");
	fprintf(outf, "\n");

	D_separator(c_black);
	D_heading("Progress bar color samples....", c_black);

	for (int i = 0;  i <= samples;  i++)
		D_progressbar("", i, samples);
	D_vertspace(20);
}


void D_backgroundColor(int r, int g, int b)
{
	fprintf(outf, "{\\*\\background {\\shp{\\*\\shpinst\\shpleft0\\shptop0");
	fprintf(outf, "\\shpright0\\shpbottom0\\shpfhdr0\\shpbxmargin\n");
	fprintf(outf, "\\shpbxignore\\shpbymargin\\shpbyignore\\shpwr0");
	fprintf(outf, "\\shpwrk0\\shpfblwtxt1\\shpz0\\shplid1025\n");
	fprintf(outf, "{\\sp{\\sn shapeType}{\\sv 1}}");
	fprintf(outf, "{\\sp{\\sn fFlipH}{\\sv 0}}");
	fprintf(outf, "{\\sp{\\sn fFlipV}{\\sv 0}}\n");
	fprintf(outf, "{\\sp{\\sn fillColor}{\\sv %d}}",
		256*256*r + 256*g + b);
	fprintf(outf, "{\\sp{\\sn fFilled}{\\sv 1}}\n");
	fprintf(outf, "{\\sp{\\sn lineWidth}{\\sv 0}}");
	fprintf(outf, "{\\sp{\\sn fLine}{\\sv 0}}\n");
	fprintf(outf, "{\\sp{\\sn bWMode}{\\sv 9}}");
	fprintf(outf, "{\\sp{\\sn fBackground}{\\sv 1}}");
	fprintf(outf, "{\\sp{\\sn fLayoutInCell}{\\sv 1}}}}}\n");
}

/******************************************************************************
*******************************************************************************
	E/M panel
*******************************************************************************
******************************************************************************/

	/*
	 * naming conventions (where XX is one of HPI, ROS, PFSH, Exam):
	 *    (.... however: Exam is reported out in the E/M advice as "PE")
	 * score_XX -- the function to generate the score
	 * XX_items -- table of minimum items per billing level
	 * XX_levels -- the names of the billing levels
	 * Level_t -- the enum for the score levels, which are Ln
	 * XX_score -- the value of the score recorded in D_billingScore
	 * but see D_billingScore(), below, for more
	 */

	// we have a list of the billing levels
enum Level_t { L0 = 0, L1, L2, L3, L4, L5 };

	// the minimum number of items to enter each level
	// (we report the max level possible)
	//              L0, L1, L2, L3, L4, L5
int HPI_items[]  = { 0,  1,  1,  1,  4,  4};
int ROS_items[]  = { 0,  0,  1,  2,  2, 10};
int PFSH_items[] = { 0,  0,  0,  0,  1,  2};
int Exam_items[] = { 0,  1,  1,  2,  6,  8};

	/*
	 * here are the names for the billing levels for each category
	 * (we may not need these in production, but some test scenarios
	 *  want to use them)
	 */
char *HPI_levels[] = { "", "", "None", "Brief", "", "Extended" };
char *ROS_levels[] = { "", "none", "PP", "", "Extended", "Complete" };
char *PFSH_levels[] = { "", "", "", "none", "Problem Pertinent", "Complete" };
char *Exam_levels[] = { "none", "Problem Focused", "", "EPF", "Detailed", "Complete" };

	// take the array of minimum items per level, and a count of items
	//  and return the level
int EM_score(int *items, int count)
{
	int level = L0;
	int i;
	for (i = L0;  i <= L5;  i++)
	{
		if (items[i] == -1) continue;
		if (count >= items[i]) level = i;
	}
	return level;
}

	/*
	 * these next few functions format the billing data on the dashboard
	 *  (strictly, this should be a separate module, but
	 *   we need a lot of the dashboard data, so having it
	 *   local to the other dashboard functions makes sense
	 *   for the moment)
	 */
		// use the same set of tabstops for the billing panel
		//  (headline & summary should use some right justified tabs,
		//   but the .Net RTF control doesn't honor \\tqr)
void D_billingTabs(bool right)
{
	fprintf(outf, "{\\pard\\tx%d\\tx%d", 
		T_inch+1*T_space, 2*T_inch-2*T_space);
}

void D_billingHeading(void)
{
	D_billingTabs(true);
	fprintf(outf, "\\fs%d\\cf%d { }{\\b %s} {\\fs%d %s}",
		ps_billing, c_billhdr, "Level",
		ps_bill_lvl, "(1/2/3/4/5)");
	fprintf(outf, "\\tab %s\\tab %s\\par}\n",
		"Elements", "E/M Level");
}

void D_billingElement(char *head, char *levels, int count, int score)
{
	D_billingTabs(false);
	fprintf(outf, "\\fs%d\\cf%d { }{\\cf%d\\b %s} {\\fs%d %s}",
		ps_billing, c_billing, c_billhdr, head, 
		ps_bill_lvl, levels);
	fprintf(outf, "\\tab {      }%d\\tab {  }", count);
	fprintf(outf, (score == 0) ? "NA\\par}\n" : " %d\\par}\n", score);
}

void D_billingSummary(int score)
{
	D_billingTabs(false);
	fprintf(outf, "\\b\\fs%d\\cf%d { }%s\\tab {  }",
		ps_billing, c_billhdr, "    Potential E/M Level");
	fprintf(outf, "\\line{}                 c/w Hx and PE:\\tab {  }");
	fprintf(outf, (score == 0) ? "NA" : " %d", score);
	fprintf(outf, "\\par}\n");
	//   uncomment these next two to see tab positions
	// D_billingTabs(false);
	// fprintf(outf, "|\\tab|\\tab|\\tab|\\par}\n");
}


void D_billingScore(void)
{
		// begin by calculating the scores
	int HPI_count = _bill_hpi.size();
	int HPI_score = EM_score(HPI_items, HPI_count);
	int max_level = HPI_score;
	int ROS_count = _bill_ros.size();
	int ROS_score = EM_score(ROS_items, ROS_count);
	max_level = __min(max_level, ROS_score);
	int PFSH_count = _bill_pfsh.size();
	int PFSH_score = EM_score(PFSH_items, PFSH_count);
	max_level = __min(max_level, PFSH_score);
	int Exam_count = _bill_exam.size();
	int Exam_score = EM_score(Exam_items, Exam_count);
	max_level = __min(max_level, Exam_score);
		// yeah, that's confusing: the routine to determine the
		// score is verb_noun, but the score variable is noun_noun
		// and the element count is XX_count, but the billing score
		// is XX_score

		// open the RTF file to contain the E/M panel
	D_openoutput(EM_PATH, "e/m panel");
	D_prolog();
	D_heading("E/M Review", c_heading);
	D_vertspace(2);  // D_vertspace(5);
	D_billingHeading();
	D_billingElement("HPI", "(1/1/1/4/4)", HPI_count, HPI_score);
	D_billingElement("ROS", "(0/1/2/2/10)", ROS_count, ROS_score);
	D_billingElement("PFSH", "(0/0/0/1/2)", PFSH_count, PFSH_score);
	D_billingElement("PE", "(1/1/2/6/8)", Exam_count, Exam_score);
	D_billingSummary(max_level);
	D_epilog();
	D_closeoutput();
}



/******************************************************************************
*******************************************************************************
	overall dashboard generation
*******************************************************************************
******************************************************************************/

	/*
	 * this is the parent routine for generating the dashboard,
	 *  calling all the utilities above named D_xxx
	 */
void S_generateDash(void)
{
	S_sortStatus();

		// open the dashboard
	D_openoutput(DASHBOARD_PATH, "dashboard");
	D_prolog();
		// header
	D_title();
		// presenting complaint(s)
	D_complaints();

	D_line();

		// resource links
	int height = 55 + 30 * max(1,_complaint.size()); // half-points
	D_hyperlinks(height);
	D_line();
	D_separator(c_sepbar_b);

		// required components -- now called "Recommended"
	int n_req_still_need = _req_hpi.size() + _req_exam.size() + _assess.size();
	D_progressbar("Recommended", _comp_req.size(),
		n_req_still_need + _comp_req.size());
	D_heading("Incomplete", c_heading);
	D_vertspace(2);  // D_vertspace(5);
	D_group("HPI", _req_hpi, c_foreground_req);
	D_group("Exam", _req_exam, c_foreground_req);
	D_group("Assessment", _assess, c_foreground_req);
	D_vertspace(2);  // D_vertspace(2);
	D_line();
	D_separator(c_sepbar_a);
	D_heading("Completed", c_heading);
	D_vertspace(2);  // D_vertspace(5);
	D_group("", _comp_req, c_foreground_comp);
	// D_group("HPI", comp_req_hpi, n_c_req_hpi, c_foreground_comp);
	// D_group("Exam", comp_req_exam, n_c_req_exam, c_foreground_comp);
	// D_group("Assessment", comp_assess, n_c_assess, c_foreground_comp);
	D_vertspace(2);
	D_line();


	/*********
	......... now eliding the "recommended" progress bar
		// recommended components -- only a progress bar
	int n_rec_still_need = _rec_hpi.size() + _rec_exam.size();
	D_progressbar("Recommended",
		_comp_rec.size(), n_rec_still_need + _comp_rec.size());
	D_line();
	**********/

		// differential diagnoses
	if (_differential)
	{
		D_separator(c_sepbar_b);
		fprintf(outf, "{\\pard{\\b Differential:} %s\\sb200\\sa200\\par}", 
			_differential);
#if 0	// say nothing if there's no differential diagnosis
	} else
	{
		D_separator(c_sepbar_b);
		fprintf(outf, "{\\pard{\\b Differential:} %s\\sb200\\sa200\\par}",
			"no differential diagnosis");
#endif
	}

		// finish main status panel in the dashboard
	D_epilog();
	D_closeoutput();

		// now generate the E/M score panel
	D_billingScore();

		// once we've finished generating the dashboard, call
		// S_Validate() to generate a warning box, if necessary
	S_Validate();
}


/******************************************************************************
*******************************************************************************
	warning box below the dashboard
*******************************************************************************
******************************************************************************/

	/*
	 * the next clump are the utilities to generate the
	 * warning box on the dashboard:
	 * if there are warnings, we generate an extra RTF box
	 * containing them to be added to the bottom
	 * of the dashboard
	 */

	// list of warnings for the dashboard
list<char *> _warnings;

	// clear the warnings list
void D_clearWarnings(void)
{
	_warnings.clear();
}

	// add a warning to the list to be displayed in the warning box
void D_addWarning(char *s)
{
	_warnings.push_back(scopy(s));
}


void D_one_warn_icon(int wid, int ht)
{
	fprintf(outf,
		"{\\pict\\wmetafile8\\picw%d\\pich%d\\picwgoal%d\\pichgoal%d\n",
		warn_picw, warn_pich, wid, ht);
	for (int i = 0;  i < warn_icon_length;  i++)
		fprintf(outf, "%s\n", warn_icon[i]);
	fprintf(outf, "}");
}

void D_warning_icons(void)
{
	int wid = 450, ht = 420; 
	int lasttab = T_width+300;
	// int midtab = lasttab / 2;
	int gap = (lasttab - 3*wid)/3 - 20;
	// fprintf(outf, "{\\tx%d\\tx%d", midtab, lasttab);
	fprintf(outf, "{\\tx%d\\tx%d\\tx%d",
		wid+gap, 2*(wid+gap), 3*(wid+gap));
	D_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	D_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	D_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	D_one_warn_icon(wid, ht);
	fprintf(outf, "\\par}\n");

}

	// now produce the warning box if we have any warnings in the list
void S_generateWarnBox(void)
{
	int height = 0;  // half-points from top

		// we've got nothing to say
	if (_warnings.empty())
	{
		D_removeWarningBox();
		return;
	}

	D_openoutput(WARN_PATH, "warning");
	D_prolog();
		// we add a background color, even though the DotNet
		// RTF control doesn't honor it, and the background
		// color must be supplied by the dashboard program
	D_backgroundColor(0,255,255);
	// D_warning_icons();
	D_vertspace(5);
	height += 10;
	list<char *>::iterator i;
	for (i = _warnings.begin();  i != _warnings.end();  i++)
	{
		D_one_warn_icon(225, 210);
		fprintf(outf, "{  }{\\pard\\fs%d\\cf%d\\b\\li%d\\ri%d %s\\par}\n",
			ps_warning, c_warning, T_space*2, T_space*2, *i);
		height += 32;
		if (_strnicmp(*i, "Check TAD Risk!", strlen(*i)) == 0)
		{
			D_showOneLink("TAD_Risk", ps_link, height);
				// comment out the above & uncomment the following for
				// bigger link text
			// D_showOneLink("TAD_Risk", ps_warning_link, height);
			height += 28;  // reflects height of box on warning panel
		}
		D_vertspace(2);
		height += 4;
	}
	D_epilog();
	D_closeoutput();
	return;
}



