// common definitions between the dashboard and the SLC


#define STATUS_PATH "C:/TEMP/Sullivan/status.txt"
#define DASHBOARD_PATH "C:/TEMP/Sullivan/dashboard.rtf"
#define WARN_PATH "C:/TEMP/Sullivan/dashwarn.rtf"

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
bool Validate(void);
void S_generateWarn(void);

// globals in the SLC
extern FILE *status_file;




