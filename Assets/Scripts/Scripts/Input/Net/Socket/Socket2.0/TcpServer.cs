using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Net;
using System.Threading;
using Josing.Net.NetInterface;

namespace Josing.Net.Sockets_2_0
{
    public class TcpServer : IJNet
    {
        Socket m_socket;
        Encoding m_encoding;
        int m_port;
        int m_listens = 10;
        int bufferLength;
        ITCPServerEvent m_serverEvent;
        Queue<byte[]> sendPools = new Queue<byte[]>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        List<ClientSocket> sockets = new List<ClientSocket>();
        Thread accpet;

        byte[] buffer;

        bool isDispose;

        public bool isRun { get; private set; } = false;
        public bool isError { get; private set; } = false;
        public bool isClose { get; private set; } = false;

        void OnRun()
        {
            if (m_serverEvent == null)
                throw new NullReferenceException("m_event 没有设置对应参数");
            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(new IPEndPoint(IPAddress.Parse(NetHelper.GetGateway()), m_port));
                m_socket.Listen(m_listens);
            }
            catch (SocketException e)
            {
                Error(e.Message, ((SocketError)e.ErrorCode).ToString());
                isError = true;
            }
            finally
            {
                if(!isError)
                {
                    AddEvent(() => { m_serverEvent.OnRun(); });
                    isRun = true;

                    accpet = new Thread(new ThreadStart(Accpet));
                    accpet.Start();
                }
            }
        }

        void SendUpdate()
        {
            if (sendPools.Count > 0)
            {
                for (int i = 0; i < sockets.Count; i++)
                {
                    byte[] sbffer = sendPools.Dequeue();
                    try { sockets[i].Send(sbffer); }
                    catch (SocketException e) { Error(e.Message, ((SocketError)e.ErrorCode).ToString()); }
                }
            }
        }

        void AddEvent(Action action) { eventQueue.Enqueue(action); }

        void Error(string msg, string type) { AddEvent(() => { m_serverEvent.OnError(msg, type); }); }

        void Accpet()
        {
            while (isRun)
            {
                try
                {
                    Socket client = m_socket.Accept();
                    ClientSocket cs = new ClientSocket(client, m_encoding, bufferLength, Error, (n)=> { msgPools.Enqueue(n); });
                    cs.StartReceive();
                    sockets.Add(cs);

                    IPEndPoint end = (IPEndPoint)client.RemoteEndPoint;
                    AddEvent(() => { m_serverEvent.OnConnClient(end.Address.ToString(), end.Port); });
                }
                catch (SocketException e)
                {
                    isRun = false;
                    Error(e.Message, ((SocketError)e.ErrorCode).ToString());
                }
            }
        }

        public IJNet SetParams(JNetParams netParams)
        {
            this.m_serverEvent = (ITCPServerEvent)netParams.NetEvent;
            buffer = new byte[netParams.BufferLengh];
            this.bufferLength = netParams.BufferLengh;
            this.m_port = netParams.Port;
            m_listens = netParams.MaxListens;
            switch (netParams.Type)
            {
                case EncodingType.Defualt: m_encoding = System.Text.Encoding.Default; break;
                case EncodingType.ASCII: m_encoding = System.Text.Encoding.ASCII; break;
                case EncodingType.UTF8: m_encoding = System.Text.Encoding.UTF8; break;
            }
            return this;
        }

        public IJNet Run()
        {
            ThreadPool.QueueUserWorkItem((o) => { OnRun(); });
            return this;
        }

        public IJNet Send(string msg, string ip = "", int port = 0)
        {
            if (isRun) sendPools.Enqueue(m_encoding.GetBytes(msg));
            return this;
        }

        public IJNet Send(byte[] msg, string ip = "", int port = 0)
        {
            if (isRun) sendPools.Enqueue(msg);
            return this;
        }

        public IJNet SendCmd(string msg, string ip = "", int port = 0)
        {
            if (isRun) sendPools.Enqueue(NetHelper.HexStrTobyte(msg));
            return this;
        }

        public void Update()
        {
            while (eventQueue.Count > 0) eventQueue.Dequeue()();
            if (isError || isClose) Close();
            if (isRun)
            {
                SendUpdate();
                while (msgPools.Count > 0) m_serverEvent.OnMessage(msgPools.Dequeue());
                for (int i = 0; i < sockets.Count; i++)
                {
                    if (!sockets[i].Connect)
                    {
                        m_serverEvent.OnDisConnected(sockets[i].Ip + ":" + sockets[i].Port);
                        sockets[i].Close();
                        sockets.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void Close()
        {
            isClose = true;
            if (isDispose) return;
            isDispose = true;
            isRun = false;
            if (accpet != null) accpet.Abort();
            if (m_socket != null) m_socket.Close();
            for (int i = 0; i < sockets.Count; i++)
                sockets[i].Close();
        }
    }
}

