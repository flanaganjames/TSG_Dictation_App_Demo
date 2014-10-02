// common definitions between the dashboard and the SLC


#define STATUS_PATH "status.txt"
#define DASHBOARD_PATH "dashboard.rtf"
#define WARN_PATH "dashwarn.rtf"

// prototypes in SLC
int S_initStatus(void);
int S_addStatus(int, _TCHAR**);
int S_multiStatus(int, _TCHAR**);
int S_finishStatus(void);
void S_parseStatus(void);
void S_sortStatus(void);
void S_generateDash(void);
void S_reset(void);
bool no_complaint(void);
void S_Validate(void);
void S_generateWarnBox(void);

// globals in the SLC
extern FILE *status_file;





