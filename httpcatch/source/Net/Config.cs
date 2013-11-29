using System;
using System.Collections.Generic;

using System.Text;

namespace JrIntercepter.Net
{
    class Config
    {
        public static int ListenPort = 8888;  
        public static bool MapSocketToProcess = true;
        public static Encoding HeaderEncoding = Encoding.UTF8;
        public static bool EnableIPv6 = true;  
    }
}
