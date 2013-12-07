/*
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

*/
#ifdef HAVE_CONFIG_H
#include "config.h"
#endif
#include	"oops.h"
#include	"cache.h"
#include	"utils.h"
#include	"log.h"
#include	"server.h"
#include	"lib.h"
#include	"cron.h"
#include	"utils.h"
#include	"KThreadPool.h"
#include	"KDnsCache.h"
#include	"kingate.h"
#include 	<string>
#ifdef		HAVE_SSTREAM
#include <sstream>
#else
#include "mysstream"
#endif
#include	<time.h>
#include	"KUser.h"
#include	"KFilter.h"
#include "malloc_debug.h"

void list_all_malloc();
#define		DB_FLUSH_RATE	63

#define		CHECK_INTERVAL	5
#define		SWAP_RATE	0

using namespace std;
static int count_file_name=0;
volatile bool quit_kingate=false;
/*
 KMutex mem_cache_size_lock;
 KMutex disk_cache_size_lock;
 int mem_cache_size=0;
 int disk_cache_size=0;
 */
string 
get_service_to_name(int port);
extern KThreadPool m_thread_p;
void check_time()
{
	filter_time_map_t::iterator it;
	filter_time_lock.Lock();
	for(it=filter_time_list.begin();it!=filter_time_list.end();){
	//	printf("have one.uid=%d\n",(*it).second.m_state.uid);
		if(!conf.m_kfilter.Check((*it).second.m_state)){
			(*it).second.server->close();
#ifndef _WIN32
			pthread_kill((*it).first,KINGATE_NOTICE_THREAD);
			filter_time_list.erase(it);
			it++;
#else
			it=filter_time_list.erase(it);
#endif
			klog(RUN_LOG,"now force to close now.\n");
		}else{
			it++;
		}
	}	
clean:
	filter_time_lock.Unlock();
}
void closeAllConnection()
{
	filter_time_map_t::iterator it;
	filter_time_lock.Lock();
	unsigned tid;
	for(it=filter_time_list.begin();it!=filter_time_list.end();){
			(*it).second.server->close();
			tid=(*it).first;
#ifndef _WIN32
			pthread_kill(tid,KINGATE_NOTICE_THREAD);
			filter_time_list.erase(it);
			it++;
#else
			it=filter_time_list.erase(it);
			LogEvent("now force to close the connection.\n");
#endif
			klog(RUN_LOG,"now force to close the connection(thread_id=%d).\n",tid);
	}
clean:
	filter_time_lock.Unlock();
}
string getConnectionInfo()
{
	stringstream s;
	filter_time_map_t::iterator it;
	filter_time_lock.Lock();
	for(it=filter_time_list.begin();it!=filter_time_list.end();it++){
		s << "<tr><td>" << make_ip((*it).second.m_state.src_ip);
		#ifndef DISABLE_USER
		if((*it).second.m_state.uid>STARTUID){
			string user;
			m_user.GetUserName((*it).second.m_state.uid,user);
			s << "[" << user << "]";
		}
		#endif
		s << "</td><td>" << get_service_to_name((*it).second.m_state.service_port) ;
		s << "</td><td>" << make_ip((*it).second.m_state.dst_ip);
		s << "</td><td>" << (*it).second.m_state.dst_port;
		s << "</td></tr>";
	}	
	filter_time_lock.Unlock();
	return s.str();
}
void get_cache_size(int *total_mem_size,int *total_disk_size)
{
	int i;
	for (i=0;i<HASH_SIZE;i++) {
		assert(hash_table[i].size>=0);
		*total_mem_size += hash_table[i].size;
		*total_disk_size+=hash_table[i].disk_size;
    }
}
void flush_mem_cache(void)
{
	int	total_size=0, kill_size;//destroyed, gc_mode;
	int total_disk_size=0;
	get_cache_size(&total_size,&total_disk_size);
//	#ifdef MALLOCDEBUG
//	list_all_malloc();	
//	printf("now have total mem cache size=%d(%d-%d), total disk cache size=%d(%d-%d).\n",total_size,conf.mem_min_cache,conf.mem_max_cache,total_disk_size,conf.disk_min_cache,conf.disk_max_cache);
//	#endif
	if ( conf.mem_max_cache<total_size  ) {
		kill_size = total_size - conf.mem_min_cache;
	//	printf("mem kill_size=%d\n",kill_size);
		if(kill_size>0)
			delete_cache(kill_size);
	}
	if(conf.disk_max_cache<total_disk_size || cache_model==DROP_DEAD){
		if(cache_model==DROP_DEAD){
			kill_size=0;
		}else{
			kill_size = total_disk_size - conf.disk_min_cache;
			if(kill_size<=0){
				return;
			}
		}
	//	printf("disk kill_size=%d\n",kill_size);
		clean_disk(kill_size);
	}
	return;
}
FUNC_TYPE FUNC_CALL time_thread(void* arg)
{
	int i=1;
	KTimeMatch m_log_rotate;
	m_log_rotate.set(conf.log_rotate);
/*	m_log_rotate.Show();
	m_log_rotate.check();
*/	quit_kingate=false;
	int last_rotate_i=-31;
	int rotate_len=0;
	forever() {
		my_sleep(CHECK_INTERVAL);	
		if(quit_kingate)
			break;
		flush_mem_cache();
		check_time();
		klog_flush();
		#ifndef DISABLE_USER
		m_user.FlushLoginUser();
		#endif
		if(m_log_rotate.check()){//rotate log now
			rotate_len=i-last_rotate_i;
			if(rotate_len>30 || rotate_len<-30){
				klog_rotate();
				last_rotate_i=i;		
			}
		}
		if(i%12==0){//every minute
			m_thread_p.Flush();			
	//		m_dns_cache.Flush();
		}
		if(i%720==0){//every hour
			m_dns_cache.Flush();
			#ifndef DISABLE_USER
	
			m_user.SaveAll();
			#endif
		}
		i++;

    	} 
	return NULL;
}
u_short T_hash(struct url *url)
{
u_short		res = 0;
int		i;
char		*p;

    p = url->host;
    if ( p && *p ) {
		p = p+strlen(p)-1;
		i = 35;
		while ( (p >= url->host) && i ) i--,res += *p**p--;
    }
    p = url->path;
    if ( p && *p ) {
		p = p+strlen(p)-1;
		i = 35;
		while ( (p >= url->path) && i ) i--,res += *p**p--;
    }
    return(res & HASH_MASK);
}
string get_disk_cache_file(struct mem_obj *obj)
{
	assert(obj);
	stringstream s;	
	u_short url_hash = T_hash(&obj->url);
	int i=url_hash/1000;
	int j=url_hash/100-i*10;
	int k=url_hash/10-i*100-10*j;
	int p=url_hash-1000*i-100*j-10*k;
#ifndef _WIN32
	s << conf.path << "/var/cache/" << i << "/" << j << "/" << k << "/" << p << "/" ;

#else
	s << conf.path << "\\var\\cache\\" << i << "\\" << j << "\\" << k << "\\" << p << "\\" ;
#endif
	if(obj->file_name==0){
		obj->file_name=time(NULL)+count_file_name++;
	}
	s << obj->file_name;
	return s.str();
}
bool swap_out_obj(struct mem_obj *obj)
{
	string name;
	FILE *fp=NULL;
	struct buff *tmp;
	if(!conf.use_disk_cache)
		goto swap_out_failed;
	if(!TEST(obj->flags,FLAG_IN_MEM)){
		return true;
	}
	if(TEST(obj->flags,FLAG_IN_DISK)){
		goto skip_save_disk;
	}
	name=get_disk_cache_file(obj);
	fp=fopen(name.c_str(),"wb");
	if(fp==NULL){
		klog(ERR_LOG,"Cann't open file %s to write.\n",name.c_str());
		goto swap_out_failed;
	}
	
	klog(RUN_LOG, "Now swap out obj %s:%d%s\n",obj->url.host,obj->url.port,obj->url.path);
	tmp=obj->container;
	while(tmp!=NULL){
		if(fwrite(tmp->data,1,tmp->used,fp)<tmp->used){
			klog(ERR_LOG,"Cann't write file %s to disk.\n",name.c_str());
			goto swap_out_failed;
		}
		tmp=tmp->next;
	}
	fclose(fp);
	SET(obj->flags,FLAG_IN_DISK);
	increase_hash_size(obj->hash_back, obj->resident_size,false);//increase disk size
skip_save_disk:
	CLR(obj->flags,FLAG_IN_MEM);
	free_container(obj->container);
	obj->container=obj->hot_buff=NULL;
	decrease_hash_size(obj->hash_back, obj->resident_size);
	return true;
swap_out_failed:
	SET(obj->flags,FLAG_DEAD);
	cache_model=DROP_DEAD;
	free_container(obj->container);
	obj->container=obj->hot_buff=NULL;
//	decrease_hash_size(obj->hash_back, obj->resident_size);
	return false;
}
bool swap_in_obj(struct mem_obj *obj)
{	
	bool result=true;
	if(TEST(obj->flags,FLAG_IN_MEM))
		return true;
	string name=get_disk_cache_file(obj);
	FILE *fp=fopen(name.c_str(),"rb");
	if(fp==NULL){
		klog(ERR_LOG,"Cann't open file %s to read.\n",name.c_str());
		SET(obj->flags,FLAG_DEAD);
		return false;
	}
	klog(RUN_LOG,"Now swap in obj %s:%d%s\n",obj->url.host,obj->url.port,obj->url.path);
	char *buf=(char *)malloc(2048);
	if(buf==NULL){
		fclose(fp);
		SET(obj->flags,FLAG_DEAD);
		return false;
	}
	int total_len=0;
	for(;;){
		int len=fread(buf,1,2048,fp);
		if(len<=0)
			break;
		total_len+=len;
      		if(store_in_chain(buf, len, obj) ) {
               		klog(ERR_LOG, "Can't store.\n");
	              	obj->flags |= FLAG_DEAD;
			free_container(obj->container);
			fclose(fp);
			free(buf);
			return false;
        	}
	}
	free(buf);
	if(total_len!=obj->content_length){
		result=false;
                klog(ERR_LOG, "load from disk length: %d  is not equale content_length\n",total_len,obj->content_length );
		SET(obj->flags,FLAG_DEAD);
        }
	fclose(fp);
	if(result){
	//	CLR(obj->flags,FLAG_IN_DISK);
		SET(obj->flags,FLAG_IN_MEM);
		increase_hash_size(obj->hash_back, obj->resident_size);
	}
	return result;
}
