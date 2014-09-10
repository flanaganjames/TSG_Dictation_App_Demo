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
	// possible information on differential diagnosis
extern char *differential;