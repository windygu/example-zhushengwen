namespace JrIntercepter.Net 
{
    using System;
    using System.Runtime.InteropServices;

    internal class Winsock
    {
        private const int AF_INET = 2;
        private const int AF_INET6 = 0x17;
        private const int ERROR_INSUFFICIENT_BUFFER = 0x7a;
        private const int NO_ERROR = 0;

        private static int findPIDForConnection(int targetPort, uint addressType)
        {
            IntPtr zero = IntPtr.Zero;
            uint dwTcpTableLength = 0;
            int num2 = 12;
            int ofs = 12;
            int num4 = 0x18;
            if (addressType == 0x17)
            {
                num2 = 0x18;
                ofs = 0x20;
                num4 = 0x38;
            }
            if (0x7a == GetExtendedTcpTable(zero, ref dwTcpTableLength, false, addressType, TcpTableType.OwnerPidConnections, 0))
            {
                try
                {
                    zero = Marshal.AllocHGlobal((int) dwTcpTableLength);
                    if (GetExtendedTcpTable(zero, ref dwTcpTableLength, false, addressType, TcpTableType.OwnerPidConnections, 0) == 0)
                    {
                        int num5 = ((targetPort & 0xff) << 8) + ((targetPort & 0xff00) >> 8);
                        int num6 = Marshal.ReadInt32(zero);
                        if (num6 == 0)
                        {
                            return 0;
                        }
                        IntPtr ptr = (IntPtr) (((long) zero) + num2);
                        for (int i = 0; i < num6; i++)
                        {
                            if (num5 == Marshal.ReadInt32(ptr))
                            {
                                return Marshal.ReadInt32(ptr, ofs);
                            }
                            ptr = (IntPtr) (((long) ptr) + num4);
                        }
                        return 0; 
                    }
                   return 0;
                }
                finally
                {
                    Marshal.FreeHGlobal(zero);
                }
            }
            return 0;  
        }

        private static int findPIDForPort(int targetPort)
        {
            int num = 0;
            try
            {
                num = findPIDForConnection(targetPort, 2);
                if ((num > 0) || !Config.EnableIPv6)
                {
                    return num;
                }
                return findPIDForConnection(targetPort, 0x17);
            }
            catch (Exception exception)
            {
            }
            return 0;
        }

        [DllImport("iphlpapi.dll", SetLastError=true, ExactSpelling=true)]
        private static extern uint GetExtendedTcpTable(
            IntPtr pTcpTable, 
            ref uint dwTcpTableLength, 
            [MarshalAs(UnmanagedType.Bool)] bool sort, 
            uint ipVersion, 
            TcpTableType tcpTableType, 
            uint reserved
        );
        
        internal static int MapLocalPortToProcessId(int port)
        {
            return findPIDForPort(port);
        }

        private enum TcpTableType
        {
            BasicListener,
            BasicConnections,
            BasicAll,
            OwnerPidListener,
            OwnerPidConnections,
            OwnerPidAll,
            OwnerModuleListener,
            OwnerModuleConnections,
            OwnerModuleAll
        }
    }
}

