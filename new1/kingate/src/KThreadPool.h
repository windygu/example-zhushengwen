#ifndef ThreadPool_H_skdfjsldfjaldskfjlsdkfj
#define ThreadPool_H_skdfjsldfjaldskfjlsdkfj
#ifndef _WIN32
#include<pthread.h>
#endif

#include<list>
#include<map>

#include "utils.h"
#include "KMutex.h"


#define KINGATE_THREAD_NULL 0
#define KINGATE_THREAD_START 1
#define  KINGATE_THREAD_END 2
#define KINGATE_NOTICE_THREAD SIGUSR2

struct ThreadInfo
{
	void *param;
	int cmd;
	pthread_t pid;
	int end_time;
#ifdef _WIN32
	HANDLE hThreadNotice; 
#endif
};
typedef std::list<ThreadInfo *> ThreadInfoList;
typedef std::map<int,int> intmap;
class KThreadPool
{
public:
	KThreadPool();
	~KThreadPool();
	void Flush();
	int Start(SERVER *param);
	int getFreeThread();
	void closeAllFreeThread();
private:
#ifndef _WIN32
	pthread_attr_t attr;


#endif
	pthread_t id;

};

extern int total_thread;
int set_max_per_ip(int value);

#endif
