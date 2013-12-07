//#include "stdafx.h"
/************************************
一个socket类，工作在linux,和win32平台下(作者：king(king@txsms.com));
版本：v0.12 build 20020718
***********************************/
#include <fcntl.h>
#include <stdio.h>
#include <errno.h>
#include "mysocket.h"
#include "malloc_debug.h"
#ifndef _WIN32
#define BSD_COMP
#include <sys/ioctl.h>
#endif

KDnsCache m_dns_cache;
KMutex m_make_ip_lock;
#define UDP_PACKAGE_SIZE		4068
#ifdef USE_UDP_E
typedef struct 
{ 
	unsigned long sequence;
	unsigned int len;
	char buf[UDP_PACKAGE_SIZE];
} UDP_E_PACKAGE;
const static int package_head_size=sizeof(UDP_E_PACKAGE)-UDP_PACKAGE_SIZE;
#endif
void make_ip(unsigned long ip,char *ips,bool mask)
{

	struct in_addr s;
	memset(ips,0,18);
	ips[0]='/';
	int skip=1;
	if(!mask)
		skip=0;
	s.s_addr=ip;
	if(ip==0)
		strcpy(ips,"*");
	else if(ip==~0)
		ips[0]=0;
	else if(ip==1)
		strcpy(ips,"localhost");
	else{	
		m_make_ip_lock.Lock();
		strncpy(ips+skip,inet_ntoa(s),16);
		m_make_ip_lock.Unlock();
	}
}
void init_socket()
{
	#ifdef _WIN32
	WORD wVersionRequested;
	WSADATA wsaData;
	int err; 
	wVersionRequested = MAKEWORD( 1, 1 ); 
	err = WSAStartup(wVersionRequested, &wsaData );
	#endif
}
void clean_socket()
{
#ifdef _WIN32
	WSACleanup();
#endif
}
int setnoblock(int sockfd)
{
	int	iMode=1;
#ifdef _WIN32
	ioctlsocket(sockfd, FIONBIO, (u_long FAR*)&iMode);
#else
	ioctl(sockfd, FIONBIO, &iMode);
#endif
	return 1;
}
int setblock(int sockfd)
{
	int	iMode=0;
#ifdef _WIN32
	ioctlsocket(sockfd, FIONBIO, (u_long FAR*) &iMode);
#else
	ioctl(sockfd, FIONBIO, &iMode);
#endif
	return 1;
}
int connect(int sockfd, const struct sockaddr *serv_addr,socklen_t addrlen,int tmo)
{
//#ifndef _WIN32
	struct timeval	tv;
	fd_set	wset;
	int ret;
	if(tmo==0)
		return connect(sockfd,serv_addr,addrlen);

/*	state=fcntl(sockfd, F_GETFL, 0);
	fcntl(sockfd, F_SETFL, state|O_NONBLOCK );
*/
	setnoblock(sockfd);
	connect(sockfd,serv_addr,addrlen);	
	tv.tv_sec=tmo;
	tv.tv_usec=0;
	FD_ZERO(&wset);
	FD_SET(sockfd,&wset);
	ret = select(sockfd+1,NULL,&wset,NULL,&tv);
//	fcntl(sockfd, F_SETFL, state );
	setblock(sockfd);
	if(ret<=0)
		return -1;
	return 0;
/*#else
	return connect(sockfd,serv_addr,addrlen);
#endif
*/
};
mysocket::mysocket()
{

	new_socket=-1;
	old_socket=-1;
//	uid=0;
	m_protocol=TCP;
	frag=send_size=recv_size=0;
	time_value=0;
	memset(&addr,0,sizeof(addr));
	#ifdef USE_SSL
	SSL_load_error_strings();
    	SSLeay_add_ssl_algorithms();
	sslContext =SSL_CTX_new(SSLv3_method());
	if(sslContext==NULL)
		fprintf(stderr,"ssl_ctx_new function error\n");
	#endif
#ifdef USE_UDP_E
	last_sequence=1;
#endif
	
}
mysocket::mysocket(int socket_type)
{

	new_socket=-1;
	old_socket=-1;
//	uid=0;
	time_value=0;
	if(socket_type==UDP)
		m_protocol=UDP;
	else
		m_protocol=TCP;
	frag=send_size=recv_size=0;
	memset(&addr,0,sizeof(addr));
#ifdef USE_UDP_E
	last_sequence=1;
#endif
}
mysocket::~mysocket()
{
	close();
}
/** No descriptions */
bool mysocket::connect(unsigned long ip,int port)
{
	addr.sin_addr.s_addr=ip;
	addr.sin_family=AF_INET;
	addr.sin_port=htons(port);
	switch(m_protocol){
	case TCP:
		if(new_socket>0)
			close();
		if((new_socket=socket(AF_INET,SOCK_STREAM,0))<0){
				new_socket=-1;
				return false;

		}
	//	printf("now try to connect %x:%d.\n",ip,port);
		if(::connect(new_socket,(struct sockaddr *)(&addr),sizeof(struct sockaddr),time_value)<0){
				new_socket=-1;
				return false;
		}
		break;
	case UDP:
		if(new_socket<=0){
			if((new_socket=socket(AF_INET,SOCK_DGRAM,0))<0){
				new_socket=-1;
				return false;
			}
		}
		break;
	}

	return true;

}
int mysocket::connect(struct sockaddr_in *server_addr)
{
	close();
	 if((new_socket=socket(AF_INET,SOCK_STREAM,0))<0){
			new_socket=-1;
			return 0;
	  }
	memcpy(&addr,server_addr,sizeof(addr));
	if(::connect(new_socket,(struct sockaddr *)(&addr),sizeof(struct sockaddr),time_value)<0)
		return 0;
	return 1;
}
int mysocket::connect(const char *host,int port,int host_type,int protocol)
{
	struct hostent	he_b;
	char		he_strb[2048];
	int		he_errno;
	int		rc = 0;
	struct hostent	*he_x;
	int work_type=m_protocol;

	if(protocol!=-1)
		work_type=protocol;
	switch(work_type){
		case TCP_MODEL:
		  close();
		  if(new_socket<=0)
		  if((new_socket=socket(AF_INET,SOCK_STREAM,0))<0){
				new_socket=-1;
				return 0;

		  }
		  addr.sin_family=AF_INET;
		  addr.sin_port=htons(port);
#ifdef USEDNSCACHE
		addr.sin_addr.s_addr=m_dns_cache.GetName(host);
		if(addr.sin_addr.s_addr==0)
			return 0;
#else
		  if(host_type==ADDR_NAME){
		
#ifdef WIN32
					if((h=gethostbyname(host))==NULL){
						::close2(new_socket);
						new_socket=-1;
						return 0;
					}
				//	memcpy(&addr.sin_addr,h->h_addr,MIN(h->h_length,sizeof(addr.sin_addr)));
				addr.sin_addr=*((struct in_addr *)h->h_addr);

#else
				rc = gethostbyname_r(host, &he_b, he_strb, sizeof(he_strb),
				&he_x,
				&he_errno);
				if(rc!=0){
					::close2(new_socket);
					new_socket=-1;
					return 0;
				}
				addr.sin_addr=*((struct in_addr *)he_b.h_addr);
#endif
			}else if(host_type==ADDR_IP)
				addr.sin_addr.s_addr=inet_addr(host);
#endif
			
		  memset(&(addr.sin_zero),0,8);
		  if(::connect(new_socket,(struct sockaddr *)(&addr),sizeof(struct sockaddr),time_value)<0){
				 //::close2(new_socket);
				 //new_socket=-1;
				return 0;
		  }
		
		 // printf("socket=%d\n",new_socket);

		
	
		break;
	case UDP_MODEL:
	case UDP_MODEL_E:
		if(new_socket<=0)
			if((new_socket=socket(AF_INET,SOCK_DGRAM,0))<0)
				return 0;
		addr.sin_family=AF_INET;
		addr.sin_port=htons(port);
#ifdef USEDNSCACHE
		addr.sin_addr.s_addr=m_dns_cache.GetName(host);
		if(addr.sin_addr.s_addr==0)
			return 0;
#else
#ifdef _WIN32
		if((h=gethostbyname(host))==NULL){
			::close2(new_socket);
			new_socket=-1;
		return 0;
		}
		addr.sin_addr=*((struct in_addr *)h->h_addr);
#else
		rc = gethostbyname_r(host, &he_b, he_strb, sizeof(he_strb),
			&he_x,
			&he_errno);
		if(rc!=0){
			::close2(new_socket);
			new_socket=-1;
			return 0;
		}
		addr.sin_addr=*((struct in_addr *)he_b.h_addr);
#endif
#endif
		//  addr_addr.sin_addr.s_addr=inet_addr(host);
		memset(&(addr.sin_zero),0,8);
		break;
	case SSL_MODEL:
		if(connect(host,port,host_type,TCP)<=0)
			return -1;
		#ifdef USE_SSL
		return sslutil_connect();
		#endif
		break;
	default:
		return -6;//protocol is error
	}
	return 1;
}
void mysocket::create(int socket_id)
{
	m_protocol=TCP_MODEL;
	new_socket=socket_id;


//	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(addr);
	::getpeername(new_socket,(struct sockaddr *)&addr,&addr_len);
//	return ntohs(s_sockaddr.sin_port);

}
int mysocket::close()
{
	int ret=new_socket;
	if(new_socket>0){
		ret=::close2(new_socket);
		new_socket=-1;
	}
	if(old_socket>0){		
		::close2(old_socket);
		old_socket=-1;
	}
	return ret;
}
int mysocket::shutdown(int howto)
{
	int  ret;
	 ret=::shutdown(new_socket,howto);
	// new_socket=-1;
	 return ret;
}

int mysocket::send(const char *str)
{
	if(str==NULL)
		return 0;
	return send(str,strlen(str));
}
int mysocket::send(const char *str,int len)
{	
	int length=len;
	int have_send,left_send=len;
	if(closed())
		return -1;
	switch(m_protocol){
		case TCP_MODEL:
			while(left_send>0){
				have_send=send_t(str,left_send);//,0);
				if(have_send<=0)
					return -1;
				left_send-=have_send;
				str+=have_send;
			}
			break;
		case UDP_MODEL:
			length=::sendto(new_socket,str,len,0,(struct sockaddr *)&addr,sizeof(addr));
			break;
		case SSL_MODEL:
			#ifdef USE_SSL
			length = SSL_write(ssl, str, len);
			#endif
		//	printf("use ssl model write,write %d length\n",length);
			break;
		#ifdef USE_UDP_E
		case UDP_MODEL_E:
		//	printf("use udp_model_e to send\n");
			return send_udp_model_e(str,len);
			break;
		#endif
		default:
			return -1;
	}

	if(length>0)
		send_size+=len;
	return length;
}
#ifdef USE_UDP_E
int mysocket::send_udp_model_e(const char * str,int len)
{
	UDP_E_PACKAGE m_package;
	UDP_E_PACKAGE recv_ack;
	socklen_t addr_len=sizeof(addr);
	const int retry_count=3;
	int current_len;
	int point=0;
	int i;
	memset(&m_package,0,sizeof(UDP_E_PACKAGE));
	memset(&recv_ack,0,sizeof(UDP_E_PACKAGE));
	while(point<len){
		if(len>=point+UDP_PACKAGE_SIZE)
			current_len=UDP_PACKAGE_SIZE;
		else
			current_len=len-point;
		memcpy(&m_package.buf,str+point,current_len);
		m_package.len=current_len;
		m_package.sequence=last_sequence;
		last_sequence++;
		for(i=0;i<retry_count;i++){//重试10次
			::sendto(new_socket,(char *)&m_package,m_package.len+package_head_size,0,(struct sockaddr *)&addr,sizeof(addr));
			if(recv_t((char *)&recv_ack,sizeof(recv_ack),1,0,UDP_MODEL)>0){//接收返回确定码
		//		printf("recv answer\n");
			//	printf("recv sequence=%d,send sequence=%d\n",recv_ack.sequence,m_package.sequence);
				if(recv_ack.sequence==m_package.sequence){//如果序号相同
			//		printf("answer sequence is right\n");
					recv_ack.buf[recv_ack.len]=0;
					if(strcmp(recv_ack.buf,"ACK")==0){//如果是返回码，则发送成功
				//		printf("recv the right ack\n");
						goto one_package_done;
					}
				}
			}
		//	printf("send failed resend now\n");
		}
		if(i==retry_count){//发送失败
		//	printf("send failed\n");
			return point;
		}
	one_package_done:
		point+=current_len;
	}
	return point;
}
int mysocket::recv_udp_model_e(char * buf,int len)
{
	UDP_E_PACKAGE ack;
	UDP_E_PACKAGE recv_package;
	socklen_t addr_len=sizeof(addr);	
	unsigned int recv_len;
	if(len<=0)
		return len;
	memset(&ack,0,sizeof(UDP_E_PACKAGE));
	memset(&recv_package,0,sizeof(UDP_E_PACKAGE));
	if(len>sizeof(UDP_E_PACKAGE))
		recv_len=UDP_PACKAGE_SIZE;
	else
		recv_len=len;	
	ack.len=3;
	strcpy(ack.buf,"ACK");
	for(;;){
		if(recvfrom(new_socket,(char *)&recv_package,recv_len,0,(struct sockaddr *)&addr,&addr_len)>0){
		//	printf("recv package from %s\n",get_client_name());
			ack.sequence=recv_package.sequence;
			if(::sendto(new_socket,(char *)&ack,ack.len+package_head_size,0,(struct sockaddr *)&addr,sizeof(addr))<=0)
				return -1;
			if(client_buffer.is_use(get_client_name(),ack.sequence)==0){
				if(recv_package.len<=len){
					memcpy(buf,recv_package.buf,recv_package.len);
				//	printf("recv_package len=%d\n",recv_package.len);
					return recv_package.len;
				}
			}			
		}else
			return -1;
	}
}
#endif
int mysocket::send_t(const char *buf,int len,int time_out)
{
	fd_set readfds;
	struct timeval t_timeout;
	memset(&t_timeout,0,sizeof(t_timeout));
	socklen_t addr_len=sizeof(addr);
	if(time_out==-1)
		time_out=time_value;
	if(time_out==0)
		goto message_recv;	
	t_timeout.tv_sec=time_out;
	FD_ZERO(&readfds);
	FD_SET(new_socket,&readfds);
	if(select(new_socket+1,NULL,&readfds,NULL,&t_timeout)<=0)
		return -2;	
message_recv:
	switch(m_protocol){
		case TCP_MODEL:
			return ::send(new_socket,buf,len,0);
		case UDP_MODEL:
                       return ::sendto(new_socket,buf,len,0,(struct sockaddr *)&addr,sizeof(addr));
		default:
			return -1;
	}

}
int mysocket::recv_t(char * buf,int len,int m_times)
{
	
	fd_set readfds;
	struct timeval t_timeout;
	socklen_t addr_len=sizeof(addr);
/*	if(protocol==-1)
		protocol=m_protocol;
*/	if(m_times==-1)
		m_times=time_value;
	if(m_times==0)
		goto message_recv;	
	t_timeout.tv_sec=m_times;
	t_timeout.tv_usec=0;
	FD_ZERO(&readfds);
	FD_SET(new_socket,&readfds);
	if(select(new_socket+1,&readfds,NULL,NULL,&t_timeout)<=0)
		return -2;	
message_recv:
	switch(m_protocol){
		case TCP_MODEL:
			return ::recv(new_socket,buf,len,0);
		case UDP_MODEL:
			return recvfrom(new_socket,buf,len,0,(struct sockaddr *)&addr,&addr_len);
#ifdef USE_UDP_E
		case UDP_MODEL_E:
			return recv_udp_model_e(buf,len);
#endif
		default:
			return -1;
	}

}
int mysocket::recv(char *str,int len,const char * end_str)
{
	int	 remaining = len;
	int length=0;
	char * buffer=str;
	int max_len=len;
	if(end_str!=NULL){
		memset(str,0,len);
		remaining=len-1;
		max_len=len-1;
		len=1;
	}
	if(closed())
		return -1;
	while(remaining > 0){
		length=recv_t(str,len);
		if(length<=0)
			return length;
		remaining -= length;
		if( (end_str==NULL)|| (strstr(buffer,end_str)!=NULL) )
			break;		
		str += length;		
	}
	length=max_len-remaining;	
	if(length>0)
		recv_size+=length;
	if(end_str!=NULL){
		buffer[length-strlen(end_str)]=0;
	}
	return length;

	/**********************************************
	以下是原来版本的recv代码
	**********************************************
	
	int	 remaining = len;
	int length=0;
	char * buffer=str;
	if(closed())
		return -1;
	socklen_t addr_len=sizeof(addr);

	switch(m_protocol){
		case TCP_MODEL:
			while(remaining > 0){
				length=::recv(new_socket,str,len,0);
				remaining -= length;
				if( (end_str==NULL) || (length<=0) || (strstr(buffer,end_str)!=NULL) )
					goto clean;
				str += length;
				
			}
			break;
		case UDP_MODEL:	
			while(remaining > 0){
				length=::recvfrom(new_socket,str,len,0,(struct sockaddr *)&addr,&addr_len);
				remaining -= length;
				if( (end_str==NULL) || (length<=0) || (strstr(buffer,end_str)!=NULL) )
					goto clean;
				str += length;
				
			}			
			break;
		case SSL_MODEL:
			#ifdef USE_SSL
			while(remaining > 0){
				length = SSL_read(ssl, str, len);
				remaining -= length;
				if( (end_str==NULL) || (length<=0) || (strstr(buffer,end_str)!=NULL) )
					goto clean;
				str += length;
			}
			#endif
			//printf("use ssl model read,read %d length\n",length);
			break;
		default:
			return -1;
	}
	//frag++;
clean:
	length=len-remaining;
	if(length>0)
		recv_size+=length;
	return length;
	***********************************************/
}
void mysocket::clear_recvq(int len)
{
	if(len<=0)
		return;
	char *str=(char *)malloc(len);
	setnoblock(new_socket);
	::recv(new_socket,str,len,0);
	setblock(new_socket);
	free(str);
}
int mysocket::open(int port,const char * ip)
{
	int n=1;
	if(m_protocol==TCP){
		if(old_socket<=0)
		if((old_socket=socket(AF_INET,SOCK_STREAM,0))<0)
			return -1;
		memset(&addr,0,sizeof(addr));
		addr.sin_family=AF_INET;
		addr.sin_port=htons(port);
		if(ip==NULL)
			addr.sin_addr.s_addr=htonl(0);
		else
			addr.sin_addr.s_addr=inet_addr(ip);
#ifndef _WIN32
		setsockopt(old_socket,SOL_SOCKET,SO_REUSEADDR,(const char *)&n,sizeof(int)); 
#endif
	//	if(port!=0)
			if(::bind(old_socket,(struct sockaddr *) &addr,sizeof(struct sockaddr))<0){
				::close2(old_socket);
				old_socket=-1;
				return -2;
			}
		if(::listen(old_socket,5)<0){
			::close2(old_socket);
			old_socket=-1;
			return -3;
		}
		return 1;
	}else if( m_protocol==UDP_MODEL || m_protocol==UDP_MODEL_E ){
		if(port!=0){
			addr.sin_addr.s_addr=INADDR_ANY;
			addr.sin_family=AF_INET;
			addr.sin_port=htons(port);
			if(new_socket<=0)
			if((new_socket=socket(AF_INET,SOCK_DGRAM,0))==-1)
				return -1;
			
			if(::bind(new_socket,(struct sockaddr *) &addr,sizeof(struct sockaddr))<0){
				::close2(new_socket);
				new_socket=-1;
				return -2;
			}
		}else{
			connect("127.0.0.1",0,ADDR_IP);
			send("test");
		}
		
		return 1;
	}else if(m_protocol==SSL_MODEL){

	}
	return -4;
}

int mysocket::bind(int port)
{
	if(new_socket<=0)
	if((new_socket=socket(AF_INET,SOCK_STREAM,0))<0)
		return -1;
	memset(&addr,0,sizeof(addr));
	addr.sin_family=AF_INET;
	addr.sin_port=htons(port);
	addr.sin_addr.s_addr=htonl(0);
	if(::bind(new_socket,(struct sockaddr *) &addr,sizeof(struct sockaddr))<0){
		::close2(new_socket);
		new_socket=-1;
		return -2;
	}
	return 1;
}
int mysocket::accept()
{
	struct timeval tm;
	memset(&tm,'0',sizeof(tm));
	tm.tv_sec=3600;//one hours
	if(m_protocol==UDP)
		return -3;
	socklen_t sin_size=sizeof(struct sockaddr);
//	struct linger m_linger;
	new_socket=::accept(old_socket,(struct sockaddr *)&addr,&sin_size);
	/*
	setsockopt(new_socket,SOL_SOCKET,SO_RCVTIMEO,(void *)&tm,sizeof(tm));
	setsockopt(new_socket,SOL_SOCKET,SO_RCVTIMEO,(void *)&tm,sizeof(tm));
	*/
	return new_socket;
}
void mysocket::use(int socket)
{
	if(socket==OLD){
  		::close2(new_socket);
		new_socket=-1;
	}else{
		::close2(old_socket);
		old_socket=-1;
	}
}
const char * mysocket::get_addr()
{
//	strncpy(client_ip,inet_ntoa(addr.sin_addr),16);
	make_ip(addr.sin_addr.s_addr,client_ip);
	return client_ip;

}
const char * mysocket::getsockname()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(new_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	make_ip(s_sockaddr.sin_addr.s_addr,client_ip);
	return client_ip;
//	return inet_ntoa(s_sockaddr.sin_addr);
}
int mysocket::get_protocol()
{
	return m_protocol;
}
void mysocket::set_protocol(int protocol)
{
	m_protocol=protocol;

}
int mysocket::closed()
{
	if(new_socket<=0)
		return 1;
	else
		return 0;
}

/** No descriptions */
int mysocket::get_port(int sockfd)
{
	struct sockaddr_in s_sockaddr;
	if(sockfd==NEW)
		return ntohs(addr.sin_port);
	
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(old_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	return ntohs(s_sockaddr.sin_port);
}

int mysocket::get_socket(int socket)
{
	if(socket==NEW)
		return new_socket;
	else
		return old_socket;
}
/*
int mysocket::get_socket(int socket)
{

		return new_socket;
}*/
mysocket * mysocket::clone()
{
	mysocket * tmp=new mysocket;
	tmp->new_socket=new_socket;
	memcpy(&tmp->addr,&addr,sizeof(addr));
//	memcpy((void *)tmp,(void *)this,sizeof(mysocket));
	tmp->old_socket=-1;
	return tmp;
}
/*
int mysocket::set_opt(int name,const void *val,socklen_t *len,int socket)
{
	if((socket==NEW)||(socket==-1))
		setsockopt(new_socket,SOL_SOCKET,name,val,len);
	if((socket==OLD)||(socket==-1))
		setsockopt(old_socket,SOL_SOCKET,name,val,len);
	return 1;
}
*/
#ifdef USE_SSL
int mysocket::sslutil_accept()
{
	int     err;
   if((ssl = SSL_new(sslContext)) == NULL){
        err = ERR_get_error();
        fprintf(stderr, "SSL: Error allocating handle: %s\n", ERR_error_string(err, NULL));
        return -1;
    }
    SSL_set_fd(ssl, new_socket);
    if(SSL_accept(ssl) <= 0){
        err = ERR_get_error();
        fprintf(stderr, "SSL: Error accepting on socket: %s\n", ERR_error_string(err, NULL));
        return -1;
    }
    fprintf(stderr, "SSL: negotiated cipher: %s\n", SSL_get_cipher(ssl));
	//d->ssl = ssl;
    return 1;
}
int mysocket::sslutil_connect()
{
	int     err;
	if((ssl = SSL_new(sslContext)) == NULL){
        err = ERR_get_error();
        fprintf(stderr, "SSL: Error allocating handle: %s\n", ERR_error_string(err, NULL));
        return -1;
    }
    SSL_set_fd(ssl, new_socket);
    if(SSL_connect(ssl) <= 0){
        err = ERR_get_error();
        fprintf(stderr, "SSL: Error conencting socket: %s\n", ERR_error_string(err, NULL));
        return -1;
    }
    fprintf(stderr, "SSL: negotiated cipher: %s\n", SSL_get_cipher(ssl));
	
    return 1;
}
#endif

int mysocket::get_client_port()
{
	return ntohs(addr.sin_port);
}
int mysocket::get_self_port()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(new_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	return ntohs(s_sockaddr.sin_port);

}
int mysocket::get_remote_port()
{
	return ntohs(addr.sin_port);
}

const char * mysocket::get_client_name()
{
//	strncpy(client_ip,inet_ntoa(addr.sin_addr),16);
	make_ip(addr.sin_addr.s_addr,client_ip);
	return client_ip;

}
unsigned long mysocket::get_client_addr()
{
	return addr.sin_addr.s_addr;
}
unsigned long mysocket::get_self_addr()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(new_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	return s_sockaddr.sin_addr.s_addr;

}
unsigned long mysocket::get_remote_addr()
{
	return addr.sin_addr.s_addr;
}
int mysocket::get_server_port()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(new_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	return ntohs(s_sockaddr.sin_port);
}
const char * mysocket::get_server_name()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(new_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	make_ip(s_sockaddr.sin_addr.s_addr,client_ip);
	return client_ip;
//	return inet_ntoa(s_sockaddr.sin_addr);
}
int mysocket::get_listen_port()
{
	struct sockaddr_in s_sockaddr;
	socklen_t addr_len=sizeof(s_sockaddr);
	::getsockname(old_socket,(struct sockaddr *)&s_sockaddr,&addr_len);
	return ntohs(s_sockaddr.sin_port);
}
unsigned mysocket::set_time(unsigned second)
{
	unsigned old_time=time_value;
	time_value=second;
	return old_time;
}
unsigned mysocket::get_time()
{
	return time_value;
}
int mysocket::operator<<(const char *str)
{
	return send(str);
}
int mysocket::operator<<(int value)
{
	return send(value);
}

int mysocket::send(int value)
{
	char tmp[16];
	memset(tmp,0,sizeof(tmp));
	int len=snprintf(tmp,sizeof(tmp)-1,"%d",value);
	return send(tmp,len);
}
struct sockaddr_in * mysocket::get_sockaddr()
{
	return &addr;
}


#ifdef USE_UDP_E
/*************************************************************************************************************/
UDPBuffer::UDPBuffer(int BufferNum)
{
	int i;
	struct SUDPBuffer *tmp=NULL;
	struct SUDPBuffer *tmp_prev=NULL;
	head=NULL;
	last_use=0;
	for(i=0;i<BufferNum;i++){
		tmp=new SUDPBuffer;		
		if(tmp==NULL)
			break;
		tmp->next=NULL;
		strcpy(tmp->ip,"NOTUSE");
		tmp->sequence=0;
		tmp->last_use=0;
		if(i==0){
			head=tmp;
			tmp_prev=head;
		}else{
			tmp_prev->next=tmp;
			tmp_prev=tmp;
		}
	}
	
}
UDPBuffer::~UDPBuffer()
{
	return;
	struct SUDPBuffer *tmp=head;
	if(head==NULL)
		return;
	while(head!=NULL){
		tmp=head->next;
		delete head;
		head=tmp;
	}
//	delete head;	
}
int UDPBuffer::is_use(const char *ip, unsigned int sequence)
{
	short IsIpIn=0;
	struct SUDPBuffer *hope,*tmp;
	last_use++;
	tmp=head;
	hope=head;
//	char buffer[16];
	if(head==NULL)
		return 1;
	do{
	//	AfxMessageBox(tmp->ip);
		if(tmp->last_use<hope->last_use)
			hope=tmp;
		if(strcmp(tmp->ip,ip)==0){
			IsIpIn=1;
			break;
		}
	}while((tmp=tmp->next)!=NULL);
//	sprintf(buffer,"%d",sequence);
	if(IsIpIn)
		hope=tmp;
//	AfxMessageBox(sequence);
	if(((IsIpIn==1)&&(sequence!=hope->sequence))||(IsIpIn==0)){
			hope->last_use=last_use;
			strncpy(hope->ip,ip,16);
			hope->sequence=sequence;
		//TRACE1("last_use=%d,ip=%s,sequence=%d",now_time,ip,sequence);
//		AfxMessageBox("test");
			return 0;
		}
	return 1;

}
#endif
