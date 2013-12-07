#include <string.h>
#include <assert.h>
#include "do_config.h"
#include "ftp.h"
#include "utils.h"
#include "allow_connect.h"
#include "log.h"
#include "forwin32.h"
#include "KUser.h"
#include "malloc_debug.h"

int get_host_message_from_ftp_str(const char * str,HOST_MESSAGE * m_host,int ftp_cmd)
{
	int tmp[6];
	switch(ftp_cmd){
		case PASV_CMD:
			sscanf(str,"%*[^(](%d,%d,%d,%d,%d,%d)",&tmp[0],&tmp[1],&tmp[2],&tmp[3],&tmp[4],&tmp[5]);
			break;
		case PORT_CMD:
			sscanf(str,"%*[^ ] %d,%d,%d,%d,%d,%d",&tmp[0],&tmp[1],&tmp[2],&tmp[3],&tmp[4],&tmp[5]);
			break;
		default:
			klog(ERR_LOG,"ftp cmd is error\n");
			return -1;//ftp cmd error
	}
	memset(m_host->host,0,sizeof(m_host->host));
	snprintf(m_host->host,sizeof(m_host->host)-1,"%d.%d.%d.%d",tmp[0],tmp[1],tmp[2],tmp[3]);
	m_host->port=tmp[4]*0x100+tmp[5];
	return 1;
}
int rewrite_cmd(SERVER *m_server,char *str,int ftp_data_cmd)
{
	if(m_server->ext!=NULL){
		klog(ERR_LOG,"ftp port or pasv cmd error.Already have another data thread\n");
		return -1;
	}
	FTP_DATA *m_ftp=new FTP_DATA;
	m_ftp->m_server=m_server;
	if(get_host_message_from_ftp_str(str,&m_ftp->remote,ftp_data_cmd)<=0){
		delete m_ftp;
		return -1;
	}
	memset(m_ftp->client_ip,0,sizeof(m_ftp->client_ip));
	if(ftp_data_cmd==PASV_CMD)
		strncpy(m_ftp->client_ip,m_server->server->get_addr(),sizeof(m_ftp->client_ip)-1);
	else
		strncpy(m_ftp->client_ip,m_server->client->get_addr(),sizeof(m_ftp->client_ip)-1);
	
	m_ftp->ftp_data_cmd=ftp_data_cmd;
	m_server->ext=(void *)1;
	ftp_data_proxy(m_server,m_ftp,str);
//clean:
	return strlen(str);

};
FUNC_TYPE FUNC_CALL ftp_data_connection(void *param)
{
	struct timeval tm;
	int ret;	
	memset(&tm,0,sizeof(tm));
	fd_set fds;
	mysocket client;
	FTP_DATA *m_ftp=(FTP_DATA *)param;
	assert(m_ftp);
	FD_ZERO(&fds);
	if(m_ftp->server.old_socket<=0)
		goto clean;
	FD_SET(m_ftp->server.old_socket,&fds);
	tm.tv_sec=20;
	ret=select(m_ftp->server.old_socket+1,&fds,NULL,NULL,&tm);
	if(ret<=0)
		goto clean;
	if(FD_ISSET(m_ftp->server.old_socket,&fds)){
		if(m_ftp->server.accept()<=0)
			goto clean;
		if(strcmp(m_ftp->server.get_addr(),m_ftp->client_ip)!=0){
			//server.use(OLD);
			goto clean;
		}
		client.set_time(conf.time_out[FTP]);
		if(client.connect(m_ftp->remote.host,m_ftp->remote.port)<=0)
			goto clean;
		create_select_pipe(&m_ftp->server,&client,conf.time_out[FTP]);
	#ifndef DISABLE_USER	
		if(m_ftp->ftp_data_cmd==1){//pasv model
			m_user.UpdateSendRecvSize(m_ftp->server.get_client_addr(),m_ftp->server.recv_size,m_ftp->server.send_size);
		}else{
			m_user.UpdateSendRecvSize(client.get_client_addr(),client.recv_size,client.send_size);			
		}
	#endif
	}
clean:
	m_ftp->m_server->ext=NULL;
	delete m_ftp;	
	return NULL;
}

mysocket * create_ftp_connect(SERVER * m_server)
{
	mysocket *server=m_server->server;
	mysocket *client=new mysocket;
	int length,port,len;
	char host[64];
	int malloc_len=4068;
	char *msg=(char *)malloc(malloc_len);
	char *str=(char *)malloc(malloc_len);
	if(msg==NULL || str==NULL){
		klog(ERR_LOG,"no memory to alloc in %s:%d\n",__FILE__,__LINE__);
		goto clean;
	}
	if(conf.ftp_redirect){
		memset(host,0,sizeof(host));
		strncpy(host,conf.ftp_redirect,sizeof(host)-1);
		port=conf.ftp_redirect_port;
		goto check_connect;
	}
	host[0]='\0';msg[0]='\0';str[0]='\0';
	sprintf(msg,"220 %s(%s) ftp proxy ready\r\n",PROGRAM_NAME,VER_ID);
	server->send(msg);
	len=server->recv(str,malloc_len-4,"\r\n");
	if(len<=0){
		goto clean;
	}
//	printf("%s\n",str);
//	printf("len=%d.\n",len);
//	goto clean;
	if(split_user_host(str,msg,malloc_len-3,host,sizeof(host))!=0){
		strcat(msg,"\r\n");
	//	printf("%s:%d\n",__FILE__,__LINE__);
		if((port=split_host_port(host,':',sizeof(host)))<=0)
			port=21;
	}
check_connect:
	if(allow_connect(FTP,server,host,port)!=ALLOW)
		goto clean;
	if(client->connect(host,port)<=0){
		klog(MODEL_LOG,"ftp client %s connect host %s:%d error\n",server->get_addr(),host,port);
		goto clean;
	}
	if(is_local_ip(client))
		goto clean;
	klog(MODEL_LOG,"ftp client %s connect host %s:%d success\n",server->get_addr(),host,port);
	if(conf.ftp_redirect)
		goto done;
	for(;;){
		if((length=client->recv(str,malloc_len,"\r\n"))<=0){
			klog(ERR_LOG,"It is error while recv first message from remote host\n");
			goto clean;
		}
	//	printf("str=%s\n",str);
		if(strncmp(str,"220 ",4)==0)
			break;
	}
	if(client->send(msg)<=0){
		klog(ERR_LOG,"It is error while send first message to remote host\n");
		goto clean;
	}
done:
	free(msg);
	free(str);
	return client;
clean:
	if(msg)
		free(msg);
	if(str)
		free(str);
	delete client;
	return NULL;
}
void ftp_data_proxy(SERVER * m_server,FTP_DATA * m_ftp,char *str)
{


	mysocket client;
	pthread_t id;
	m_ftp->server.open(0);
	if(m_ftp->ftp_data_cmd==1)
		create_pasv_str(m_server,&m_ftp->server,str);
	else
		create_port_str(m_server,&m_ftp->server,str);
#ifndef _WIN32
	pthread_attr_t attr;
	pthread_attr_init(&attr);
	pthread_attr_setdetachstate(&attr,PTHREAD_CREATE_DETACHED);//设置线程为分离
#endif	
	pthread_create(&id,&attr,ftp_data_connection,(void *)m_ftp);

}
void create_pasv_str(SERVER *m_server,mysocket *server,char * str)
{

	char tmp[21];
	int port,port_low,port_high;
	strcpy(str,"227 Entering Passive Mode (");
	memset(tmp,0,sizeof(tmp));
	strncpy(tmp,m_server->server->getsockname(),sizeof(tmp)-1);
//	while(str_replace(tmp,".",",")>0);
	int tmp_len=strlen(tmp);
	for(int i=0;i<tmp_len;i++){
		if(tmp[i]=='.'){
			tmp[i]=',';
		}
	}
	strcat(str,tmp);
	port=server->get_port(OLD);
	port_high=port/0x100;
	port_low=port-port_high*0x100;
	memset(tmp,0,sizeof(tmp));
	snprintf(tmp,sizeof(tmp)-1,",%d,%d)\r\n",port_high,port_low);
	strcat(str,tmp);
}
void create_port_str(SERVER *m_server,mysocket *server,char * str)
{

	char tmp[20];
	int port,port_low,port_high;
	strcpy(str,"PORT ");
	strcpy(tmp,m_server->client->getsockname());
	while(str_replace(tmp,".",",")>0);
	strcat(str,tmp);
	port=server->get_port(OLD);
	port_high=port/0x100;
	port_low=port-port_high*0x100;
	sprintf(tmp,",%d,%d\r\n",port_high,port_low);
	strcat(str,tmp);
}

