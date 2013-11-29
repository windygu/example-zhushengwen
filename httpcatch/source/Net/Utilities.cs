namespace JrIntercepter.Net
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Windows.Forms;

    public class Utilities        
    {

        [CodeDescription("Try parsing the string for a Hex-formatted int. If it fails, return false and 0 in iOutput.")]
        public static bool TryHexParse(string sInput, out int iOutput)
        {
            return int.TryParse(sInput, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out iOutput);
        }

        internal static string HtmlEncode(string sInput)
        {
            if (sInput == null)
            {
                return null;
            }
            return HttpUtility.HtmlEncode(sInput);
        }


        internal static bool IsChunkedBodyComplete(Session m_session, MemoryStream oData, long iStartAtOffset, out long outStartOfLatestChunk, out long outEndOfEntity)
        {
            long num = iStartAtOffset;
            outStartOfLatestChunk = num;
            outEndOfEntity = -1L;
            while (num < oData.Length)
            {
                outStartOfLatestChunk = num;
                oData.Position = num;
                byte[] buffer = new byte[0x20];
                oData.Read(buffer, 0, buffer.Length);
                string sInput = Encoding.ASCII.GetString(buffer);
                int index = sInput.IndexOf("\r\n", StringComparison.Ordinal);
                if (index > -1)
                {
                    num += index + 2;
                    sInput = sInput.Substring(0, index);
                }
                else
                {
                    return false;
                }
                index = sInput.IndexOf(';');
                if (index > -1)
                {
                    sInput = sInput.Substring(0, index);
                }
                int iOutput = 0;
                if (!TryHexParse(sInput, out iOutput))
                {
                    return true;
                }
                if (iOutput == 0)
                {
                    oData.Position = num;
                    bool flag = true;
                    bool flag2 = false;
                    for (int i = oData.ReadByte(); i != -1; i = oData.ReadByte())
                    {
                        int num5 = i;
                        if (num5 != 10)
                        {
                            if (num5 != 13)
                            {
                                goto Label_010C;
                            }
                            flag2 = true;
                        }
                        else if (flag2)
                        {
                            if (flag)
                            {
                                outEndOfEntity = oData.Position;
                                oData.Position = oData.Length;
                                return true;
                            }
                            flag = true;
                            flag2 = false;
                        }
                        else
                        {
                            flag2 = false;
                            flag = false;
                        }
                        continue;
                    Label_010C:
                        flag2 = false;
                        flag = false;
                    }
                    return false;
                }
                num += iOutput + 2;
            }
            oData.Position = oData.Length;
            return false;
        }

        [CodeDescription("Returns TRUE if the HTTP Method MUST have a body.")]
        public static bool HTTPMethodRequiresBody(string sMethod)
        {
            return ("PROPPATCH" == sMethod);
        }


        [CodeDescription("Returns the part of a string after (but not including) the last instance of specified delimiter. If delim not found, returns entire string.")]
        public static string TrimBeforeLast(string sString, char chDelim)
        {
            if (sString == null)
            {
                return string.Empty;
            }
            int num = sString.LastIndexOf(chDelim);
            if (num < 0)
            {
                return sString;
            }
            return sString.Substring(num + 1);
        }


        [CodeDescription("This function attempts to be a ~fast~ way to return an IP from a hoststring that contains an IP-Literal. ")]
        public static IPAddress IPFromString(string sHost)
        {
            for (int i = 0; i < sHost.Length; i++)
            {
                if (((((sHost[i] != '.') && (sHost[i] != ':')) && ((sHost[i] < '0') || (sHost[i] > '9'))) && ((sHost[i] < 'A') || (sHost[i] > 'F'))) && ((sHost[i] < 'a') || (sHost[i] > 'f')))
                {
                    return null;
                }
            }
            if (sHost.EndsWith("."))
            {
                sHost = TrimBeforeLast(sHost, '.');
            }
            try
            {
                return IPAddress.Parse(sHost);
            }
            catch
            {
                return null;
            }
        }


        [CodeDescription("This function cracks the Host/Port combo, removing IPV6 brackets if needed.")]
        public static void CrackHostAndPort(string sHostPort, out string sHostname, ref int iPort)
        {
            int length = sHostPort.LastIndexOf(':');
            if ((length > -1) && (length > sHostPort.LastIndexOf(']')))
            {
                if (!int.TryParse(sHostPort.Substring(length + 1), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out iPort))
                {
                    iPort = -1;
                }
                sHostname = sHostPort.Substring(0, length);
            }
            else
            {
                sHostname = sHostPort;
            }
            if (sHostname.StartsWith("[", StringComparison.Ordinal) && sHostname.EndsWith("]", StringComparison.Ordinal))
            {
                sHostname = sHostname.Substring(1, sHostname.Length - 2);
            }
        }

       
        public static bool areOriginsEquivalent(string sHost1, string sHost2, int iDefaultPort)
        {
            string str;
            string str3;
            if (string.Equals(sHost1, sHost2, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            int iPort = iDefaultPort;
            CrackHostAndPort(sHost1, out str, ref iPort);
            string a = str + ":" + iPort.ToString();
            iPort = iDefaultPort;
            CrackHostAndPort(sHost2, out str3, ref iPort);
            string b = str3 + ":" + iPort.ToString();
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        [CodeDescription("Returns a string representing a Hex view of a byte array. Slow.")]
        public static string ByteArrayToHexView(byte[] inArr, int iBytesPerLine)
        {
            return ByteArrayToHexView(inArr, iBytesPerLine, inArr.Length);
        }

        [CodeDescription("Returns a string representing a Hex view of a byte array. PERF: Slow.")]
        public static string ByteArrayToHexView(byte[] inArr, int iBytesPerLine, int iMaxByteCount)
        {
            if ((inArr == null) || (inArr.Length == 0))
            {
                return string.Empty;
            }
            if ((iBytesPerLine < 1) || (iMaxByteCount < 1))
            {
                throw new ArgumentOutOfRangeException("iBytesPerLine", "iBytesPerLine and iMaxByteCount must be >0");
            }
            iMaxByteCount = Math.Min(iMaxByteCount, inArr.Length);
            StringBuilder builder = new StringBuilder(iMaxByteCount * 5);
            int num = 0;
            bool flag = false;
            while (num < iMaxByteCount)
            {
                int num2 = Math.Min(iBytesPerLine, iMaxByteCount - num);
                flag = num2 < iBytesPerLine;
                for (int i = 0; i < num2; i++)
                {
                    builder.Append(inArr[num + i].ToString("X2"));
                    builder.Append(" ");
                }
                if (flag)
                {
                    builder.Append(new string(' ', 3 * (iBytesPerLine - num2)));
                }
                builder.Append(" ");
                for (int j = 0; j < num2; j++)
                {
                    if (inArr[num + j] < 0x20)
                    {
                        builder.Append(".");
                    }
                    else
                    {
                        builder.Append((char) inArr[num + j]);
                    }
                }
                if (flag)
                {
                    builder.Append(new string(' ', iBytesPerLine - num2));
                }
                builder.Append("\r\n");
                num += iBytesPerLine;
            }
            return builder.ToString();
        }

        [CodeDescription("Returns a string representing a Hex stream of a byte array. Slow.")]
        public static string ByteArrayToString(byte[] inArr)
        {
            if (inArr == null)
            {
                return "null";
            }
            if (inArr.Length == 0)
            {
                return "empty";
            }
            StringBuilder builder = new StringBuilder(inArr.Length * 3);
            for (int i = 0; i < inArr.Length; i++)
            {
                builder.Append(inArr[i].ToString("X2") + ' ');
            }
            return builder.ToString();
        }

    
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [Flags]
        private enum FlashWInfo
        {
            FLASHW_ALL = 3,
            FLASHW_CAPTION = 1,
            FLASHW_STOP = 0,
            FLASHW_TIMER = 4,
            FLASHW_TIMERNOFG = 12,
            FLASHW_TRAY = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public int cbSize;
            public IntPtr hwnd;
            public Utilities.FlashWInfo dwFlags;
            public int uCount;
            public int dwTimeout;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LV_COLUMN
        {
            public uint mask;
            public int fmt;
            public int cx;
            public string pszText;
            public int cchTextMax;
            public int iSubItem;
            public int iImage;
            public int iOrder;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SendDataStruct
        {
            public IntPtr dwData;
            public int cbData;
            public string strData;
        }

        [Flags]
        internal enum SoundFlags
        {
            SND_ALIAS = 0x10000,
            SND_ALIAS_ID = 0x110000,
            SND_ASYNC = 1,
            SND_FILENAME = 0x20000,
            SND_LOOP = 8,
            SND_MEMORY = 4,
            SND_NODEFAULT = 2,
            SND_NOSTOP = 0x10,
            SND_NOWAIT = 0x2000,
            SND_RESOURCE = 0x40004,
            SND_SYNC = 0
        }
    }
}

