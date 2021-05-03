using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.Net.Sockets;
using System.Text;
using System;

namespace Josing.Net
{
    public abstract class TcpServerAbstract : MonoBehaviour
    {
        [SerializeField]
        string ip;
        [SerializeField]
        int port;
        [SerializeField]
        int listens;
        [SerializeField]
        EncodingType encodingType = EncodingType.ASCII;

        TcpServer tcpServer;

        protected virtual void Awake()
        {
            switch (encodingType)
            {
                case EncodingType.Defualt: tcpServer = new TcpServer(ip, port, listens, Encoding.Default); break;
                case EncodingType.ASCII: tcpServer = new TcpServer(ip, port, listens, Encoding.ASCII); break;
                case EncodingType.UTF8: tcpServer = new TcpServer(ip, port, listens, Encoding.UTF8); break;
            }
            tcpServer.OnConnect += OnConnect;
            tcpServer.OnDisConnect += OnDisConnect;
            tcpServer.OnError += OnError;
            tcpServer.OnMessage += OnMessage;
            tcpServer.OnConnCount += OnConnCount;
            tcpServer.Run();
        }

        protected virtual void Update() { tcpServer.Update(); }
        public void Send(string msg) { tcpServer.Send(msg); }
        public void Send(byte[] msg) { tcpServer.Send(msg); }
        protected virtual void OnDestroy() { tcpServer.Close(); }

        protected abstract void OnError(string e);
        protected abstract void OnMessage(NetMessage data);
        protected abstract void OnConnect(string ip, int port);
        protected abstract void OnDisConnect(string msg);
        protected abstract void OnConnCount(int count);
    }
}

