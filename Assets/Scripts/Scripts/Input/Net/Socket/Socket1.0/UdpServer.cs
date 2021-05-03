namespace Josing.Net.Sockets
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Text;

    internal class UdpServer
    {
        Socket server;

        Thread receive;

        byte[] buffer = new byte[2048];

        int port;

        Encoding encoding;

        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<UMsg> sendToPools = new Queue<UMsg>();


        public event Action<NetMessage> OnMessage;
        public event Action OnOpen;
        public event Action<string> OnError;
        public bool IsRun { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boardcastIP">格式192.168.x.255</param>
        /// <param name="port"></param>
        public UdpServer(int port, Encoding encoding)
        {
            this.encoding = encoding;
            this.port = port;
        }

        void Receive()
        {
            EndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, port);
            while (true)
            {
                int len = server.ReceiveFrom(buffer, ref iPEndPoint);
                if (len > 0)
                {
                    byte[] buf = new byte[len];
                    Array.Copy(buffer, buf, len);
                    msgPools.Enqueue(new NetMessage(buf, encoding));
                }
            }
        }

        public void Run()
        {
            if (!IsRun)
            {
                try
                {
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    server.Bind(new IPEndPoint(IPAddress.Any, port));
                    server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                    receive = new Thread(new ThreadStart(Receive));
                    receive.Start();
                    IsRun = true;

                    eventQueue.Enqueue(() => { OnOpen?.Invoke(); });
                }
                catch (Exception e)
                {
                    IsRun = false;
                    eventQueue.Enqueue(() => { OnError?.Invoke(e.Message); });
                }
            }

        }

        public void Update()
        {
            while (sendToPools.Count > 0)
            {
                try
                {
                    UMsg msg = sendToPools.Dequeue();
                    server.SendTo(msg.data, msg.endPoint);
                }
                catch (Exception e) { OnError?.Invoke(e.Message); }
            }
            while (eventQueue.Count > 0)
                eventQueue.Dequeue()();
            while (msgPools.Count > 0)
                OnMessage?.Invoke(msgPools.Dequeue());
        }
        public void Send(byte[] msg, string ip, int port) { sendToPools.Enqueue(new UMsg(msg, ip, port)); }
        public void Send(string msg, string ip, int port) { sendToPools.Enqueue(new UMsg(encoding.GetBytes(msg), ip, port)); }
        public void Close()
        {
            if (receive != null) receive.Abort();
            if (server != null) server.Close();
        }
    }
}


