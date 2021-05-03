using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.Net.Sockets;
using System.Text;
using System;

namespace Josing.Net
{
    public abstract class TcpClientAbstract : MonoBehaviour
    {
        [SerializeField]
        string ip;
        [SerializeField]
        int port;
        [SerializeField]
        EncodingType encodingType = EncodingType.ASCII;

        protected bool IsRun { get { return tcpClient.isRun; } }

        TcpClient tcpClient;


        protected virtual void Start()
        {
            switch (encodingType)
            {
                case EncodingType.Defualt:
                    tcpClient = new TcpClient(ip, port, Encoding.Default);
                    break;
                case EncodingType.ASCII:
                    tcpClient = new TcpClient(ip, port, Encoding.ASCII);
                    break;
                case EncodingType.UTF8:
                    tcpClient = new TcpClient(ip, port, Encoding.UTF8);
                    break;
            }
            tcpClient.OnConnect += OnConnect;
            tcpClient.OnError += OnError;
            tcpClient.OnMessage += OnMessage;
            tcpClient.OnDisConnect += OnDisConnect;
            tcpClient.Connect();
        }


        protected virtual void Update() { tcpClient.Update(); }

        public void Send(string msg) { tcpClient.Send(msg); }

        public void Send(byte[] msg) { tcpClient.Send(msg); }

        public void SendCmd(string msg) { tcpClient.Send(HexStrTobyte(msg)); }

        protected byte[] HexStrTobyte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }

        protected virtual void OnDestroy() { tcpClient.Close(); }

        protected abstract void OnError(string e);
        protected abstract void OnMessage(NetMessage data);
        protected abstract void OnConnect();
        protected abstract void OnDisConnect();

    }
}

