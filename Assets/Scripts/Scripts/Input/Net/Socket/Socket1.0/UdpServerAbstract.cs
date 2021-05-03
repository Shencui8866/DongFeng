using Josing.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using System.Net;
using System;

namespace Josing.Net
{
    public abstract class UdpServerAbstract : MonoBehaviour
    {

        [SerializeField]
        int receivePort;
        [SerializeField]
        protected int sendPort;
        [SerializeField]
        protected string send_to_ip;
        [SerializeField]
        protected string boardcastIP;
        [SerializeField]
        EncodingType encodingType = EncodingType.ASCII;


        UdpServer udpServer;

        protected virtual void Start()
        {
            boardcastIP = NetHelper.GetGateway(true);

            switch (encodingType)
            {
                case EncodingType.Defualt: udpServer = new UdpServer(receivePort, System.Text.Encoding.Default); break;
                case EncodingType.ASCII: udpServer = new UdpServer(receivePort, System.Text.Encoding.ASCII); break;
                case EncodingType.UTF8: udpServer = new UdpServer(receivePort, System.Text.Encoding.UTF8); break;
            }

            udpServer.OnError += OnError;
            udpServer.OnMessage += OnMessage;
            udpServer.OnOpen += OnOpen;
            udpServer.Run();
        }

        protected virtual void Update() { udpServer.Update(); }
        public void Send(string msg, string ip, int port) { udpServer.Send(msg, ip, port); }
        public void Send(byte[] b, string ip, int port) { udpServer.Send(b, ip, port); }
        public void SendCmd(string msg, string ip, int port) { udpServer.Send(NetHelper.HexStrTobyte(msg), ip, port); }
        public void SendBoardcast(string msg, int port) { udpServer.Send(msg, boardcastIP, port); }
        public void SendBoardcast(byte[] b, int port) { udpServer.Send(b, boardcastIP, port); }
        public void SendCmdBoardcast(string msg, int port) { udpServer.Send(NetHelper.HexStrTobyte(msg), boardcastIP, port); }
        protected virtual void OnDestroy() { udpServer.Close(); }

        protected abstract void OnError(string e);
        protected abstract void OnMessage(NetMessage data);
        protected abstract void OnOpen();

    }
}

