using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace _58
{
    #region 配置文件类 ConfigFile
    /// <summary>
    /// 配置文件类 ConfigFile,内容存储到文件中，继承自配置接口 （一定要给ConfigFile.Instanse.fileName 赋值）
    /// </summary>
    public class ConfigFile : IConfig
    {
        #region 文件物理路径 fileName
        private string _fileName = "";
        /// <summary>
        /// 文件物理路径
        /// </summary>
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        #endregion

        private ConfigFile() { }

        #region 单例模式，返回一个静态实例 Instanse
        private static ConfigFile _Instance;
        /// <summary>
        /// 实例
        /// </summary>
        public static ConfigFile Instanse
        {
            get { if (_Instance == null) { _Instance = new ConfigFile(); } return _Instance; }
        }
        #endregion

        #region CreateFile()文件不存在则创建文件
        /// <summary>
        /// CreateFile()文件不存在则创建文件
        /// </summary>
        /// <returns>返回文件是否是新建的</returns>
        public bool CreateFile()
        {
            bool isCreate = false;
            if (!File.Exists(fileName))
            {
                using (File.Create(fileName))//创建完成后立即释放资源，否则会占用、报错
                {
                }
                isCreate = true;
            }
            return isCreate;
        }
        #endregion

        #region 支持Get,set方法取值、加入值、修改值 this[Key]
        /// <summary>
        /// 索引函数
        /// </summary>
        /// <param name="Key">键（key）</param>
        /// <returns></returns>
        public string this[string Key]
        {
            get
            {
                if (CreateFile())//如果是刚新建的空文件，返回空
                {
                    return "";
                }
                else
                {
                    string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        var match = Regex.Match(line, @"(\w+)=([\w\W]+)");
                        string linekey = match.Groups[1].Value;
                        string lineValue = match.Groups[2].Value;
                        if (linekey == Key)
                        {
                            return lineValue; //如果匹配，返回找到的值
                        }
                    }
                    return "";//如果上面没找到，返回空
                }
            }
            set
            {
                if (CreateFile())
                {
                    File.AppendAllText(fileName, Key + "=" + value + "\r\n");//如果是新建的文件 ，直接加入
                }
                else
                {
                    string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        var match = Regex.Match(line, @"(\w+)=(\w+)");
                        string linekey = match.Groups[1].Value;
                        string lineValue = match.Groups[2].Value;
                        //逐行找，如果遇到名字等于name的就把这一行的值修改
                        //然后写回文件
                        if (linekey == Key)
                        {
                            lines[i] = linekey + "=" + value;
                            File.WriteAllLines(fileName, lines);
                            return;//如果找到值并修改了，不继续向下执行
                        }
                    }
                    File.AppendAllText(fileName, Key + "=" + value + "\r\n");//如果上面没找到，加入键值到文件结尾
                }

            }
        }
        #endregion

        #region 得到所有键名称的集合 List<string>
        /// <summary>
        /// 得到所有键名称的集合 List《string》
        /// </summary>
        public string[] Keys
        {
            get
            {
                List<string> listKey = new List<string>();
                if (CreateFile())
                {
                    return listKey.ToArray();
                }
                else
                {

                    string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        var match = Regex.Match(line, @"(\w+)=(\w+)");
                        string linekey = match.Groups[1].Value;
                        listKey.Add(linekey);
                    }
                    return listKey.ToArray();
                }
            }
        }
        #endregion

        #region 是否存在键 KeyExists(Key)
        /// <summary>
        /// 是否存在键 KeyExists(Key)
        /// </summary>
        /// <param name="Key">键名称</param>
        /// <returns></returns>
        public bool KeyExists(string Key)
        {
            return (Keys as ICollection<string>).Contains(Key);
        }
        #endregion

    }
    #endregion

    #region 加密的配置文件类 ConfigFileDES
    /// <summary>
    /// 加密的配置文件类 ConfigFileDES,内容存储到文件中，使用时必须调用 ConfigFileDES.Instanse.SetIConfig(IConfig) 赋值
    /// </summary>
    public class ConfigFileDES : IConfig
    {
        private IConfig _iconfig = null;
        /// <summary>
        /// 接口变量
        /// </summary>
        public IConfig iconfig
        {
            get { return _iconfig; }
            set { _iconfig = value; }
        }
        private ConfigFileDES() { }

        private static ConfigFileDES _Instance;
        /// <summary>
        /// 实例
        /// </summary>
        public static ConfigFileDES Instanse
        {
            get { if (_Instance == null) { _Instance = new ConfigFileDES(); } return _Instance; }
        }
        /// <summary>
        /// 传入一个可操作的继承自IConfig的类的对象
        /// </summary>
        /// <param name="iconfig">继承自IConfig的类的对象</param>
        public void SetIConfig(IConfig iconfig)//本加密类是在其它实现IConfig的类基础上进行，必须要传入一个类的实例
        {
            this.iconfig = iconfig;
        }
        /// <summary>
        /// 索引函数
        /// </summary>
        /// <param name="key">名称（key）</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (iconfig[key] == "") return "";
                return DesEncrypt.Decrypt(iconfig[key]);//解密后 取出
            }
            set
            {
                iconfig[key] = DesEncrypt.Encrypt(value);//加密后 存入
            }
        }
        /// <summary>
        /// 所有键的集合
        /// </summary>
        public string[] Keys
        {
            get { return iconfig.Keys; }
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键(key)</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return iconfig.KeyExists(key);
        }

    }
    #endregion

    #region 配置接口  IConfig
    /// <summary>
    /// 配置接口  IConfig
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 通过键得到对应的值
        /// </summary>
        /// <param name="Key">键(key)</param>
        /// <returns></returns>
        string this[string Key] { get; set; }
        /// <summary>
        /// 返回存在的键的集合
        /// </summary>
        string[] Keys { get; }
        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="Key">键名</param>
        /// <returns></returns>
        bool KeyExists(string Key);
    }
    #endregion

    /// <summary>
    /// 可逆加密解密类
    /// </summary>
    public class DesEncrypt
    {
        /// <summary>
        /// 解密函数
        /// </summary>
        /// <param name="Text">要解密的字符串(必须是经过加密后的字符串才能解密成功)</param>
        /// <param name="sKey">key</param>
        /// <returns></returns>
        public static string Decrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            int num = Text.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num3 = Convert.ToInt32(Text.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num3;
            }
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            return Encoding.Default.GetString(stream.ToArray());
        }
        /// <summary>
        /// 加密函数
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <param name="sKey">key</param>
        /// <returns></returns>
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(Text);
            provider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            return builder.ToString();
        }
        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="Text">要解密的字符串（必须是加密后的！不然格式不对！）</param>
        /// <returns></returns>
        public static string Decrypt(string Text)
        {
            return Decrypt(Text, "test");
        }
        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt(string Text)
        {
            return Encrypt(Text, "test");
        }
    }

}
