using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Ping = System.Net.NetworkInformation.Ping;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Management;

namespace Josing.Net
{

    public enum EncodingType
    {
        Defualt,
        ASCII,
        UTF8
    }

    public class NetHelper
    {
        //得到网关地址
        public static string GetGateway(bool boardcast = false)
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                string ip = "";
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        ip = IpEntry.AddressList[i].ToString();
                    if (ip.IndexOf("192.168") == 0) break;
                }
                if (boardcast)
                {
                    string[] str = ip.Split('.');
                    ip = str[0] + "." + str[1] + "." + str[2] + ".255";
                }
                return ip;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary> 
        /// 获得广播地址 
        /// </summary> 
        /// <param name="ipAddress">IP地址</param> 
        /// <param name="subnetMask">子网掩码</param> 
        /// <returns>广播地址</returns> 
        public static string GetBroadcast(string ipAddress, string subnetMask)
        {
            //ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            //ManagementObjectCollection nics = mc.GetInstances();
            //foreach (ManagementObject nic in nics)
            //{
            //    if (Convert.ToBoolean(nic["ipEnabled"]) == true)
            //    {
            //        Console.WriteLine((nic["IPAddress"] as String[])[0]);
            //        Console.WriteLine((nic["IPSubnet"] as String[])[0]);
            //        Console.WriteLine((nic["DefaultIPGateway"] as String[])[0]);
            //    }
            //}
            byte[] ip = IPAddress.Parse(ipAddress).GetAddressBytes();
            byte[] sub = IPAddress.Parse(subnetMask).GetAddressBytes();

            // 广播地址=子网按位求反 再 或IP地址 
            for (int i = 0; i < ip.Length; i++)
            {
                ip[i] = (byte)((~sub[i]) | ip[i]);
            }
            return new IPAddress(ip).ToString();
        }

        /// <summary>
        /// 字符转命令
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }

        public string HexByteToStr(byte[] buffer)
        {
            StringBuilder strBuider = new StringBuilder();
            for (int index = 0; index < buffer.Length; index++)
                strBuider.Append(((int)buffer[index]).ToString("X2"));
            return strBuider.ToString();
        }

        /// <summary>
        /// 指定类型的端口是否已经被使用了
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="type">端口类型</param>
        /// <returns></returns>
        public static bool PortInUse(int port, PortType type)
        {
            bool flag = false;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipendpoints = null;
            if (type == PortType.TCP)
                ipendpoints = properties.GetActiveTcpListeners();
            else
                ipendpoints = properties.GetActiveUdpListeners();
            foreach (IPEndPoint ipendpoint in ipendpoints)
            {
                if (ipendpoint.Port == port)
                {
                    flag = true; break;
                }
            }
            ipendpoints = null;
            properties = null;
            return flag;
        }

        public static void Ping(string ip, Action<bool> result)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Ping p1 = new Ping();
                PingReply reply = p1.Send(ip); //发送主机名或Ip地址
                if (reply.Status == IPStatus.Success) result(true);
                else result(false);
            });
        }

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
                macs.Add(ni.GetPhysicalAddress().ToString());
            return macs;
        }

        ///<summary>
        /// 通过SendARP获取网卡Mac
        /// 网络被禁用或未接入网络（如没插网线）时此方法失灵
        ///</summary>
        ///<param name="remoteIP"></param>
        ///<returns></returns>
        public static string GetMacBySendArp(string remoteIP)
        {
            StringBuilder macAddress = new StringBuilder();
            try
            {
                Int32 remote = inet_addr(remoteIP);
                Int64 macInfo = new Int64();
                Int32 length = 6;
                SendARP(remote, 0, ref macInfo, ref length);
                string temp = Convert.ToString(macInfo, 16).PadLeft(12, '0').ToUpper();
                int x = 12;
                for (int i = 0; i < 6; i++)
                {
                    if (i == 5)
                        macAddress.Append(temp.Substring(x - 2, 2));
                    else
                        macAddress.Append(temp.Substring(x - 2, 2) + "-");
                    x -= 2;
                }
                return macAddress.ToString();
            }
            catch
            {
                return macAddress.ToString();
            }
        }

        ///<summary>
        /// 根据截取ipconfig /all命令的输出流获取网卡Mac，支持不同语言编码
        ///</summary>
        ///<returns></returns>
        public static string GetMacByIpConfig()
        {
            List<string> macs = new List<string>();

            var runCmd = Cmd.RunCmd("chcp 437&&ipconfig/all");

            foreach (var line in runCmd.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()))
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith("Physical Address"))
                        macs.Add(line.Substring(36));
                    else if (line.StartsWith("DNS Servers") && line.Length > 36 && line.Substring(36).Contains("::"))
                        macs.Clear();
                    else if (macs.Count > 0 && line.StartsWith("NetBIOS") && line.Contains("Enabled"))
                        return macs.Last();
                }
            }

            return macs.FirstOrDefault();
        }

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        class Cmd
        {
            private static string CmdPath = @"C:\Windows\System32\cmd.exe";
            /// <summary>
            /// 执行cmd命令 返回cmd窗口显示的信息
            /// 多命令请使用批处理命令连接符：
            /// <![CDATA[
            /// &:同时执行两个命令
            /// |:将上一个命令的输出,作为下一个命令的输入
            /// &&：当&&前的命令成功时,才执行&&后的命令
            /// ||：当||前的命令失败时,才执行||后的命令]]>
            /// </summary>
            /// <param name="cmd">执行的命令</param>
            public static string RunCmd(string cmd)
            {
                cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = CmdPath;
                    p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                    p.Start();//启动程序

                    //向cmd窗口写入命令
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.AutoFlush = true;

                    //获取cmd窗口的输出信息
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();

                    return output;
                }
            }
        }
    }

    #region 端口枚举类型
    /// <summary>
    /// 端口类型
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// TCP类型
        /// </summary>
        TCP,
        /// <summary>
        /// UDP类型
        /// </summary>
        UDP
    }
    #endregion

    public class NetMessage
    {
        public string Msg { get; private set; }
        public byte[] Rawdata { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        System.Text.Encoding encoding;

        public NetMessage(byte[] data)
        {
            Rawdata = data;
            encoding = Encoding.Default;
            Msg = encoding.GetString(Rawdata);
            Ip = "";
            Port = 0;
        }

        public NetMessage(byte[] data, System.Text.Encoding encoding)
        {
            Rawdata = data;
            this.encoding = encoding;
            Msg = encoding.GetString(Rawdata);
            Ip = "";
            Port = 0;
        }

        public NetMessage(byte[] data, System.Text.Encoding encoding, IPEndPoint endPoint)
        {
            Rawdata = data;
            this.encoding = encoding;
            if(endPoint != null)
            {
                Ip = endPoint.Address.ToString();
                Port = endPoint.Port;
            }
            Msg = encoding.GetString(Rawdata);
        }
    }

    public class UMsg
    {
        public byte[] data;
        public IPEndPoint endPoint;
        public UMsg(byte[] data) { this.data = data; }

        public UMsg(byte[] data, string ip, int port)
        {
            this.data = data;
            this.endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
    }

    public enum SocketType
    {
        TcpClient,
        TcpServer,
        UDPClient,
        UDPServer
    }
}

