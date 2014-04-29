#define ConfigFile "ntconfig.ini"

bool file_exists(const string &filename)
{

	struct stat buffer;
	int exist = stat(filename.c_str(),&buffer);
	if(exist == 0)
		return true;
	else // -1
		return false;

}
string get_curr_path()
{
	char buff[MAX_PATH]={0};
	GetModuleFileName(NULL, buff, MAX_PATH);
	return buff;
}
string get_file_path(const string &path)
{
	string tpath=split(path.c_str()).trim("\\/");
	int pos=tpath.find_last_of("/");
	if(string::npos==pos)
	{
		pos=tpath.find_last_of("\\");
	}
	if(string::npos==pos)return "";
	return tpath.substr(0,pos);
}
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
class Config
{
public:
	Config()
	{
		
		string nfp=get_curr_path();
		string cfp=combine_path(get_file_path(nfp),ConfigFile);
		const char * FilePath=cfp.c_str();

		GetPrivateProfileString("DateBase", "host", "localhost", Db.host, 256, FilePath);
		GetPrivateProfileString("DateBase", "user", "root", Db.user, 20, FilePath);
		Db.port=GetPrivateProfileInt("DateBase", "port", 3306, FilePath);
		GetPrivateProfileString("DateBase", "pass", "admin", Db.pass, 20, FilePath);
		GetPrivateProfileString("DateBase", "dbname", "nt_m2m", Db.dbname, 20, FilePath);
		GetPrivateProfileString("DateBase", "charset", "GBK", Db.charset, 20, FilePath);
		Lsn.port=GetPrivateProfileInt("Listen", "port", 8012, FilePath);
		Lsn.lsncount=GetPrivateProfileInt("Listen", "lsncount", 20, FilePath);
		if(file_exists(FilePath))
		{
			cout<<"配置文件已发现！"<<endl;
		}
		else
		{
			cout<<"配置文件未发现，加载默认配置！"<<endl;
			fclose(fopen(FilePath,"w"));
			WritePrivateProfileString("DateBase", "host", Db.host, FilePath);
			WritePrivateProfileString("DateBase", "user",  Db.user, FilePath);
			WritePrivateProfileString("DateBase", "port", IAS(Db.port), FilePath);
			WritePrivateProfileString("DateBase", "pass",  Db.pass,  FilePath);
			WritePrivateProfileString("DateBase", "dbname",  Db.dbname,  FilePath);
			WritePrivateProfileString("DateBase", "charset",  Db.charset, FilePath);
			WritePrivateProfileString("Listen", "port", IAS(Lsn.port), FilePath);
			WritePrivateProfileString("Listen", "lsncount", IAS(Lsn.lsncount), FilePath);
		}
	}
public:
	struct DateBase
	{
		char host[256];
		char user[20];
		unsigned short port;
		char pass[20];
		char dbname[20];
		char charset[20];
	} Db;
	struct Listen
	{
		unsigned short port;
		unsigned short lsncount;
	} Lsn;

};