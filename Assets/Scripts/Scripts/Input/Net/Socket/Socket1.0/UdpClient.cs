namespace Josing.Net.Sockets
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    internal class UdpClient
    {

        Socket client;

        Thread receive;

        EndPoint point;

        int port;

        Encoding encoding;

        byte[] buffer = new byte[2048];

        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<UMsg> sendToPools = new Queue<UMsg>();

        public event Action<NetMessage> OnMessage;
        public event Action OnOpen;
        public event Action<string> OnError;
        public bool IsRun { get; private set; }

        public UdpClient(int port, Encoding encoding)
        {
            this.encoding = encoding;
            this.port = port;
            point = new IPEndPoint(IPAddress.Any, port);
        }

        void Receive()
        {

            while (true)
            {
                int len = client.ReceiveFrom(buffer, ref point);
                if(len > 0)
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
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//端口重用
                    client.Bind(point);

                    receive = new Thread(new ThreadStart(Receive));
                    receive.Start();

                    eventQueue.Enqueue(() => { OnOpen?.Invoke(); });
                    IsRun = true;
                }
                catch (Exception e)
                {
                    eventQueue.Enqueue(() => { OnError?.Invoke(e.Message); });
                    IsRun = false;
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
                    client.SendTo(msg.data, msg.endPoint);
                }
                catch (Exception e)
                {
                    OnError?.Invoke(e.Message);
                }
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
            if (receive != null)
                receive.Abort();
            if (client != null)
                client.Close();
        }
    }
}

