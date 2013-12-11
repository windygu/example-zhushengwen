using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;
namespace _58
{
    class Company
    {
        public Company() { }
        public string  address = "";//北京海淀12街道25号
        public string  areaid = "";//1143
        public string  checkBackCode = "";//
        public string  checkTelphone = "";//
        public string  enterpriseAlias = "";//大地科技
        public string  enterpriseName = "";//北京大地科技有限公司
        public string  fc = "111";
        public string  intro = "111";//简介
        public string  jscode = "";//42555548546254506348
        public string  jsmoverecord = "";
        public string  linkMan = "";//北京大地
        public string  phone = "";//15234516212
        public string  pics = "";//
        public string  telPhone = "";//
        public string  validatecode = "";
        public  string  ObjectToJson()
        {
            JsonSerializer jse = new JsonSerializer();
            StringWriter sw1=new StringWriter();
            jse.Serialize(sw1, this);

            return sw1.ToString ();
        }

    }
}