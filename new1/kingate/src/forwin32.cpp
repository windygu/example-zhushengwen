#include<time.h>
#include "forwin32.h"
#include "kingate.h"
#include "malloc_debug.h"

#ifdef _WIN32

SERVICE_STATUS        ServiceStatus;
SERVICE_STATUS_HANDLE ServiceStatusHandle;

int  pthread_mutex_init(pthread_mutex_t *mutex,void *t)
{
//	char tmp[100];
//	sprintf(tmp,"lock%d%d",rand(),time(NULL));
	*mutex=CreateMutex(NULL,FALSE,NULL);
	return 0;
};
int pthread_mutex_destroy(pthread_mutex_t *mutex)
{
	CloseHandle(*mutex);
	return 0;
}

extern void StartAll();
extern void StopAll();
void WINAPI ThreadProc()
{
    StartAll();
}

void WINAPI kingateControl(DWORD dwCode)
{
    switch(dwCode)
    {
    case SERVICE_CONTROL_PAUSE:
        ServiceStatus.dwCurrentState = SERVICE_PAUSED;
        break;

    case SERVICE_CONTROL_CONTINUE:
        ServiceStatus.dwCurrentState = SERVICE_RUNNING;
        break;
    case SERVICE_CONTROL_STOP:
        StopAll();
        ServiceStatus.dwCurrentState  = SERVICE_STOPPED;
        ServiceStatus.dwWin32ExitCode = 0;
        ServiceStatus.dwCheckPoint    = 0;
        ServiceStatus.dwWaitHint      = 0;
        if(SetServiceStatus(ServiceStatusHandle,&ServiceStatus)==0)
        {
            OutputDebugString("SetServiceStatus in CmdControl in Switch Error !\n");
        }
 
        return ;
    case SERVICE_CONTROL_INTERROGATE:
        break;
    default:
        break;
    }

    if(SetServiceStatus(ServiceStatusHandle,&ServiceStatus)==0)
    {
        OutputDebugString("SetServiceStatus in CmdControl out Switch Error !\n");
    }
}

void WINAPI kingateMain(DWORD argc, LPTSTR * argv)
{
    HANDLE    hThread;
    ServiceStatus.dwServiceType             = SERVICE_WIN32;
    ServiceStatus.dwCurrentState            = SERVICE_START_PENDING;
    ServiceStatus.dwControlsAccepted        = SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_PAUSE_CONTINUE;
    ServiceStatus.dwServiceSpecificExitCode = 0;
    ServiceStatus.dwWin32ExitCode           = 0;
    ServiceStatus.dwCheckPoint              = 0;
    ServiceStatus.dwWaitHint                = 0;

    ServiceStatusHandle = RegisterServiceCtrlHandler("kingate",kingateControl);

    if(ServiceStatusHandle == 0)
    {
        OutputDebugString("RegisterServiceCtrlHandler Error !\n");
        return ;
    }

    ServiceStatus.dwCurrentState = SERVICE_RUNNING;
    ServiceStatus.dwCheckPoint   = 0;
    ServiceStatus.dwWaitHint     = 0;

    if(SetServiceStatus(ServiceStatusHandle,&ServiceStatus)==0)
    {
        OutputDebugString("SetServiceStatus in CmdStart Error !\n");
        return ;
    }

    hThread=CreateThread(NULL,0,(LPTHREAD_START_ROUTINE)ThreadProc,NULL,0,NULL);
    if(hThread==NULL)
    {
        OutputDebugString("CreateThread in CmdStart Error !\n");
    }

    return ;
}
bool InstallService(const char * szServiceName)
{
    char             szCurrentPath[MAX_PATH];
    GetModuleFileName(NULL,szCurrentPath,MAX_PATH);
	SERVICE_STATUS   InstallServiceStatus;
    DWORD dwErrorCode = 0;
    SC_HANDLE schSCManager = OpenSCManager(NULL,NULL,SC_MANAGER_ALL_ACCESS);
    if(schSCManager == NULL)
    {
        printf("Open Service Control Manager Database Failure !\n");
        return false;
    }

    printf("Creating Service .... ");
    SC_HANDLE schService=CreateService(schSCManager,szServiceName,szServiceName,SERVICE_ALL_ACCESS,
        SERVICE_WIN32_OWN_PROCESS,SERVICE_AUTO_START,
        SERVICE_ERROR_IGNORE,szCurrentPath,NULL,NULL,NULL,NULL,NULL); 
    if(schService==NULL)
    {
        dwErrorCode=GetLastError();
        if(dwErrorCode!=ERROR_SERVICE_EXISTS)
        {
            printf("Failure !\n");
            CloseServiceHandle(schSCManager);
            return false;
        }
        else
        {
            printf("already Exists !\n");
            schService = OpenService(schSCManager,szServiceName,SERVICE_START);
            if(schService==NULL)
            {
                printf("Opening Service .... Failure !\n");
                CloseServiceHandle(schSCManager);
                return false;
            }
        }
    }
    else
    {
        printf("Success !\n");
    }

    printf("Starting Service .... ");
    if(StartService(schService,0,NULL)==0)                         
    {
        dwErrorCode=GetLastError();
        if(dwErrorCode==ERROR_SERVICE_ALREADY_RUNNING)
        {
            printf("already Running !\n");
            CloseServiceHandle(schSCManager);  
            CloseServiceHandle(schService);
            return true;
        }
    }
    else
    {
        printf("Pending ... ");
    }

    while(QueryServiceStatus(schService,&InstallServiceStatus)!=0)           
    {
        if(InstallServiceStatus.dwCurrentState==SERVICE_START_PENDING)
        {
            Sleep(100);
        }
        else
        {
            break;
        }
    }
    if(InstallServiceStatus.dwCurrentState!=SERVICE_RUNNING)
    {
        printf("Failure !\n");                       
    }
    else
    {
        printf("Success !\n");
    }

    CloseServiceHandle(schSCManager);
    CloseServiceHandle(schService);

    return InstallServiceStatus.dwCurrentState == SERVICE_RUNNING;
}
bool UninstallService(const char * szServiceName)
{
    DWORD dwErrorCode = 0;
    SERVICE_STATUS   RemoveServiceStatus;
    SC_HANDLE schSCManager = OpenSCManager(NULL,NULL,SC_MANAGER_ALL_ACCESS);
    if(schSCManager == NULL)
    {
        printf("Opening SCM ......... ");
        dwErrorCode = GetLastError();
        if(dwErrorCode != 5)
        {
            printf("Failure !\n"); 
        }
        else
        {
            printf("Failuer ... Access is Denied !\n");
        }
        return false;
    }

    SC_HANDLE schService = OpenService(schSCManager,szServiceName,SERVICE_ALL_ACCESS);
    if(schService==NULL) 
    {
        printf("Opening Service ..... ");
        dwErrorCode=GetLastError();
        if(dwErrorCode==1060)
        {
            printf("no Exists !\n");
        }
        else
        {
            printf("Failure !\n");
        }
        CloseServiceHandle(schSCManager);
        return false;
    }
    else
    {
        printf("Stopping Service .... ");
        if(QueryServiceStatus(schService,&RemoveServiceStatus)!=0)
        {
            if(RemoveServiceStatus.dwCurrentState==SERVICE_STOPPED)
            {
                printf("already Stopped !\n"); 
            }
            else
            {
                printf("Pending ... ");
                if(ControlService(schService,SERVICE_CONTROL_STOP,&RemoveServiceStatus)!=0)
                {
                    while(RemoveServiceStatus.dwCurrentState==SERVICE_STOP_PENDING)         
                    {
                        Sleep(10);
                        QueryServiceStatus(schService,&RemoveServiceStatus);
                    }
                    if(RemoveServiceStatus.dwCurrentState==SERVICE_STOPPED)
                    {
                        printf("Success !\n");
                    }
                    else
                    {
                        printf("Failure !\n");
                    }
                }
                else
                {
                    printf("Failure !\n");          
                }
            }
        }
        else
        {
            printf("Query Failure !\n");
        }

        printf("Removing Service .... ");     
        if(DeleteService(schService)==0)
        {
            printf("Failure !\n");   
        }
        else
        {
            printf("Success !\n");
        }
    }

    CloseServiceHandle(schSCManager);        
    CloseServiceHandle(schService);
    return RemoveServiceStatus.dwCurrentState == SERVICE_STOPPED;
}

void Log(const char *format, va_list argptr)
{
    char szMsg[1024];
    _vsnprintf(szMsg, sizeof(szMsg)-2, format, argptr);
    szMsg[sizeof(szMsg)-1] = '\0';
    strcat(szMsg, "\n");

    OutputDebugString(szMsg);
}
bool InstallService(const char * szServiceName)
{
    char             szCurrentPath[MAX_PATH];
    GetModuleFileName(NULL,szCurrentPath,MAX_PATH);
	SERVICE_STATUS   InstallServiceStatus;
    DWORD dwErrorCode = 0;
    SC_HANDLE schSCManager = OpenSCManager(NULL,NULL,SC_MANAGER_ALL_ACCESS);
    if(schSCManager == NULL)
    {
        printf("Open Service Control Manager Database Failure !\n");
        return false;
    }

    printf("Creating Service .... ");
    SC_HANDLE schService=CreateService(schSCManager,szServiceName,szServiceName,SERVICE_ALL_ACCESS,
        SERVICE_WIN32_OWN_PROCESS,SERVICE_AUTO_START,
        SERVICE_ERROR_IGNORE,szCurrentPath,NULL,NULL,NULL,NULL,NULL); 
    if(schService==NULL)
    {
        dwErrorCode=GetLastError();
        if(dwErrorCode!=ERROR_SERVICE_EXISTS)
        {
            printf("Failure !\n");
            CloseServiceHandle(schSCManager);
            return false;
        }
        else
        {
            printf("already Exists !\n");
            schService = OpenService(schSCManager,szServiceName,SERVICE_START);
            if(schService==NULL)
            {
                printf("Opening Service .... Failure !\n");
                CloseServiceHandle(schSCManager);
                return false;
            }
        }
    }
    else
    {
        printf("Success !\n");
    }

    printf("Starting Service .... ");
    if(StartService(schService,0,NULL)==0)                         
    {
        dwErrorCode=GetLastError();
        if(dwErrorCode==ERROR_SERVICE_ALREADY_RUNNING)
        {
            printf("already Running !\n");
            CloseServiceHandle(schSCManager);  
            CloseServiceHandle(schService);
            return true;
        }
    }
    else
    {
        printf("Pending ... ");
    }

    while(QueryServiceStatus(schService,&InstallServiceStatus)!=0)           
    {
        if(InstallServiceStatus.dwCurrentState==SERVICE_START_PENDING)
        {
            Sleep(100);
        }
        else
        {
            break;
        }
    }
    if(InstallServiceStatus.dwCurrentState!=SERVICE_RUNNING)
    {
        printf("Failure !\n");                       
    }
    else
    {
        printf("Success !\n");
    }

    CloseServiceHandle(schSCManager);
    CloseServiceHandle(schService);

    return InstallServiceStatus.dwCurrentState == SERVICE_RUNNING;
}
bool UninstallService(const char * szServiceName)
{
    DWORD dwErrorCode = 0;
    SERVICE_STATUS   RemoveServiceStatus;
    SC_HANDLE schSCManager = OpenSCManager(NULL,NULL,SC_MANAGER_ALL_ACCESS);
    if(schSCManager == NULL)
    {
        printf("Opening SCM ......... ");
        dwErrorCode = GetLastError();
        if(dwErrorCode != 5)
        {
            printf("Failure !\n"); 
        }
        else
        {
            printf("Failuer ... Access is Denied !\n");
        }
        return false;
    }

    SC_HANDLE schService = OpenService(schSCManager,szServiceName,SERVICE_ALL_ACCESS);
    if(schService==NULL) 
    {
        printf("Opening Service ..... ");
        dwErrorCode=GetLastError();
        if(dwErrorCode==1060)
        {
            printf("no Exists !\n");
        }
        else
        {
            printf("Failure !\n");
        }
        CloseServiceHandle(schSCManager);
        return false;
    }
    else
    {
        printf("Stopping Service .... ");
        if(QueryServiceStatus(schService,&RemoveServiceStatus)!=0)
        {
            if(RemoveServiceStatus.dwCurrentState==SERVICE_STOPPED)
            {
                printf("already Stopped !\n"); 
            }
            else
            {
                printf("Pending ... ");
                if(ControlService(schService,SERVICE_CONTROL_STOP,&RemoveServiceStatus)!=0)
                {
                    while(RemoveServiceStatus.dwCurrentState==SERVICE_STOP_PENDING)         
                    {
                        Sleep(10);
                        QueryServiceStatus(schService,&RemoveServiceStatus);
                    }
                    if(RemoveServiceStatus.dwCurrentState==SERVICE_STOPPED)
                    {
                        printf("Success !\n");
                    }
                    else
                    {
                        printf("Failure !\n");
                    }
                }
                else
                {
                    printf("Failure !\n");          
                }
            }
        }
        else
        {
            printf("Query Failure !\n");
        }

        printf("Removing Service .... ");     
        if(DeleteService(schService)==0)
        {
            printf("Failure !\n");   
        }
        else
        {
            printf("Success !\n");
        }
    }

    CloseServiceHandle(schSCManager);        
    CloseServiceHandle(schService);
    return RemoveServiceStatus.dwCurrentState == SERVICE_STOPPED;
}

void LogEvent(LPCTSTR pFormat, ...)
{
    va_list args;
    va_start(args,pFormat);
    Log(pFormat, args);
    va_end(args);
}
/*
int
strerror_r(int err, char *errbuf, size_t lerrbuf)
{
LPTSTR	lpszMsgBuf = NULL;
char	b[80];

    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
		  FORMAT_MESSAGE_FROM_SYSTEM |
		  FORMAT_MESSAGE_IGNORE_INSERTS,
		  NULL, (DWORD)err,
		  MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                  (LPTSTR)&lpszMsgBuf, 0, NULL);

    if ( lpszMsgBuf == NULL ) {
	sprintf(b, "Error: (%d)", err);
	if ( lerrbuf > 0 ) strncpy(errbuf, b, lerrbuf-1);
	return(-1);

    }

    strncpy(errbuf, lpszMsgBuf, MIN(lerrbuf-1, strlen(lpszMsgBuf)-1));
    LocalFree(lpszMsgBuf);
    return(0);
}
*/
#endif
