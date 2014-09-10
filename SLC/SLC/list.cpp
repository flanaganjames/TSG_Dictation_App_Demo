/*
	Class definition for List class
 */

class List {
public:
	List(void);
	~List(void);
	List *Next();
	List *Prev();
private:
	char *tag;
	List *next;
	List *prev;
};


