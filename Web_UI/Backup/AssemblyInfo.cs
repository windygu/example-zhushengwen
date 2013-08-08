using System.Reflection;
using log4net.Config;
//log4net使用一个FileInfo对象来指定配置文件
[assembly : XmlConfigurator(ConfigFile="log4net.config", Watch=true)]
//产品名称 
[assembly : AssemblyProduct("")]
//公司 
[assembly : AssemblyCompany("")]
//版权 
[assembly : AssemblyCopyright("1997-2006")]
//合法商标 
[assembly : AssemblyTrademark("")]
//内部名称 
[assembly : AssemblyCulture("")]
[assembly : AssemblyTitle("界面实现")]
[assembly : AssemblyDescription("本组件负责实现界面交互")]
[assembly : AssemblyConfiguration("")]
[assembly : AssemblyVersion("1.0.2")]
[assembly : AssemblyDelaySign(false)]
[assembly : AssemblyKeyFile("")]
[assembly : AssemblyKeyName("")]