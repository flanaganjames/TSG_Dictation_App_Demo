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
extern list<char *> _links, _wlinks;
	// billing lists
extern list<char *> _bill_hpi, _bill_ros, _bill_pfsh, _bill_exam;
extern int _max_exam_level;
	// vital signs -- one item per category
extern char *_VS_p, *_VS_r, *_VS_sbp, *_VS_dbp, *_VS_t;
	// vital sign values
extern int _VVS_p, _VVS_r, _VVS_sbp, _VVS_dbp;
extern float _VVS_t;
	// possible information on differential diagnosis
extern char *_differential;

char *scopy(const char *);
void D_clearWarnings(void);
void D_addWarning(char *, bool);
void D_removeWarningBox(void);
