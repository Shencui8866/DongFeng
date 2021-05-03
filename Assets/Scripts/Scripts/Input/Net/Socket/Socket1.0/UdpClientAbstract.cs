using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.Net
{
    using Sockets;
    public abstract class UdpClientAbstract : MonoBehaviour
    {
        [SerializeField]
        int receivePort;
        [SerializeField]
        protected int sendPort;
        [SerializeField]
        protected string send_to_ip;
        [SerializeField]
        EncodingType encodingType = EncodingType.ASCII;


        UdpClient udpClient;

        protected virtual void Start()
        {
            switch (encodingType)
            {
                case EncodingType.Defualt: udpClient = new UdpClient(receivePort, System.Text.Encoding.Default); break;
                case EncodingType.ASCII: udpClient = new UdpClient(receivePort, System.Text.Encoding.ASCII); break;
                case EncodingType.UTF8: udpClient = new UdpClient(receivePort, System.Text.Encoding.UTF8); break;
            }

            udpClient.OnError += OnError;
            udpClient.OnMessage += OnMessage;
            udpClient.OnOpen += OnOpen;
            udpClient.Run();
        }
        protected virtual void Update() { udpClient.Update(); }
        public void Send(string msg, string ip, int port) { udpClient.Send(msg, ip, port); }
        public void Send(byte[] b, string ip, int port) { udpClient.Send(b, ip, port); }
        public void SendCmd(string msg, string ip, int port) { udpClient.Send(NetHelper.HexStrTobyte(msg), ip, port); }
        protected virtual void OnDestroy() { udpClient.Close(); }

        protected abstract void OnError(string e);
        protected abstract void OnMessage(NetMessage data);
        protected abstract void OnOpen();
    }

}
