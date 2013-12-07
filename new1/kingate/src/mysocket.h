/************************************
一个socket类，工作在linux,和win32平台下(作者：king(king@txsms.com));
版本：v0.1 build 20020718
***********************************/

#ifndef MYSOCKET1_H__6249DFB7_FF97_4C74_8833_8DFE68599955__INCLUDED_
#define MYSOCKET1_H__6249DFB7_FF97_4C74_8833_8DFE68599955__INCLUDED_ 
//#define USE_SSL
#pragma comment(lib,"ws2_32.lib")  
#ifdef HAVE_CONFIG_H
#include "config.h"
#endif
#if     !defined(HAVE_SOCKLEN_T)
#if     defined(_AIX41)
typedef         size_t          socklen_t;
#else
typedef         int             socklen_t;
#endif
#endif  /* !HAVE_SOCKLEN_T */

#define USEDNSCACHE
#ifndef MIN
#define MIN(a,b)        (((a)<(b))?(a):(b))
#endif
#ifdef _WIN32 //for win32
	#include <Winsock2.h>
#ifndef bzero
	#define bzero(X,Y)	memset(X,0,Y)
#endif
	#define close2(X)	closesocket(X)
#else	//for linux
	#include<sys/wait.h>
	#include <netinet/in.h>
	#include<sys/socket.h>
	#include<string.h>
	#include<netdb.h>
	#include<arpa/inet.h>
	#include<unistd.h>
	#define close2(X)	close(X)
	#ifdef USE_SSL
	#include <openssl/ssl.h>
	#include <openssl/err.h>
	#endif
	#include <sys/select.h>
	#include <sys/time.h>
	#include <sys/types.h>


#endif
#ifdef USEDNSCACHE
#include "KDnsCache.h"
#endif
#include "KMutex.h"
#define OLD					0
#define NEW					1
#define CHILD				NEW
#define MAIN				OLD

#define TCP					1
#define UDP					2
#define TCP_MODEL			1
#define UDP_MODEL			2
#define UDP_MODEL_E			3		//保证的UDP协议连接
#define SSL_MODEL			4


#define ADDR_IP				0
#define ADDR_NAME			1

#ifdef USE_UDP_E
//单个缓冲区定义
struct SUDPBuffer
{
	char ip[17];//该缓冲区目前使用者
	unsigned long sequence;//该缓冲区目前使用者所用的序列号
	unsigned long last_use;//该缓冲区最后使用时，每用一次就加一
	struct SUDPBuffer *next;//指向下一缓冲区
};
class UDPBuffer  
{
public:
	int is_use(const char *ip,unsigned sequence);//判断该ip及序列号是否已经使用过。
	UDPBuffer(int BufferNum=50);
	~UDPBuffer();
protected:
	struct SUDPBuffer * head;
	unsigned long last_use;
};
#endif

class mysocket  
{
	public: 
		mysocket();
		mysocket(int socket_type);
		~mysocket();
		void create(int socket_id);
		int connect(struct sockaddr_in *server_addr);
		bool connect(unsigned long ip,int port);
		int connect(const char * host,int port,int host_type=ADDR_NAME,int protocol=-1);
	//	int connect(const char * host,int port,int socket_type);
		int send(const char *str);
		int send(const char *str,int len);
		int send(int value);
		int send_t(const char *buf,int len,int time_out=-1);
		int recv(char *str,int len,const char * end_str=NULL);
		void clear_recvq(int len);
		int open(int port=0,const char * ip=NULL);
		int accept();
		int bind(int port);
		void use(int socket);//select to use socket,socket=0 use old socket,socket=1 use new socket.
		unsigned set_time(unsigned second=0);
		unsigned get_time();
		int close();
		int shutdown(int howto);
		int closed();//socket是否关闭,如果是返回1,否则返回0；
		int recv_t(char * buf,int len,int m_times=-1);
#ifdef USE_UDP_E
		int send_udp_model_e(const char * str,int len);
		int recv_udp_model_e(char * buf,int len);
#endif

		int get_client_port();
		int get_server_port();
		int get_listen_port();
		int get_self_port();
		int get_remote_port();

		const char * get_client_name();		
		const char * get_server_name();
		unsigned long get_client_addr();
		unsigned long get_self_addr();
		unsigned long get_remote_addr();

		struct sockaddr_in * get_sockaddr();
		int get_protocol();
		int get_socket(int socket=NEW);
	//	int get_socket();
		void set_protocol(int protocol);
		mysocket * clone();//
		#ifdef USE_SSL
		int sslutil_accept();
		int sslutil_connect();
		#endif
		//旧函数
		const char *get_addr();
		const char *getsockname();
		
		int get_port(int sockfd=NEW);
		int operator<<(const char *str);
		int operator<<(int value);
	public:
		unsigned long frag;
		unsigned long send_size;
		unsigned long recv_size;
		int new_socket,old_socket;
	private:
		void create();
		int m_protocol;
		struct hostent *h;
		struct sockaddr_in addr;
		char client_ip[18];
		unsigned time_value;	
		#ifdef USE_UDP_E
		unsigned long last_sequence;
		UDPBuffer client_buffer;
		#endif
		#ifdef USE_SSL
		SSL_CTX  *sslContext;
		SSL	*ssl;
		#endif
	

};
void init_socket();
void clean_socket();
int connect(int sockfd, const struct sockaddr *serv_addr,socklen_t addrlen,int tmo);
void make_ip(unsigned long ip,char *ips,bool mask=false);
extern KMutex m_make_ip_lock;
#endif 
