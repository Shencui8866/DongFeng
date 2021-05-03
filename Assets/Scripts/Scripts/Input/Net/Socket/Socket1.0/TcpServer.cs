
namespace Josing.Net.Sockets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading;
    using UnityEngine;
    using System.Net;
    using System.Text;

    internal class TcpServer
    {
        Socket server;

        Thread accept;

        int listens;

        byte[] buffer = new byte[2048];

        Encoding encoding;

        IPEndPoint endPoint;

        List<ClientInfo> clients = new List<ClientInfo>();
        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<UMsg> sendToPools = new Queue<UMsg>();

        public event Action<NetMessage> OnMessage;
        public event Action<string, int> OnConnect;
        public event Action<string> OnDisConnect;
        public event Action<string> OnError;
        public event Action<int> OnConnCount;
        public bool isRun { get; private set; }
        public bool isDebug { get; set; }

        public TcpServer(string ip, int port, int listen, Encoding encoding)
        {
            this.listens = listen;
            this.encoding = encoding;
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void Run()
        {
            if (OnError == null)
                throw new NullReferenceException("OnError is null");

            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(endPoint);
                server.Listen(listens);

                accept = new Thread(new ThreadStart(Accept));
                accept.Start();

                isRun = true;
            }
            catch (Exception e)
            {
                eventQueue.Enqueue(() => { OnError(e.Message); });
                isRun = false;
            }
        }

        void Accept()
        {
            if (OnConnect == null)
                throw new NullReferenceException("OnConnect is null");

            while (isRun)
            {
                Socket client = server.Accept();

                ClientInfo clientInfo = new ClientInfo(client, encoding, (n) => { msgPools.Enqueue(n); }, Error);
                clientInfo.StartReceive();

                clients.Add(clientInfo);

                eventQueue.Enqueue(() => { OnConnect((client.RemoteEndPoint as IPEndPoint).Address.ToString(), (client.RemoteEndPoint as IPEndPoint).Port); });
            }

        }

        void Error(string err)
        {
            eventQueue.Enqueue(() => { OnError(err); });
        }

        public void Send(string msg)
        {
            sendToPools.Enqueue(new UMsg(encoding.GetBytes(msg)));
        }

        public void Send(byte[] msg)
        {
            sendToPools.Enqueue(new UMsg(msg));
        }

        public void Close()
        {
            if (accept != null)
                accept.Abort();
            if (server != null)
                server.Close();
            for (int i = 0; i < clients.Count; i++)
                clients[i].Close();
        }

        public void Update()
        {
            while (sendToPools.Count > 0)
            {
                try
                {
                    byte[] send = sendToPools.Dequeue().data;
                    for (int i = 0; i < clients.Count; i++)
                        clients[i].socket.Send(send);
                }
                catch (Exception e)
                {
                    OnError(e.Message);
                }
            }

            while (eventQueue.Count > 0)
                eventQueue.Dequeue()();


            while (msgPools.Count > 0)
                OnMessage(msgPools.Dequeue());

            OnConnCount(clients.Count);

            for (int i = 0; i < clients.Count; i++)
            {
                if(!clients[i].Connect)
                {
                    clients[i].Close();
                    clients.RemoveAt(i);
                    i--;
                }
            }
        }

        class ClientInfo
        {
            public bool Connect { get; private set; }
            public Thread thread;
            public Socket socket;

            Encoding encoding;
            Action<NetMessage> Msg;
            Action<string> Err;
            public ClientInfo(Socket socket, Encoding encoding, Action<NetMessage> msg, Action<string> err)
            {
                Connect = true;
                Msg = msg;
                Err = err;
                this.encoding = encoding;
                this.socket = socket;
            }

            public void StartReceive()
            {
                thread = new Thread(new ThreadStart(Receive));
                thread.Start();
            }

            void Receive()
            {
                byte[] m = new byte[2048];
                Debug.Log("conn");
                while (Connect)
                {
                    int len = socket.Receive(m);
                    if (len > 0)
                    {
                        byte[] buf = new byte[len];
                        Array.Copy(m, buf, len);
                        Msg(new NetMessage(buf, encoding));
                    }
                    else
                    {
                        Connect = false;
                        Err("链接断开:" + socket.RemoteEndPoint.ToString());
                    }
                }
            }

            public void Close()
            {
                if (thread != null)
                    thread.Abort();
                if (socket != null)
                    socket.Close();
            }
        }
    }
}

