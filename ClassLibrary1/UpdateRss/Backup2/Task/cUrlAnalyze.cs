using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Web;
using System.Reflection;
using System.Resources;
using SoukeyNetget.Gather;

///功能：任务中URL规则解析处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.Task
{
    //主要处理带有参数的网址信息
    //分解/解析 等
    public class cUrlAnalyze
    {
        public cUrlAnalyze()
        {
        }

        ~cUrlAnalyze()
        {
        }

        ///根据指定的导航规则进行页面导航，在1.6版本中，增加了多层导航的功能
        ///网址导航是属于一对多的关系，即每一级别的导航都是属于一对多（也会是一对一的关系）
        ///在此无论是几级导航，返回的都是最终的需要采集内容的网址
        ///因为是多层导航，所以是属于递归的一种算法
        ///解析网址后返回的都是标准网址，不会存在相对网址的情况
        public List<string> ParseUrlRule(string Url, List<cNavigRule> nRules,cGlobalParas.WebCode webCode, string cookie)
        {
            List<string> pUrls = new List<string>();
            List<string> Urls = new List<string>();
            
            pUrls.Add(Url);

            //第一层导航分解都是从一个单一网址进行，之所以
            //选择集合，是为了统一调用接口参数
            try
            {
                Urls = PUrlRule(pUrls, 1, nRules, webCode, cookie);
            }
            catch (System.Exception ex)
            {
                //导航失败，无法解析导航规则
                return null;
            }

            return Urls;
        }

        ///解析导航网页
        ///判断是否为最后一个级别，在这里需要注意一个问题，因为有可能
        ///存储的级别并不是按照顺序进行的，所以，要根据传入的级别Level进行
        ///判断，否则会出现错误，导航网页的解析必须是按照顺序的，否则会
        ///无法解析
        public List<string> PUrlRule(List<string> pUrl,int Level, List<cNavigRule> nRules,cGlobalParas.WebCode webCode, string cookie)
        {
            List<string> tmpUrls;
            List<string> Urls =new List<string> ();

            if (nRules.Count == 0)
            {
                Urls.Add(pUrl[0].ToString());
                return Urls;
            }
           
            string UrlRule="";
            int i;

            //根据Level得到需要导航级别的导航规则
            for (i = 0; i < nRules.Count; i++)
            {
                if (Level ==nRules[i].Level )
                {
                    UrlRule =nRules [i].NavigRule ;
                    break;
                }
            }


            for (i = 0; i < pUrl.Count; i++)
            {
                tmpUrls = new List<string>();

                tmpUrls = GetUrlsByRule(pUrl[i].ToString(), UrlRule,webCode,cookie);

                if (tmpUrls != null)
                {
                    Urls.AddRange(tmpUrls);
                }
            }

            //判断是否为最底级的导航，如果是则返回，如果不是则继续导航
            if (Level == nRules.Count)
            {
                return Urls;
            }
            else
            {
                List<string> rUrls=  PUrlRule(Urls, Level + 1, nRules,webCode,cookie);
                return rUrls;
            }

        }

        //根据导航规则，获取网页地址，是一个集合
        public List<string> GetUrlsByRule(string Url, string UrlRule,cGlobalParas.WebCode webCode, string cookie)
        {
            string Url1;
            List<string> Urls=new List<string> ();

            if (UrlRule.Trim() == "")
            {
                Urls.Add(Url);
                return Urls;
            }

            //判断网址是否存在参数，如果存在参数则取出第一个可用网址
            if (Regex.IsMatch(Url, "{.*}"))
            {
                List<string> Urls1 = SplitWebUrl(Url );  //,IsUrlEncode ,UrlEncode
                Url1 = Urls1[0].ToString();
            }
            else
            {
                Url1 = Url;
            }

            //返回网址的源码，并根据提取规则提取导航的网址
            //string UrlSource= cTool.GetHtmlSource(Url1,true );

            cGatherWeb gW = new cGatherWeb();
            string UrlSource = gW.GetHtml(Url1, webCode, cookie, "", "", true, false);
            gW = null;

            if (UrlSource == "")
            {
                return null ;
            }

            //string Rule=@"(?<=href=[\W])" + cTool.RegexReplaceTrans(UrlRule) + @"(\S[^'"">]*)(?=[\s'""])";
            string Rule = "";

            if (UrlRule.StartsWith("<Regex:"))
            {
                Rule = @"(?<=[href=|src=|open(][\W])";

                //处理前缀
                string strPre = UrlRule.Substring(UrlRule.IndexOf("<Regex:")+7, UrlRule.IndexOf(">")-7);
                Rule += strPre;
                
                //处理中间内容
                string cma=@"(?<=<Common:)\S+?(?=>)";

                Regex cmas = new Regex(cma, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection cs = cmas.Matches(UrlRule);
                foreach (Match ma in cs)
                {
                    Rule +=@"(\S*)" + ma.Value.ToString ();
                }

                //处理后缀
                if (Regex.IsMatch(UrlRule, "<End:"))
                {
                    string s = UrlRule.Substring(UrlRule.IndexOf("<End:") + 5, UrlRule.Length - UrlRule.IndexOf("<End:") - 6);
                    Rule += @"(\S*)" + s;
                }
                else
                {
                    Rule += @"(\S[^'"">]*)(?=[\s'""])";
                }
                
            }
            else
            {
                Rule = @"(?<=[href=|src=|open(][\W])" + cTool.RegexReplaceTrans(UrlRule) + @"(\S[^'"">]*)(?=[\s'""])";
            }

            Regex re = new Regex(Rule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection aa = re.Matches(UrlSource);

            DataTable d1 = new DataTable();
            d1.Columns.Add("Name");

            foreach (Match ma in aa)
            {
                //Urls.Add(ma.Value.ToString());
                d1.Rows.Add(ma.Value.ToString());
            }

            //导航时可能会获取重复网址的列表，所以需要去重
            //去除重复行

            string[] strComuns = new string[d1.Columns.Count];

            for (int m = 0; m < d1.Columns.Count; m++)
            {
                strComuns[m] = d1.Columns[m].ColumnName;
            }

            DataView dv = new DataView(d1);

            DataTable d2 = dv.ToTable(true, strComuns);

            for (int i = 0; i < d2.Rows.Count; i++)
            {
                if (string.Compare(d2.Rows[i][0].ToString ().Substring (0,4), "http", true) != 0)
                {
                    string PreUrl = Url;

                    if (d2.Rows[i][0].ToString().Substring(0, 1) == "/")
                    {
                        PreUrl = PreUrl.Substring(7, PreUrl.Length - 7);
                        PreUrl = PreUrl.Substring(0, PreUrl.IndexOf("/"));
                        PreUrl = "http://" + PreUrl;
                    }
                    else
                    {
                        Match a = Regex.Match(PreUrl, ".*/");
                        PreUrl = a.Groups[0].Value.ToString();
                    }

                    Urls.Add(PreUrl + d2.Rows[i][0].ToString());
                }
                else
                {
                    Urls.Add(d2.Rows[i][0].ToString());
                }

            }

            return Urls;
        }

        //拆分网址
        public List<string> SplitWebUrl(string Url)         //, bool IsUrlEncode
        {
            List<string> Urls = new List<string>();

            try
            {
                Urls = SplitUrl(Url);
            }
            catch (System.Exception ex)
            {
                throw new cSoukeyException(ex.Message);
            }
            
            return Urls;                                  //, IsUrlEncode, ""
        }

        //public List<string> SplitWebUrl(string Url)//, bool IsUrlEncode, string UrlEncode
        //{
        //    return SplitUrl(Url);//, IsUrlEncode, UrlEncode
        //}

        private List<string> SplitUrl(string Url)   //, bool IsUrlEncode, string UrlEncode
        {
            List<string> tmp_list_Url=new List<string>() ;
            List<string> list_Url;
            List<string> g_Url = new List<string>();
            string Para = "";

            if (!Regex.IsMatch(Url, "{.*}"))
            {
                tmp_list_Url.Add(Url);
                return tmp_list_Url;
            }

            //增加tmp_list_Url的初始值
            //初始值为Url第一个参数前所有字符
            //应该以{为准
            tmp_list_Url.Add(Url.Substring (0,Url.IndexOf ("{")));

            //初始化参数值
            //Para = Url.Substring(Url.IndexOf("?") + 1, Url.IndexOf("=") - Url.IndexOf("?") );

            //判断是否存在
            while (Regex.IsMatch(Url, "{.*}"))
            {
                //提取参数内容
                string strMatch = "(?<={)[^}]*(?=})";
                Match s = Regex.Match(Url, strMatch,RegexOptions.IgnoreCase);
                string UrlPara = s.Groups[0].Value;

                g_Url = getListUrl(UrlPara); //,IsUrlEncode ,UrlEncode 

                list_Url = new List<string>();
               
                for (int j=0 ;j<tmp_list_Url.Count ;j++)
                {
                    for (int i=0;i<g_Url.Count ;i++)
                    {
                        list_Url.Add(tmp_list_Url[j].ToString() + Para + g_Url[i].ToString());

                    }
                }

                tmp_list_Url =list_Url ;
                list_Url = null;

                Url = Url.Substring(Url.IndexOf("}")+1, Url.Length - Url.IndexOf("}")-1);

                //判断是否还有参数，如果有，则截取中间部分以拼接网址
                if (Url.IndexOf("{") > 0)
                {
                    //Para = Url.Substring(Url.IndexOf("&"), Url.IndexOf("=") - Url.IndexOf("&") + 1);
                    Para =Url.Substring (0,Url.IndexOf ("{"));
                }
                 
            }

            list_Url = new List<string>();
            
            for (int m = 0; m < tmp_list_Url.Count; m++)
            {
                list_Url.Add(tmp_list_Url[m].ToString() + Url);
            }

            tmp_list_Url = null;
            g_Url = null;

            return list_Url;

        }

        //判断当前所含参数的数量,其中包括字典的内容
        public int GetUrlCount(string Url)
        {
            if (Url == "")
                return 0;

            int UrlCount = 1;
            List<string> g_Url = new List<string>();

            while (Regex.IsMatch(Url, "{.*}"))
            {
                //提取参数内容
                string strMatch = "(?<={)[^}]*(?=})";
                Match s = Regex.Match(Url, strMatch, RegexOptions.IgnoreCase);
                string UrlPara = s.Groups[0].Value;

                //因为仅计算网址的数量，所以不需要对url进行编码转换，如果有需要转换的话，也不需要
                g_Url = getListUrl(UrlPara); 

                UrlCount = UrlCount * g_Url.Count;
                Url = Url.Substring(Url.IndexOf("}") + 1, Url.Length - Url.IndexOf("}") - 1);

            }

            if (UrlCount == 0)
            {
                UrlCount = 1;
            }
            return UrlCount;
        }

        private List<string> getListUrl(string dicPre)//,bool IsUrlEncode,string UrlEncode
        {
            List<string> list_Para=new List<string>();
            Regex re;
            MatchCollection aa;
            int step;
            int startI;
            int endI;
            int i = 0;

            switch (dicPre.Substring(0, dicPre.IndexOf(":")))
            {
                
                case "Num":

                    re = new Regex("([\\-\\d]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    aa = re.Matches(dicPre);

                    startI = int.Parse(aa[0].Groups[0].Value.ToString());
                    endI = int.Parse(aa[1].Groups[0].Value.ToString());
                    step = int.Parse(aa[2].Groups[0].Value.ToString());

                    if (step > 0)
                    {
                        for (i = startI; i <= endI; i = i + step)
                        {
                            list_Para.Add(i.ToString());
                        }
                    }
                    else
                    {
                        for (i = startI; i >= endI; i = i + step)
                        {
                            list_Para.Add(i.ToString());
                        }
                    }

                    

                    break;
                
                case "Letter":
                    startI =getAsc( dicPre.Substring(dicPre.IndexOf(":") + 1, 1));
                    endI  =getAsc( dicPre.Substring(dicPre.IndexOf(",") + 1, 1));

                    if (startI > endI)
                    {
                        step = -1;
                    }
                    else
                    {
                        step = 1;
                    }

                    for (i = startI; i <= endI; i = i + step)
                    {
                        char s;
                        s = Convert.ToChar(i);  
                        list_Para.Add(s.ToString ());
                    }

                    break;
                case "Dict":
                    cDict d = new cDict();
                    string tClass = dicPre.Substring(dicPre.IndexOf(":") + 1, dicPre.Length - dicPre.IndexOf(":") - 1);
                    DataView dName = d.GetDict(tClass);
                   
                    //在此不进行url参数编码处理了
                    //if (IsUrlEncode == true)
                    //{
                    //    for (i = 0; i < dName.Count; i++)
                    //    {
                    //        switch ((cGlobalParas.WebCode)(int.Parse(UrlEncode)))
                    //        {
                    //            case cGlobalParas.WebCode.utf8 :
                    //                list_Para.Add(HttpUtility.UrlEncode(dName[i].Row["Name"].ToString(), Encoding.UTF8));
                    //                break;
                    //            case cGlobalParas.WebCode.gb2312 :
                    //                list_Para.Add(HttpUtility.UrlEncode(dName[i].Row["Name"].ToString(), Encoding.GetEncoding("gb2312")));
                    //                break;
                    //            case cGlobalParas.WebCode.gbk :
                    //                list_Para.Add(HttpUtility.UrlEncode(dName[i].Row["Name"].ToString(), Encoding.GetEncoding("gbk")));
                    //                break;
                    //            case cGlobalParas.WebCode .big5 :
                    //                list_Para.Add(HttpUtility.UrlEncode(dName[i].Row["Name"].ToString(), Encoding.GetEncoding("big5")));
                    //                break;
                    //            default :
                    //                list_Para.Add(HttpUtility.UrlEncode(dName[i].Row["Name"].ToString(), Encoding.UTF8));
                    //                break;
                    //        }
                            
                            
                    //    }
                    //}
                    //else
                    //{ 
                        for (i = 0; i < dName.Count; i++)
                        {
                            list_Para.Add(dName[i].Row["Name"].ToString());
                        }
                    //}

                    break;
                default:
                    list_Para = null;
                    break;
            }

            return list_Para;
        }

        private int getAsc(string s)
        {
            byte[] array = new byte[1];
            array = System.Text.Encoding.ASCII.GetBytes(s);
            int asciicode = (int)(array[0]);
            return  asciicode; 
        }
    }
}
