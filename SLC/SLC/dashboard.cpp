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
const int T_space = 5 * 20;  // 5pt space in twips
	// point sizes of various elements (in half points)
const int ps_def = 11*2;
const int ps_title = 11*2;
const int ps_exam = 8*2;
const int ps_heading = 11*2;
const int ps_link = 10*2;
const int ps_warning = 12*2;

	// current file to which we're outputting
	//  it's global to this module for convenience
static FILE *outf = NULL;  


enum colors_t { 
	c_ignore = 0, c_black=1, c_white, c_gray, 
	c_hyperlink, c_highlight_req, c_highlight_comp, 
	c_foreground_req, c_foreground_comp, c_background,
	c_sepbar_a, c_sepbar_b, c_heading, c_warning,
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
	{0,0,255}, {250, 250, 250}, {192,192,192}, 
		// c_hyperlink, c_highlight_req, c_highlight_comp
	{0,0,0}, {160, 160, 160}, {241,217,198},
		// c_foreground_req, c_foreground_comp, c_background
	{192, 192, 192}, {160, 160, 160}, {127,127,127}, {255,0,0},
		// sepbar_a, sepbar_b, heading, warning
};
int bar_colors[][3] = {
	// now the list of the colors for the progress bar
	//  ... but no explicit enum names for any past c_bar_0,
	//  ... so these must stay at the end of the colortbl
	{229,229,229},  // c_bar_inc == gray90
		// the colors are a progression through HSL space
		// of (h, 100%, 50%) for (h=0; h<=120; h+=12),
		// but with a deeper green for 100%
	{255,0,0}, {255,50,0}, {255,102,0}, {255,153,0},
	{255,204,0}, {255,255,0}, {204,255,0}, {153,255,0},
	{101,255,0}, {50,255,0}, {0,170,0}
};

	// RTF encoding for the background color
const int cc_background = rgb_colors[c_background][c_red] * 256 * 256
	+ rgb_colors[c_background][c_green] * 256
	+ rgb_colors[c_background][c_blue];
const int color_count = (sizeof(rgb_colors)/sizeof(rgb_colors[0]));
const int bar_count = (sizeof(bar_colors)/sizeof(bar_colors[0]));

void R_prolog(void)
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

void R_icon(void)
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

void R_title(void)
{
	fprintf(outf, "{\\pard\\fs%d\n", ps_title);
	R_icon();
	fprintf(outf, "{ }\\b RSQ{\\super \\'a9} Guidance\\sa200\\par}");
	fprintf(outf, "\\fs%d\n", ps_def);
}

void R_epilog()
{
	fprintf(outf, "}\n");
}

void R_separator(int color)
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

void R_hyperlinks(void)
{
	list<char *>::iterator i;
	for (i = _links.begin();  i != _links.end();  i++)
	{
			// do we need to prepend "www."?
		char *www = (strncmp(*i, "www.", 4) == 0) ? "" : "www.";
		fprintf(outf, "\\pard\\li%d{\\field{\\*\\fldinst{HYPERLINK %s%s}}\n", 
			T_space, www, *i);
		fprintf(outf, "{\\fldrslt{\\ul\\fs%d\\cf%d %s%s}}}\\par\n", 
			ps_link, c_hyperlink, www, *i);
	}
}

void R_heading(char *head, int color)
{
	fprintf(outf, "\\pard\\s0{\\fs%d\\cf%d\\b %s}\\par\n", 
		ps_heading, color, head);
}

void R_line(void)
{
	// fprintf(outf, "\\line");
}

	// provide a little vertical space
void R_vertspace(int points)
{
	fprintf(outf, "{\\pard\\fs%d\\sl0\\sa0\\par}\n", points*2);
}


void R_group(char *hdr, list<char *> L, int color)
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


void R_complaints(void)
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


void R_progressbar(char *title, int documented, int needed)
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
		R_heading(title, c_heading);
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
	R_vertspace(5);
}

void S_testColorBars(void)
{
		// for grins, let's check the bar colors:
		//  add some blank space and a number of sample colors
	const int samples = 33;
	const int blanks = 2;

	for (int i = 0;  i < blanks;  i++)  
		fprintf(outf, (i && (i%10)==0) ? "\\line\n" : "\\line");
	fprintf(outf, "\n");

	R_separator(c_black);
	R_heading("Progress bar color samples....", c_black);

	for (int i = 0;  i <= samples;  i++)
		R_progressbar("", i, samples);
	R_vertspace(20);
}

void S_generateDash(void)
{
	S_sortStatus();

		// open the dashboard
	if (outf != NULL)
	{		// already open!?!?
		fprintf(stderr, "already have an output file!!!!!\n");
		exit(2);
	}
	if ((outf = fopen(DASHBOARD_PATH, "w")) == NULL)
	{
		fprintf(stderr, "cannot open dashboard file!!!!!\n");
		exit(1);
	}

	R_prolog();
		// header
	R_title();
		// presenting complaint(s)
	R_complaints();

	R_line();

		// resource links
	R_hyperlinks();
	R_line();
	R_separator(c_sepbar_b);

		// required components
	int n_req_still_need = _req_hpi.size() + _req_exam.size() + _assess.size();
	R_progressbar("Required", _comp_req.size(),
		n_req_still_need + _comp_req.size());
	R_heading("Incomplete", c_heading);
	R_vertspace(5);
	R_group("HPI", _req_hpi, c_foreground_req);
	R_group("Exam", _req_exam, c_foreground_req);
	R_group("Assessment", _assess, c_foreground_req);
	R_vertspace(2);
	R_line();
	R_separator(c_sepbar_a);
	R_heading("Completed", c_heading);
	R_vertspace(5);
	R_group("", _comp_req, c_foreground_comp);
	// R_group("HPI", comp_req_hpi, n_c_req_hpi, c_foreground_comp);
	// R_group("Exam", comp_req_exam, n_c_req_exam, c_foreground_comp);
	// R_group("Assessment", comp_assess, n_c_assess, c_foreground_comp);
	R_vertspace(2);
	R_line();
	R_separator(c_sepbar_b);

		// recommended components -- only a progress bar
	int n_rec_still_need = _rec_hpi.size() + _rec_exam.size();
	R_progressbar("Recommended",
		_comp_rec.size(), n_rec_still_need + _comp_rec.size());
	R_line();

		// differential diagnoses
	if (_differential)
	{
		fprintf(outf, "{\\pard{\\b Differential:} %s\\sb200\\sa200\\par}", 
			_differential);
#if 0	// say nothing if there's no differential diagnosis
	} else
	{
		fprintf(outf, "{\\pard{\\b Differential:} %s\\sb200\\sa200\\par}",
			"no differential diagnosis");
#endif
	}

	// S_testColorBars();

		// finish
	R_epilog();
	fclose(outf);
	outf = NULL;

		// once we've finished generating the dashboard,
		// check if we've got a validation request from the EHR;
		// Validate() generates a warning box, if necessary
	if (validation_required)
		Validate();
}


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


void R_one_warn_icon(int wid, int ht)
{
	fprintf(outf,
		"{\\pict\\wmetafile8\\picw%d\\pich%d\\picwgoal%d\\pichgoal%d\n",
		warn_picw, warn_pich, wid, ht);
	for (int i = 0;  i < warn_icon_length;  i++)
		fprintf(outf, "%s\n", warn_icon[i]);
	fprintf(outf, "}");
}

void R_warning_icons(void)
{
	int wid = 450, ht = 420; 
	int lasttab = T_width+300;
	// int midtab = lasttab / 2;
	int gap = (lasttab - 3*wid)/3 - 20;
	// fprintf(outf, "{\\tx%d\\tx%d", midtab, lasttab);
	fprintf(outf, "{\\tx%d\\tx%d\\tx%d",
		wid+gap, 2*(wid+gap), 3*(wid+gap));
	R_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	R_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	R_one_warn_icon(wid, ht);
	fprintf(outf, "\\tab");
	R_one_warn_icon(wid, ht);
	fprintf(outf, "\\par}\n");

}

	// now produce the warning box if we have any warnings in the list
void S_generateWarn(void)
{
		// we've got nothing to say
	if (_warnings.empty())
	{
		D_removeWarningBox();
		return;
	}

	if (outf != NULL)
	{		// already open!?!?
		fprintf(stderr, "already have an output file!!!!!\n");
		exit(2);
	}
	if ((outf = fopen(WARN_PATH, "w")) == NULL)
	{
		fprintf(stderr, "cannot open warning file!!!!!");
		exit(1);
	}
	R_prolog();
	R_warning_icons();
	R_vertspace(10);
	list<char *>::iterator i;
	for (i = _warnings.begin();  i != _warnings.end();  i++)
	{
		fprintf(outf, "{\\pard\\fs%d\\cf%d\\li%d\\ri%d %s\\par}\n",
			ps_warning, c_black, T_space*2, T_space*2, *i);
		R_vertspace(2);

	}
	R_vertspace(10);
	// R_vertspace(21);
	// R_warning_icons();
	R_epilog();
	fclose(outf);
	outf = NULL;
}



