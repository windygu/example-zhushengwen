// iconTest.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <Windows.h>
#include <string>
using namespace std;
#include <shlobj.h> 
#include "resource.h"

struct DataBlock
{
	DataBlock():pt(NULL),size(0){}
	LPCSTR pt;
	DWORD size;
};
void WriteFileByPoint(LPCSTR lp,const string& path,ULONG length=0)
{
	if(length==0)length=strlen(lp);
	if(lp!=NULL)
	{
		FILE *stream;
		if ((stream = fopen(path.c_str(), "wb")) != NULL) /* open file TEST.$$$ */
		{
			fwrite(lp,length, 1, stream);
			fclose(stream); 
		}
	}
}
class split
{
public:
	split(const string& p,const char * s=" ")
	{
		length=strlen(p.c_str());
		ssps=p;
		sps=ssps.c_str();
		sp=s;
		reset();

	}
	string trim(const char* ts=" \n\t\v\r",int type=0)
	{
		if(sps)
		{
			const char * temp=sps;
			if(type==0 || type==1)
			{
				for(int i=0;sps[i];i++)
				{
					int j=0;
					for(;ts[j];j++)
					{
						if(ts[j]==sps[i])
						{
							temp++;
							break;
						}
					}
					if(ts[j]==sps[i])
					{
						continue;
					}
					else
					{
						break;
					}
				}
			}
			const char* temq=sps+length-1;
			if(type==0 || type ==2)
			{
				for(;temq>temp;temq--)
				{
					int j=0;
					for(;ts[j];j++)
					{
						if(ts[j]==*temq)
						{
							break;
						}
					}
					if(ts[j]==*temq)
					{
						continue;
					}
					else
					{
						break;
					}
				}
			}
			return string(temp,temq-temp+1);

		}
		return string("");

	}
	bool startwith(const char* sc)
	{
		return (sps && sc && sps[0]==sc[0] && sps==strstr(sps,sc));
	}
	bool endwith(const char* ec)
	{
		if(!ec)return true;
		if(!sps)return false;
		int len=strlen(ec)+1;
		const char * temp=sps+length-len+1;
		return (temp==strstr(temp,ec));
	}
	//将多个相邻的分隔符换成一个
	string uniqe()
	{
		string rets;
		for(int i=0;i<=count;i++)
		{
			if(at(i)!="")
			{
				if(rets!="")rets+=sp;
				rets+=at(i);
			}
		}
		return rets;
	}
	void setchars(const char * s)
	{
		if(s)sp=s;
		reset();
	}
	void reset()
	{
		point=strstr(sps,sp);
		count=recount();
		if(sps && sps[0] && point)
		{
			pointi=0;
			const char* tpoint=strstr(point+strlen(sp),sp);
			if(tpoint && tpoint<sps+length)
			{
				newlen=tpoint-point;
			}
			else
			{
				newlen=(sps+length)-point;
			}
		}
		else 
		{
			pointi=0;
			newlen=0;
		}
	}

	const char* operator[](int i)
	{
		if(point==NULL) return NULL;
		if(i<0 || i>=count)
			return NULL;
		while(i>pointi)add();
		while(i<pointi)dec();
		return point;

	}
	string replace(const char* reps)
	{
		reset();
		string str=at(0);
		for(int i=0;i<count;i++)
		{
			if(reps)
			{
				str.append(reps);
			}
			str.append(at(i+1));
		}
		return str;
	}
	string at(int i)
	{
		if(0==i)
		{
			const char* tpoint=strstr(sps,sp);
			int len=0;
			if(tpoint && tpoint<sps+length)
			{
				len=tpoint-sps;
			}
			else
			{
				len=(sps+length)-sps;
			}
			if(0==len)return string("");
			else
			{
				return string(sps,len);
			}
		}
		else --i;
		const char *p=(*this)[i];
		if(p)
		{	
			p+=strlen(sp);
			if(p>=sps+length)
				return string("");
			return string(p,newlen-strlen(sp));
		}
		return string("");
	}

	int getcount()
	{
		return count+1;
	}
	// 	int getcount()
	// 	{
	// 
	// 	}

protected:
private:
	string ssps;
	const char *sps;
	int length;
	const char * sp;
	int pointi;
	const char *point;
	int newlen;
	int count;
	int recount()
	{
		if(length==0)return 0;
		int i=strlen(sp);
		if(i==0)return 0;
		int counts=0;
		for(const char* ps=sps;ps-sps>=0&&ps-sps<length;ps+=i)
		{
			if(ps=strstr(ps,sp))
				++counts;
			else break;
		}

		return counts;
	}

	const char* add()
	{
		if(point==NULL) return NULL;
		const char* tpoint=strlen(sp)+point;
		if(tpoint<sps+length && tpoint>=sps) 
		{
			tpoint=strstr(tpoint,sp);
			if(tpoint)
			{
				point=tpoint;
				++pointi;
				int len=0;
				//newline获取新值
				const char* ttpoint=strstr(point+strlen(sp),sp);
				if(ttpoint && ttpoint<sps+length)
				{
					newlen=ttpoint-point;
				}
				else
					newlen=sps+length-point;
			}

			return tpoint;
		}
		else
			return NULL;


	}
	const char* dec()
	{
		if(point==NULL) return NULL;
		int i=strlen(sp);
		const char* tpoint=	tpoint=point-i;
		for(;tpoint>=sps&&tpoint<point;tpoint-=i)
		{
			const char * ttpoint=strstr(tpoint,sp);
			if(ttpoint && ttpoint>=sps && ttpoint <= point-i)
			{
				newlen=point-ttpoint;
				point=ttpoint;
				--pointi;
				return ttpoint;
			}
		}
		return NULL;
	}
};

string combine_path(const string& dir,string name)
{
	split sp(dir.c_str(),"/");
	string path=sp.trim("\\/");
	split sp1(name.c_str());
	sp1.trim("\\/");
	if(sp.getcount()==1)
	{
		path+="\\";
		sp1.setchars("\\");
		if(sp1.getcount()==1)
		{
			sp1.setchars("/");
			name=sp1.replace("\\");
		}
	}
	else
	{
		path+="/";
		sp1.setchars("/");
		if(sp1.getcount()==1)
		{
			sp1.setchars("\\");
			name=sp1.replace("/");
		}
	}
	if(name!="")path+=name;
	return path;
}

string get_desk_path(const string & comp="")
{
	TCHAR path1[255];
	SHGetSpecialFolderPath(0,path1,CSIDL_DESKTOPDIRECTORY,0);
	return combine_path(string((char*)path1),comp);
}
typedef struct _GRPICONDIRENTRY {
	BYTE   bWidth;               // Width, in pixels, of the image
	BYTE   bHeight;              // Height, in pixels, of the image
	BYTE   bColorCount;          // Number of colors in image (0 if >=8bpp)
	BYTE   bReserved;            // Reserved
	WORD   wPlanes;              // Color Planes
	WORD   wBitCount;            // Bits per pixel
	DWORD   dwBytesInRes;         // how many bytes in this resource?
	WORD   nID;                  // the ID
} GRPICONDIRENTRY, *LPGRPICONDIRENTRY;


typedef struct  _GRPICONDIR {
	WORD            idReserved;   // Reserved (must be 0)
	WORD            idType;       // Resource type (1 for icons)
	WORD            idCount;      // How many images?
	GRPICONDIRENTRY   idEntries[1]; // The entries for each image
} GRPICONDIR, *LPGRPICONDIR;
void FavIcon()
{
	DataBlock lp;
	string temp=get_desk_path("favicon.ico");
		HRSRC hRes = FindResource(NULL, MAKEINTRESOURCE(IDI_ICON1), RT_GROUP_ICON);
		HGLOBAL hGlobal = LoadResource(NULL, hRes);
		LPVOID lpIconDir = LockResource(hGlobal);
		hRes = FindResource(NULL,
			MAKEINTRESOURCE(((LPGRPICONDIR)lpIconDir)->idEntries[0].nID),
			RT_ICON);
		lp.size = SizeofResource(NULL, hRes);
		hGlobal = LoadResource(NULL, hRes);
		lp.pt = (LPCSTR)LockResource(hGlobal);
		if(lp.size)WriteFileByPoint(lp.pt,temp,lp.size);
	
}
int _tmain(int argc, _TCHAR* argv[])
{
	FavIcon();
	return 0;
}

