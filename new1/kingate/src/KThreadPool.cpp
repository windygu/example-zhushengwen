#ifndef _WIN32
#include <sys/types.h>
#include <unistd.h>
#include <signal.h>
#include <syslog.h>
#endif
#include <time.h>
#include <assert.h>
#include "KThreadPool.h"
#include "do_config.h"

#include "log.h"
#include "malloc_debug.h"

static const int max_sleep_time=300;
static ThreadInfoList FreeThread;
static intmap m_ip;
static KMutex m_ThreadPoolLock;
#ifndef _WIN32
static sigset_t m_blockset;
static sigset_t m_all;

void recv_notice_thread_ignore(int sig)
{
	assert(sig==KINGATE_NOTICE_THREAD);
	signal(sig,recv_notice_thread_ignore);	
}
#endif
int set_max_per_ip(int value)
{
	int old_value=conf.max_per_ip;
	m_ThreadPoolLock.Lock();
	conf.max_per_ip=value;
        intmap::iterator it2;
	for(it2=m_ip.begin();it2!=m_ip.end();){
#ifndef _WIN32
            m_ip.erase(it2);
			it2++;
#else
	        it2=m_ip.erase(it2);
#endif
	}
	m_ThreadPoolLock.Unlock();
	return old_value;
}	
FUNC_TYPE FUNC_CALL run_thread(void *param)
{
	ThreadInfo *m_thread=new ThreadInfo;
	intmap::iterator it2;
	unsigned long ip;
	if(m_thread==NULL){
		return NULL;
	}
	m_thread->pid=pthread_self();
#ifdef _WIN32
	m_thread->hThreadNotice=CreateEvent(NULL,FALSE,FALSE,NULL);
#endif
	ip=(int)server_thread(param);
	for(;;){
		m_thread->end_time=time(NULL);
		m_thread->cmd=KINGATE_THREAD_NULL;

#ifndef _WIN32
		pthread_sigmask(SIG_BLOCK,&m_blockset,NULL);//阻塞信号
#endif
		m_ThreadPoolLock.Lock();
		total_thread--;
		if(conf.max_per_ip>0){
			it2=m_ip.find(ip);
			if(it2!=m_ip.end()){//it is a bug
				if((*it2).second<2){
					m_ip.erase(it2);
				}else{
					(*it2).second--;
				}
			}
		}
		FreeThread.push_front(m_thread);
		m_ThreadPoolLock.Unlock();
#ifdef _WIN32
		WaitForSingleObject(m_thread->hThreadNotice,-1);
#else
		sigsuspend(&m_all);
		pthread_sigmask(SIG_UNBLOCK,&m_blockset,NULL);
#endif
		if(m_thread->cmd==KINGATE_THREAD_START){
			ip=(int)server_thread(m_thread->param);
		}else if(m_thread->cmd==KINGATE_THREAD_END){
#ifdef _WIN32
			CloseHandle(m_thread->hThreadNotice);
#endif
			delete m_thread;
			return NULL;
		}else{			
			klog(ERR_LOG,"kingate closed or bug!!in %s:%d(recv awake signal),my pthread id=%d\n",__FILE__,__LINE__,pthread_self());
			delete m_thread;
			return NULL;
		}
	}
	
}
KThreadPool::KThreadPool()
{
#ifndef _WIN32
	pthread_attr_init(&attr);
	pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);//设置线程为分离
	signal(KINGATE_NOTICE_THREAD,recv_notice_thread_ignore);
	sigemptyset(&m_blockset);
	sigaddset(&m_blockset,KINGATE_NOTICE_THREAD);
	sigemptyset(&m_all);
#endif
}
KThreadPool::~KThreadPool()
{
}
void KThreadPool::closeAllFreeThread()
{
	ThreadInfoList::iterator it;
	pthread_t pid;//
	m_ThreadPoolLock.Lock();
	for(it=FreeThread.end();it!=FreeThread.begin();){
		it--;
		pid=(*it)->pid;
		(*it)->cmd=KINGATE_THREAD_END;			
#ifndef _WIN32
		if(pthread_kill(pid,KINGATE_NOTICE_THREAD)!=0){
			klog(ERR_LOG,"cann't send signal to thread(command:close,id=%d)\n",pid);
		}
#else
		SetEvent((*it)->hThreadNotice);
#endif
		it=FreeThread.erase(it);
	}
	m_ThreadPoolLock.Unlock();
	

}
void KThreadPool::Flush()
{
	ThreadInfoList::iterator it;
	int now_time=time(NULL);
	pthread_t pid;//
	m_ThreadPoolLock.Lock();
	for(it=FreeThread.end();it!=FreeThread.begin();){
		if(FreeThread.size()<=conf.min_free_thread)
			break;
		it--;
		if(now_time-(*it)->end_time>max_sleep_time){
			pid=(*it)->pid;
			(*it)->cmd=KINGATE_THREAD_END;			
#ifndef _WIN32
			if(pthread_kill(pid,KINGATE_NOTICE_THREAD)!=0){
				klog(ERR_LOG,"cann't send signal to thread(command:close,id=%d)\n",pid);
			}
#else
			SetEvent((*it)->hThreadNotice);
#endif
			it=FreeThread.erase(it);
		}else{
			break;
		}
	}
	m_ThreadPoolLock.Unlock();
	
}
int KThreadPool::getFreeThread()
{
	int FreeThreadCount=0;
	m_ThreadPoolLock.Lock();
	FreeThreadCount=FreeThread.size();
	m_ThreadPoolLock.Unlock();
	return FreeThreadCount;
}
int KThreadPool::Start(SERVER *param)
{
	ThreadInfoList::iterator it;
	unsigned long ip=param->server->get_client_addr();
	intmap::iterator it2;
	int ret=0;
	m_ThreadPoolLock.Lock();	
	if(conf.max_per_ip>0){
		it2=m_ip.find(ip);
		if(it2==m_ip.end()){
			m_ip[ip]=1;
		}else{
			if((*it2).second>=conf.max_per_ip){
#ifndef _WIN32
				syslog(LOG_ERR,"error,refuse %s try to connect %d ,max_per_ip(%d) limit.\n",param->server->get_client_name(),param->model,conf.max_per_ip);
#endif
				goto err;
			}
			(*it2).second++;
		}
	}
	it=FreeThread.begin();
	if(it==FreeThread.end()){//It is no free Thread now		
		ret=pthread_create(&id,&attr,run_thread,param);//
#ifndef _WIN32
		if(ret!=0){
			klog(ERR_LOG,"create thread error.result=%d\n",ret);
			goto err;

		}
#endif
	}else{
		(*it)->cmd=KINGATE_THREAD_START;
		(*it)->param=(void *)param;
#ifndef _WIN32
		if(pthread_kill((*it)->pid,KINGATE_NOTICE_THREAD)!=0){
			klog(ERR_LOG,"cann't send signal to thread(command:start,id=%d)\n",(*it)->pid);
		}
#else
		SetEvent((*it)->hThreadNotice);
#endif
		FreeThread.erase(it);
	}
	total_thread++;
	m_ThreadPoolLock.Unlock();
	return 1;
err:
	m_ThreadPoolLock.Unlock();
	return 0;
}
