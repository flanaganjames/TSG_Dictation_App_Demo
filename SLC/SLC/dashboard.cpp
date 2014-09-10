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

static char *icon_data[] = {  // nominal 24x24 icon data
	"010009000003aa0300000000810300000000040000000301080005000000",
	"0b0200000000050000000c0219001900030000001e000400000007010400",
	"040000000701040081030000410b2000cc00180018000000000018001800",
	"000000002800000018000000180000000100180000000000c00600000000",
	"0000000000000000000000000000ad9c6bb5ad84ad9c73b5ad84ad9c6bb5",
	"a584ad9c73b5ad84ad9c73b5a584ad9c73b5ad84ad9c73b5ad84ad9c73b5",
	"ad84ad9c73b5ad84ad9c73b5ad84ad9c73b5ad84ad9c73b5ad84b5a573b5",
	"ad8cb5a57bb5ad8cb5a57bb5ad84b5a57bb5ad8cb5a57bb5ad8cb5a57bb5",
	"ad8cb5a57bb5ad8cb5a57bb5ad8cb5a57bb5ad8cb5a57bb5ad8cb5a57bb5",
	"ad8cb5a57bb5ad8c9c8442a58c529c8442a58c529c8442a58c529c8442a5",
	"8c529c8442a58c529c8442a58c529c8442a58c529c8442a58c529c8442a5",
	"8c529c8442a58c529c8442a58c529c8442a58c529c7b29947321947b2994",
	"7321947b29947b21947b29947321947b29947321947b29947321947b2994",
	"7321947b29947321947b29947321947b29947321947b29947321947b2194",
	"7321947321947b29947329947b29947321947b29947321947b2994732194",
	"7b299473219c7b29947321947b29947321947b29947b21947b2994732194",
	"7b29947b21947b29947321947b29947b29947b219c7b29947b29947b2994",
	"73219c7b29947b29947b299473219c7b29947b299c7b29947321947b2994",
	"7b299c7b29947b299c7b29947b29947b29947b299c7b29947b2994732994",
	"7b29947321947b29947329947b29947321947b29947321947b2994732194",
	"7b29947321947b29947321947b29947321947b29947321947b2994732194",
	"7b29947321947b299c7b29947b29947b29947b299c7b29947b29947b2994",
	"73219c7b29947b299c7b29947321947b29947b299c7b29947b29947b2994",
	"7b299c7b29947b29947b299473299c7b29947b29947321947b2994732194",
	"7b29947321947b298c7321947b29947318947b29947329947b2994731894",
	"7b29947b29947b29947318947b29947329947b29947318947b2994732994",
	"7b29947b298c73299c7b2184844a5a9cbd5a94b584845a9473217b8c735a",
	"94b552a5d65a94bd7b8c6b947329947b298c7b29738c7b5a94bd52a5d65a",
	"94ad84845a947321947b29947329846b218c73298c6b18737b6331adff39",
	"adff6b84738c6b184a9cd639adff39a5ff39adff39adff6b847b846b185a",
	"94bd39adff39adff39a5ff39adff39a5ff6b8c848c6b18847329846b2984",
	"6b218c6b18737b5a42adff42a5ff6b84738c63107384736b847b73846b4a",
	"9ce739b5ff5294b573847339a5ff42adff5a8ca56b84734a9cd642adff42",
	"9ce7847339846b21846b21846b29846b18737b6339a5ff42adff6b7b738c",
	"6b1873734a529cc642a5f739adff31adff638c945a8c9439adff42a5ef7b",
	"73425294b539adff31adff39adff6b7b638c6b18846b29846b218c6b2173",
	"7b5a39adff39a5ff6b84738c6b10529cce39adff42adff4a9cce638c9484",
	"6b29638c9439adff42adff737b637384635a94ad638c946384847b7b4a84",
	"6b18846b218c6b185a8c9c4aa5e739a5ff42a5ff429cef638c944a94c639",
	"adff429ce7638c945a8c9c84733973734a42a5ff39adff4aa5e7528cad52",
	"9cce4a94ce6b846b846318846b298c7b29947310529cd631adff39adff39",
	"adff31b5ff529cbd7b846342a5ef39adff39adff39adff7384739473186b",
	"8c8c42adff39adff39b5ff31adff4aadff738c7b9473218c73298c732994",
	"7b2173846b6b8c8c6b8c846b8c8c6b8c8c7b8c6b94732184844a6b8c8c63",
	"949c6b847b8c7b39947321947b217b7b526b8c8c638c9c738c84847b3994",
	"7b218c7329947b29947b298c73299c7b219c73109c73189473109c731894",
	"7318947b299473219c7b189473189c7b18947321947b29947b299c7b2194",
	"73189c7b21947318947b21947329947b29947b298c7321947b298c732994",
	"7b298c7329947b298c7329947b29947321947b298c7329947b2994732994",
	"7b298c7321947b29947329947b298c7321947b298c7329947b298c732194",
	"7b29947b29947b29947b29947329947b29947321947b29947321947b2994",
	"7321947b29947321947b29947329947b29947b21947b29947b29947b2994",
	"7321947b29947b29947b299473218c7321947b298c7321947b298c732194",
	"7b298c7321947b298c7321947b29947321947b298c7321947b2994732194",
	"7b298c7321947b298c7321947b298c7321947b29947321947b29947b2994",
	"73219c7b29947b29947b29947321947b29947b29947b29947321947b2994",
	"7321947b29947b21947b29947321947b29947b29947b29947321947b2994",
	"7329947b29947b29947321947b29947321947b298c7321947b2994732194",
	"7b298c7321947b29947321947b29947321947b29947321947b2994732194",
	"7b298c7321947b29947321947b298c7321947b299c7b29947b29947b298c",
	"7321947b29947b29947b298c7321947b29947b219c7b29947321947b2994",
	"7321947b29947b29947b29947321947b29947b29947b29947321947b2994",
	"7321040000002701ffff030000000000",
};
const int icon_length = (sizeof(icon_data)/sizeof(icon_data[0]));

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
		// sepbar_a = gray50, sepbar_b = gray60, heading, warning
};
int bar_colors[][3] = {
	// now the list of the colors for the progress bar
	//  ... but no explicit enum names for any past c_bar_0,
	//  ... so these must stay at the end of the colortbl
	{229,229,229},  // c_bar_inc == gray90
	{0,0,0}, // c_bar_0 == 0% done == black
	{115,115,115}, {127,127,127}, {140,140,140}, {153,153,153}, 
		// 10%, 20%, 30%, 40% = gray45, gray50, gray55, gray60
	{166,166,166}, {179,179,179}, {191,191,191}, 
		// 50%, 60%, 70% = gray65, gray70, gray75,
	{0,139,0}, {0,170,0},  // 80%, 90% == green4, arbitrary
	// {193,255,193}, {84,255,159},
		// 80%, 90% == DarkSeaGreen, SeaGreen
	// {0,139,0}, {152,251,152},
		// 80%, 90% = green4, green3
	{0,255,0}	// 100% = green1
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
		"{\\pict\\wmetafile8\\picw423\\pich397\\picwgoal%d\\pichgoal%d\n",
		wid, ht);
	for (int i = 0;  i < icon_length;  i++)
		fprintf(outf, "%s\n", icon_data[i]);
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
	if (differential)
	{
		fprintf(outf, "{\\pard{\\b Differential:} %s\\sb200\\sa200\\par}", 
			differential);
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
}


	/*
	 * generate an extra RTF box containing a warning
	 * (with the specified text) to be added to the bottom
	 * of the dashboard
	 */
void S_generateWarn(char *warn)
{
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
	R_vertspace(20);
	fprintf(outf, "\\pard\\s0{\\fs%d\\cf%d\\b\\i %s}\\par\n",
		ps_warning, c_warning, warn);
	R_epilog();
	fclose(outf);
	outf = NULL;
}



