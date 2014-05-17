using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;


///功能：全局 变量 常量 等
///完成时间：2009-9-19
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：增加对语言资源文件的处理。
///版本：01.80.00
///修订：无
namespace SoukeyNetget
{
    public static class cGlobalParas
    {

        #region 枚举常量
        public enum TaskState
        {
            UnStart = 1010,
            Started=1011,
            Aborted=1012,
            Waiting=1013,
            Running = 1014,
            Pause = 1015,
            Stopped = 1016,
            Exporting = 1017,
            Completed=1018,
            Failed = 1019,
            Publishing=1020,
        }

        public enum GatherResult
        {
            GatherSucceed=1071,
            GatherFailed=1072,
            PublishSuccees=1073,
            PublishFailed=1074,
        }

        public enum DownloadResult
        {
            Succeed=1081,
            Failed=1082,
            Err=1083,
        }

        public enum GDataType
        {
            Txt=1091,
            Picture=1092,
            Flash=1093,
            File=1094,
        }

        public enum GatherThreadState
        {
            UnStart = 1030,
            Started = 1031,
            Running = 1032,
            Stopped = 1033,
            Completed = 1034,
            Failed = 1035,
            Aborted=1036,
            Waiting=1037,
        }

        public enum FormState
        {
            New=1021,
            Edit=1022,
            Browser=1023,
        }

        public enum TaskRunType
        {
            OnlyGather=1041,
            GatherExportData=1042,
        }

        public enum TaskType
        {
            HtmlByUrl=1051,
            RssByUrl=1052,
            HtmlByWeb=1053,
            AjaxHtmlByUrl=1054,
        }

        public enum PublishType
        {
            NoPublish=1060,
            PublishAccess = 1061,
            PublishMSSql = 1062,
            PublishTxt=1063,
            PublishExcel=1064,
            PublishMySql=1065,
            PublishWeb=1066,
        }

        public enum LimitSign
        {
           NoLimit = 2001,          //不做任意格式的限制
           NoWebSign = 2002,        //匹配时去掉网页符号
           OnlyCN = 2003,           //只匹配中文
           OnlyDoubleByte=2004,     //只匹配双字节字符
           OnlyNumber=2005,         //只匹配数字
           OnlyChar=2006,           //只匹配字母数字及常用字符
           Custom = 2007,           //自定义正则匹配表达式 
        }

        public enum ExportLimit
        {
            ExportNoLimit = 2040,      //不做输出控制
            ExportNoWebSign = 2041,    //输出时去掉网页符号
            ExportPrefix = 2042,       //输出时附加前缀  
            ExportSuffix = 2043,       //输出时附加后缀
            ExportTrimLeft = 2044,     //左起去掉字符
            ExportTrimRight = 2045,    //右起去掉字符
            ExportReplace = 2046,      //替换其中符合条件的字符
            ExportTrim = 2047,         //去掉字符串首尾空格
            ExportRegexReplace = 2048, //输出时采用正则表达式进行替换


        }

        public enum WebCode
        {
            auto = 1000,
            gb2312 = 1001,
            utf8 = 1002,
            gbk = 1003,
            big5=1004,
            NoCoding=1005,
        }

        public enum ExitPara
        {
            Exit=2010,
            MinForm=2012,
        }

        public enum UpdateUrlCountType
        {
            Gathered=2020,
            Err=2021,
            ReIni=2022,
            UrlCountAdd=2023,
            ErrUrlCountAdd=2024,
        }

        public enum UrlGatherResult
        {
            UnGather=2031,
            Succeed=2032,
            Error=2033,
            Gathered=2034,
        }

        public enum DatabaseType
        {
            Access=2051,
            MSSqlServer=2052,
            MySql=2053,
        }

        public enum LogType
        {
            Info=2061,
            Error=2062,
            Warning=2063,
            RunPlanTask=2064,
            GatherError=2065,
            PublishError=2066,
        }

        public enum RunTaskType
        {
            SoukeyTask=2071,           //Soukey采摘任务
            DataTask=2072,               //数据库存储过程
            OtherTask=2073,            //外接可执行命令
        }

        public enum RunTaskPlanType
        {
            Ones=2081,                //仅运行一次
            DayOnes=2082,             //每天运行一次
            DayTwice=2083,            //每天上午下午各运行一次
            Weekly=2084,              //每周运行
            Custom=2085,              //自定义运行间隔
        }

        public enum PlanDisabledType
        {
            RunTime=2091,            //按照运行次数失效
            RunDateTime=2092,        //按照时间失效 
        }

        public enum PlanState
        {
            Enabled=3001,           //有效
            Disabled=3002,          //无效
            Expired =3003,          //过期
        }

        //监听类型
        public enum MessageType
        {
            RunSoukeyTask=3010,
            RunFileTask=3011,
            RunData=3013,
            ReloadPlan=3013,
            MonitorFileFaild=3014,
        }

        //触发器类型
        public enum TriggerType
        {
            GatheredRun=3020,
            PublishedRun=3021,
        }

        public enum CurLanguage
        {
            Auto=3031,
            zhCN=3032,
            enUS=3033,
        }

        #endregion
       

        #region 界面显示与系统处理转换  这个需要改成字典表进行筛选了，不能这样了
        static public int ConvertID(string enumName)
        {
            int cID = 0;

            if (System.Globalization.CultureInfo.CurrentUICulture.Name == "zh-CN")
            {
                cID=ConvertIDByCN(enumName);
            }
            else
            {
                cID=ConvertIDByEN(enumName);
            }

            return cID;

        }

        static private int ConvertIDByCN(string enumName)
        {
            switch (enumName)
            {
                case "不编码":
                    return (int)WebCode.NoCoding;
                case "自动":
                    return (int)WebCode.auto;
                case "gb2312":
                    return (int)WebCode.gb2312;
                case "UTF-8":
                    return (int)WebCode.utf8;
                case "gbk":
                    return (int)WebCode.gbk;
                case "big5":
                    return (int)WebCode.big5;


                case "仅采集数据":
                    return (int)TaskRunType.OnlyGather;
                case "采集并发布数据":
                    return (int)TaskRunType.GatherExportData;

                case "根据网址采集网页数据":
                    return (int)TaskType.HtmlByUrl;
                case "采集整站网页数据":
                    return (int)TaskType.HtmlByWeb;
                case "采集RSS聚合数据":
                    return (int)TaskType.RssByUrl;
                case "采集ajax网页数据":
                    return (int)TaskType.AjaxHtmlByUrl;


                case "不做任意格式的限制":
                    return (int)LimitSign.NoLimit;
                case "匹配时去掉网页符号":
                    return (int)LimitSign.NoWebSign;
                case "只匹配中文":
                    return (int)LimitSign.OnlyCN;
                case "只匹配双字节字符":
                    return (int)LimitSign.OnlyDoubleByte;
                case "只匹配数字":
                    return (int)LimitSign.OnlyNumber;
                case "只匹配字母数字及常用字符":
                    return (int)LimitSign.OnlyChar;
                case "自定义正则匹配表达式":
                    return (int)LimitSign.Custom;

                case "不做输出控制":
                    return (int)ExportLimit.ExportNoLimit;
                case "输出时去掉网页符号":
                    return (int)ExportLimit.ExportNoWebSign;
                case "输出时附加前缀":
                    return (int)ExportLimit.ExportPrefix;
                case "输出时附加后缀":
                    return (int)ExportLimit.ExportSuffix;
                case "左起去掉字符":
                    return (int)ExportLimit.ExportTrimLeft;
                case "右起去掉字符":
                    return (int)ExportLimit.ExportTrimRight;
                case "替换其中符合条件的字符":
                    return (int)ExportLimit.ExportReplace;
                case "去掉字符串首尾空格":
                    return (int)ExportLimit.ExportTrim;
                case "输出时采用正则表达式进行替换":
                    return (int)ExportLimit.ExportRegexReplace;

                case "文件":
                    return (int)GDataType.File;
                case "Flash":
                    return (int)GDataType.Flash;
                case "图片":
                    return (int)GDataType.Picture;
                case "文本":
                    return (int)GDataType.Txt;

                case "Soukey采摘任务":
                    return (int)RunTaskType.SoukeyTask;
                case "数据库任务":
                    return (int)RunTaskType.DataTask;
                case "其它任务":
                    return (int)RunTaskType.OtherTask;

                case "超过指定运行次数计划失效":
                    return (int)PlanDisabledType.RunTime;
                case "超过指定时间计划失效":
                    return (int)PlanDisabledType.RunDateTime;

                case "MS Access":
                    return (int)DatabaseType.Access;
                case "MS Sqlserver":
                    return (int)DatabaseType.MSSqlServer;
                case "MySql":
                    return (int)DatabaseType.MySql;

                default:
                    return 0;
            }
        }

        static private int ConvertIDByEN(string enumName)
        {
            switch (enumName)
            {
                case "No Encoding":
                    return (int)WebCode.NoCoding;
                case "Auto":
                    return (int)WebCode.auto;
                case "gb2312":
                    return (int)WebCode.gb2312;
                case "UTF-8":
                    return (int)WebCode.utf8;
                case "gbk":
                    return (int)WebCode.gbk;
                case "big5":
                    return (int)WebCode.big5;


                case "Only Collection":
                    return (int)TaskRunType.OnlyGather;
                case "Collection and Publish":
                    return (int)TaskRunType.GatherExportData;
                case "Collection by url":
                    return (int)TaskType.HtmlByUrl;
                case "Collection by website":
                    return (int)TaskType.HtmlByWeb;
                case "Collection by RSS":
                    return (int)TaskType.RssByUrl;
                case "Ajax ":
                    return (int)TaskType.AjaxHtmlByUrl;


                case "NO Limit":
                    return (int)LimitSign.NoLimit;
                case "No Inclued Html-Code":
                    return (int)LimitSign.NoWebSign;
                case "Only matching Chinese":
                    return (int)LimitSign.OnlyCN;
                case "Only matching Double-Character":
                    return (int)LimitSign.OnlyDoubleByte;
                case "Only matching number":
                    return (int)LimitSign.OnlyNumber;
                case "Only matching character":
                    return (int)LimitSign.OnlyChar;
                case "Use Regex":
                    return (int)LimitSign.Custom;

                case "Absoluteness":
                    return (int)ExportLimit.ExportNoLimit;
                case "Delete Html-Coding":
                    return (int)ExportLimit.ExportNoWebSign;
                case "Add prefix":
                    return (int)ExportLimit.ExportPrefix;
                case "Add suffix":
                    return (int)ExportLimit.ExportSuffix;
                case "Trim from left":
                    return (int)ExportLimit.ExportTrimLeft;
                case "Trim from right":
                    return (int)ExportLimit.ExportTrimRight;
                case "Replace":
                    return (int)ExportLimit.ExportReplace;
                case "Trim blank":
                    return (int)ExportLimit.ExportTrim;
                case "Replace use Regex":
                    return (int)ExportLimit.ExportRegexReplace;

                case "File":
                    return (int)GDataType.File;
                case "Flash":
                    return (int)GDataType.Flash;
                case "Picture":
                    return (int)GDataType.Picture;
                case "Text":
                    return (int)GDataType.Txt;

                case "SoukeyNetget Task":
                    return (int)RunTaskType.SoukeyTask;
                case "StoreProcess":
                    return (int)RunTaskType.DataTask;
                case "Program":
                    return (int)RunTaskType.OtherTask;

                case "Disabled by run-time":
                    return (int)PlanDisabledType.RunTime;
                case "Disabled by time":
                    return (int)PlanDisabledType.RunDateTime;

                case "MS Access":
                    return (int)DatabaseType.Access;
                case "MS Sqlserver":
                    return (int)DatabaseType.MSSqlServer;
                case "MySql":
                    return (int)DatabaseType.MySql;

                default:
                    return 0;
            }
        }

        static public string  ConvertName(int CustomType)
        {
            ResourceManager rm = new ResourceManager("SoukeyNetget.Resources.globalPara", Assembly.GetExecutingAssembly());
            string str="";

            switch (CustomType)
            {
                case (int)WebCode.NoCoding :
                    str= rm.GetString("WebCode1").ToString();
                    break;
                case (int)WebCode.auto:
                    str = rm.GetString("WebCode2").ToString();
                    break ;
                case (int)WebCode.gb2312:
                    str = rm.GetString("WebCode3").ToString();
                    break ;
                case (int)WebCode.utf8:
                    str = rm.GetString("WebCode4").ToString();
                    break ;
                case (int)WebCode.gbk:
                    str = rm.GetString("WebCode5").ToString();
                    break ;
                case (int)WebCode.big5 :
                    str = rm.GetString("WebCode6").ToString();
                    break ;

                case (int)TaskRunType.OnlyGather :
                    str = rm.GetString("TaskRunType1").ToString();
                    break ;
                case (int)TaskRunType.GatherExportData :
                    str = rm.GetString("TaskRunType2").ToString();
                    break ;
                case (int)TaskType.HtmlByUrl :
                    str = rm.GetString("TaskType1").ToString();
                    break ;
                case (int)TaskType.HtmlByWeb :
                    str = rm.GetString("TaskType2").ToString();
                    break ;
                case (int)TaskType.RssByUrl :
                    str = rm.GetString("TaskType3").ToString();
                    break ;
                case (int)TaskType.AjaxHtmlByUrl :
                    str = rm.GetString("TaskType4").ToString();
                    break;

                case (int)LimitSign.NoLimit:
                    str = rm.GetString("LimitSign1").ToString();
                    break;
                case (int)LimitSign.NoWebSign:
                    str = rm.GetString("LimitSign2").ToString();
                    break;
                case (int)LimitSign.OnlyCN:
                    str = rm.GetString("LimitSign3").ToString();
                    break;
                case (int)LimitSign.OnlyDoubleByte:
                    str = rm.GetString("LimitSign4").ToString();
                    break;
                case (int)LimitSign.OnlyNumber:
                    str = rm.GetString("LimitSign5").ToString();
                    break;
                case (int)LimitSign.OnlyChar:
                    str = rm.GetString("LimitSign6").ToString();
                    break;
                case (int)LimitSign.Custom:
                    str = rm.GetString("LimitSign7").ToString();
                    break;

                case (int)ExportLimit.ExportNoLimit:
                    str = rm.GetString("ExportLimit1").ToString();
                    break;
                case (int)ExportLimit.ExportNoWebSign:
                    str = rm.GetString("ExportLimit2").ToString();
                    break;
                case (int)ExportLimit.ExportPrefix:
                    str = rm.GetString("ExportLimit3").ToString();
                    break;
                case (int)ExportLimit.ExportSuffix:
                    str = rm.GetString("ExportLimit4").ToString();
                    break;
                case (int)ExportLimit.ExportTrimLeft:
                    str = rm.GetString("ExportLimit5").ToString();
                    break;
                case (int)ExportLimit.ExportTrimRight:
                    str = rm.GetString("ExportLimit6").ToString();
                    break;
                case (int)ExportLimit.ExportReplace:
                    str = rm.GetString("ExportLimit7").ToString();
                    break;
                case (int)ExportLimit.ExportTrim:
                    str = rm.GetString("ExportLimit8").ToString();
                    break;
                case (int)ExportLimit.ExportRegexReplace:
                    str = rm.GetString("ExportLimit9").ToString();
                    break;

                case (int)PublishType.NoPublish :
                    str = rm.GetString("PublishType1").ToString();
                    break;
                case (int)PublishType.PublishAccess:
                    str = rm.GetString("PublishType2").ToString();
                    break;
                case (int)PublishType.PublishMSSql:
                    str = rm.GetString("PublishType3").ToString();
                    break;
                case (int)PublishType.PublishExcel:
                    str = rm.GetString("PublishType4").ToString();
                    break;
                case (int)PublishType.PublishTxt:
                    str = rm.GetString("PublishType5").ToString();
                    break;
                case (int)PublishType.PublishMySql:
                    str = rm.GetString("PublishType6").ToString();
                    break;
                case (int)PublishType.PublishWeb:
                    str = rm.GetString("PublishType7").ToString();
                    break;

                case (int)GDataType.File :
                    str = rm.GetString("GDataType1").ToString();
                    break;
                case (int)GDataType.Flash:
                    str = rm.GetString("GDataType2").ToString();
                    break;
                case (int)GDataType.Picture:
                    str = rm.GetString("GDataType3").ToString();
                    break;
                case (int)GDataType.Txt:
                    str = rm.GetString("GDataType4").ToString();
                    break;

                case (int)RunTaskType.SoukeyTask :
                    str = rm.GetString("RunTaskType1").ToString();
                    break;
                case (int)RunTaskType.DataTask:
                    str = rm.GetString("RunTaskType2").ToString();
                    break;
                case (int)RunTaskType.OtherTask:
                    str = rm.GetString("RunTaskType3").ToString();
                    break;

                case (int)PlanDisabledType.RunTime:
                    str = rm.GetString("PlanDisabledType1").ToString();
                    break;
                case (int)PlanDisabledType.RunDateTime:
                    str = rm.GetString("PlanDisabledType2").ToString();
                    break;

                case (int)PlanState .Enabled :
                    str = rm.GetString("PlanState1").ToString();
                    break;
                case (int)PlanState.Disabled:
                    str = rm.GetString("PlanState2").ToString();
                    break;
                case (int)PlanState.Expired:
                    str = rm.GetString("PlanState3").ToString();
                    break;

                case (int)DatabaseType.Access:
                    str = rm.GetString("DatabaseType1").ToString();
                    break;
                case (int)DatabaseType.MSSqlServer:
                    str = rm.GetString("DatabaseType2").ToString();
                    break;
                case (int)DatabaseType.MySql:
                    str = rm.GetString("DatabaseType3").ToString();
                    break;

                default:
                    break;
            }

            rm = null;
            return str;
        }

        #endregion

    }
}
