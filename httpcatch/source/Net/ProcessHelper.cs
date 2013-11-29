namespace JrIntercepter.Net 
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal static class ProcessHelper
    {
        private static readonly Dictionary<int, ProcessNameCacheEntry> processNames = new Dictionary<int, ProcessNameCacheEntry>();
        private const int MSEC_PROCESSNAME_CACHE_LIFETIME = 0x7530;

        static ProcessHelper()
        {
        }

        internal static string GetProcessName(int pid)
        {
            try
            {
                ProcessNameCacheEntry entry;
                if (processNames.TryGetValue(pid, out entry))
                {
                    if (entry.LastLookup > (Environment.TickCount - 0x7530))
                    {
                        return entry.ProcessName;
                    }
                    lock (processNames)
                    {
                        processNames.Remove(pid);
                    }
                }
                string str = Process.GetProcessById(pid).ProcessName.ToLower();
                if (string.IsNullOrEmpty(str))
                {
                    return string.Empty;
                }
                lock (processNames)
                {
                    if (!processNames.ContainsKey(pid))
                    {
                        processNames.Add(pid, new ProcessNameCacheEntry(str));
                    }
                }
                return str;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        internal static void ScavengeCache()
        {
            lock (processNames)
            {
                List<int> list = new List<int>();
                foreach (KeyValuePair<int, ProcessNameCacheEntry> pair in processNames)
                {
                    if (pair.Value.LastLookup < (Environment.TickCount - 0x7530))
                    {
                        list.Add(pair.Key);
                    }
                }
                foreach (int num in list)
                {
                    processNames.Remove(num);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ProcessNameCacheEntry
        {
            public readonly int LastLookup;
            public readonly string ProcessName;
            public ProcessNameCacheEntry(string processName)
            {
                this.LastLookup = Environment.TickCount;
                this.ProcessName = processName;
            }
        }
    }
}

