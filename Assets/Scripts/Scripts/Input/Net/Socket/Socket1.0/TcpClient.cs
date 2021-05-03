
namespace Josing.Net.Sockets
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Text;

    internal class TcpClient
    {
        Socket client;

        Thread receive;

        IPEndPoint endPoint;

        byte[] buffer = new byte[2048];

        Encoding encoding;

        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<UMsg> sendToPools = new Queue<UMsg>();

        public event Action<NetMessage> OnMessage;
        public event Action OnConnect;
        public event Action OnDisConnect;
        public event Action<string> OnError;
        public bool isRun { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="encoding"></param>
        public TcpClient(string ip, int port, Encoding encoding)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            this.encoding = encoding;
        }

        public void Connect()
        {
            NetHelper.Ping(endPoint.Address.ToString(), (x) =>
            {
                if (x) Run();
                else OnError?.Invoke("该地址无法连通" + endPoint.Address.ToString());
            });
        }

        void Run()
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(endPoint);

                eventQueue.Enqueue(() => { OnConnect?.Invoke(); });

                receive = new Thread(new ThreadStart(Receive));
                receive.Start();

                isRun = true;
            }
            catch (Exception e)
            {
                eventQueue.Enqueue(() => { OnError?.Invoke(e.Message); });
                isRun = false;
            }
        }

        void Receive()
        {
            if (OnMessage == null)
                throw new NullReferenceException("OnMessage is null");

            while (isRun)
            {
                int len = client.Receive(buffer);
                if(len > 0)
                {
                    byte[] buf = new byte[len];
                    Array.Copy(buffer, buf, len);
                    msgPools.Enqueue(new NetMessage(buf, encoding));
                }
                else
                {
                    eventQueue.Enqueue(() => { OnDisConnect?.Invoke(); });
                    isRun = false;
                }
            }
        }

        public void Send(string msg) { sendToPools.Enqueue(new UMsg(encoding.GetBytes(msg))); }

        public void Send(byte[] msg) { sendToPools.Enqueue(new UMsg(msg)); }

        public void Update()
        {
            while (eventQueue.Count > 0)
                eventQueue.Dequeue()();
            while (sendToPools.Count > 0)
            {
                try { client.Send(sendToPools.Dequeue().data); }
                catch(Exception e) { OnError(e.Message); }
            }
            while (msgPools.Count > 0)
                OnMessage(msgPools.Dequeue());
        }

        public void Close()
        {
            if (receive != null) receive.Abort();
            if (client != null) client.Close();
        }
    }
}


