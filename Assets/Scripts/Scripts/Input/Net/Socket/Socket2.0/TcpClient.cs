using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
using Josing.Net.NetInterface;

namespace Josing.Net.Sockets_2_0
{
    public class TcpClient : IJNet
    {
        Socket m_socket;
        Encoding m_encoding;
        string m_ip;
        int m_port;
        ITCPClientEvent m_event;
        Queue<byte[]> sendPools = new Queue<byte[]>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Thread receive;
        Thread sendMsg;

        byte[] buffer;

        bool isDispose;

        public bool isRun { get; private set; } = false;
        public bool isError { get; private set; } = false;
        public bool isClose { get; private set; } = false;

        void OnRun()
        {
            if (m_event == null)
                throw new NullReferenceException("m_event 没有设置对应参数");
            try
            {
                if (m_socket == null)
                    m_socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
                m_socket.Connect(new IPEndPoint(IPAddress.Parse(m_ip), m_port));
                AddEvent(() => { m_event.OnConnected(); });
            }
            catch (SocketException e)
            {
                Error(e.Message, ((SocketError)e.ErrorCode).ToString());
                isRun = false;
                isError = true;
            }
            finally
            {
                if(!isError)
                {
                    receive = new Thread(new ThreadStart(Receive));
                    receive.Start();

                    isRun = true;
                    AddEvent(() => { m_event.OnRun(); });

                    sendMsg = new Thread(new ThreadStart(SendUpdate));
                    sendMsg.Start();
                }
            }
        }

        void AddEvent(Action action) { eventQueue.Enqueue(action); }

        void Error(string msg, string type) { AddEvent(() => { m_event.OnError(msg, type); }); }

        void Receive()
        {
            while (isRun)
            {
                try
                {
                    int len = m_socket.Receive(buffer);
                    if (len == 0)
                    {
                        AddEvent(() => { m_event.OnDisConnected(); });
                        isRun = false;
                        isError = true;
                    }
                    else
                    {
                        byte[] data = new byte[len];
                        Array.Copy(buffer, data, len);
                        msgPools.Enqueue(new NetMessage(data, m_encoding, (IPEndPoint)m_socket.RemoteEndPoint));
                    }
                }
                catch (SocketException e)
                {
                    Error(e.Message, ((SocketError)e.ErrorCode).ToString());
                    isRun = false;
                    isError = true;
                }
            }
        }

        void SendUpdate()
        {
            while(isRun)
            {
                if (sendPools.Count > 0)
                {
                    try { m_socket.Send(sendPools.Dequeue()); }
                    catch (SocketException e) { Error(e.Message, ((SocketError)e.ErrorCode).ToString()); }
                }
                Thread.Sleep(1);
            }
        }

        public IJNet SetParams(JNetParams netParams)
        {
            this.m_ip = netParams.Ip;
            this.m_event = (ITCPClientEvent)netParams.NetEvent;
            buffer = new byte[netParams.BufferLengh];
            this.m_port = netParams.Port;
            switch (netParams.Type)
            {
                case EncodingType.Defualt: this.m_encoding = System.Text.Encoding.Default; break;
                case EncodingType.ASCII: this.m_encoding = System.Text.Encoding.ASCII; break;
                case EncodingType.UTF8: this.m_encoding = System.Text.Encoding.UTF8; break;
            }
            return this;
        }

        public IJNet Run()
        {
            NetHelper.Ping(m_ip, (x) =>
            {
                if (x) OnRun();
                else
                {
                    Error("地址" + m_ip + "无法连通。", null);
                    isError = true;
                }
            });
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
            if (isRun)
                while (msgPools.Count > 0) m_event.OnMessage(msgPools.Dequeue());
            if (isError || isClose) Close();
        }
        public void Close()
        {
            isClose = true;
            if (isDispose) return;
            isDispose = true;
            isRun = false;
            if (receive != null) receive.Abort();
            if (sendMsg != null) sendMsg.Abort();
            if (m_socket != null) m_socket.Close();
        }
    }
}

