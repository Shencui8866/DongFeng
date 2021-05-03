using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.Net;
using System.Text;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Josing.Net.NetInterface;

namespace Josing.Net.Sockets_2_0
{
    public class UdpClient : IJNet
    {
        Socket m_socket;
        Encoding m_encoding;
        string m_ip;
        int m_port;
        int m_bLength;
        IUDPEvent m_event;
        Queue<UMsg> sendPools = new Queue<UMsg>();
        Queue<Action> eventQueue = new Queue<Action>();
        Queue<NetMessage> msgPools = new Queue<NetMessage>();
        Thread receive;


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
                isError = false;
                m_socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//端口重用
                m_socket.Bind(new IPEndPoint(IPAddress.Any, m_port));
            }
            catch (SocketException e)
            {
                isError = true;
                Error(e.Message, ((SocketError)e.ErrorCode).ToString());
            }
            finally
            {
                if (!isError)
                {
                    isRun = true;
                    eventQueue.Enqueue(() => { m_event.OnRun(); });

                    receive = new Thread(new ThreadStart(Receive));
                    receive.Start();
                }
            }
        }

        void Error(string msg, string type) { eventQueue.Enqueue(() => { m_event.OnError(msg, type); }); }

        void Receive()
        {
            EndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, m_port);
            while (isRun)
            {
                try
                {
                    int len = m_socket.ReceiveFrom(buffer, ref iPEndPoint);
                    if (len > 0)
                    {
                        byte[] data = new byte[len];
                        Array.Copy(buffer, data, len);
                        msgPools.Enqueue(new NetMessage(data, m_encoding, (IPEndPoint)iPEndPoint));
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

        public IJNet SetParams(JNetParams netParams)
        {
            this.m_event = (IUDPEvent)netParams.NetEvent;
            m_bLength = netParams.BufferLengh;
            buffer = new byte[netParams.BufferLengh];
            this.m_port = netParams.Port;
            switch (netParams.Type)
            {
                case EncodingType.Defualt: m_encoding = Encoding.Default; break;
                case EncodingType.ASCII: m_encoding = Encoding.ASCII; break;
                case EncodingType.UTF8: m_encoding = Encoding.UTF8; break;
            }
            return this;
        }

        public IJNet Run()
        {
            ThreadPool.QueueUserWorkItem((o) => { OnRun(); });
            return this;
        }
        public IJNet Send(string msg, string ip, int port)
        {
            sendPools.Enqueue(new UMsg(m_encoding.GetBytes(msg), ip, port));
            return this;
        }
        public IJNet Send(byte[] msg, string ip, int port)
        {
            sendPools.Enqueue(new UMsg(msg, ip, port));
            return this;
        }
        public IJNet SendCmd(string msg, string ip, int port)
        {
            sendPools.Enqueue(new UMsg(NetHelper.HexStrTobyte(msg), ip, port));
            return this;
        }
        public void Update()
        {
            while (eventQueue.Count > 0) eventQueue.Dequeue()();
            if (isError || isClose) Close();
            if (isRun)
            {
                while (msgPools.Count > 0)
                    m_event.OnMessage(msgPools.Dequeue());
                while (sendPools.Count > 0)
                {
                    try
                    {
                        UMsg msg = sendPools.Dequeue();
                        m_socket.SendTo(msg.data, msg.endPoint);
                    }
                    catch (SocketException e)
                    { m_event.OnError(e.Message, ((SocketError)e.ErrorCode).ToString()); }
                }
            }
        }

        public void Close()
        {
            isClose = true;
            if (isDispose) return;
            isDispose = true;
            isRun = false;
            if (receive != null) receive.Abort();
            if (m_socket != null) m_socket.Close();
        }
    }
}

