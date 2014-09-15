/*
 * common data between parser and dashboard
 */

#include <list>
using namespace std;

extern list<char *> _complaint;
	// incomplete required items
extern list<char *> _req_hpi, _req_exam, _assess;
	// incomplete recommended items
extern list<char *> _rec_hpi, _rec_exam;
	// items completed
extern list<char *> _all_complete, _comp_req, _comp_rec;
	// resource links
extern list<char *> _links;
	// vital signs -- one item per category
extern char *VS_p, *VS_r, *VS_t, *VS_sbp, *VS_dbp;
	// vital sign values
extern int VVS_p, VVS_r, VVS_sbp, VVS_dbp;
extern float VVS_t;
	// possible information on differential diagnosis
extern char *differential;

	// do we need validation?
extern bool validation_required;

char *scopy(const char *);
void D_clearWarnings(void);
void D_addWarning(char *);
void D_removeWarningBox(void);
