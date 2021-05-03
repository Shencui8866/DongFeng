using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Josing.Net.Sockets_2_0
{
    public class ClientSocket
    {
        public bool Connect { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        Socket client;
        Thread receive;
        Encoding m_encoding;
        int bufferLength;
        Action<string, string> Err;
        Action<NetMessage> Msg;
        SocketError socketError;
        byte[] buffer;

        public ClientSocket(Socket client, Encoding m_encoding, int bufferLength, Action<string, string> err, Action<NetMessage> msg)
        {
            Ip = (client.RemoteEndPoint as IPEndPoint).Address.ToString();
            Port = (client.RemoteEndPoint as IPEndPoint).Port;
            this.client = client;
            this.m_encoding = m_encoding;
            this.bufferLength = bufferLength;
            this.Err = err;
            this.Msg = msg;
            Connect = true;
            buffer = new byte[bufferLength];
        }

        public void StartReceive()
        {
            receive = new Thread(new ThreadStart(Receive));
            receive.Start();
        }

        public void Close()
        {
            Connect = false;
            if (receive.IsAlive)
                receive.Abort();
            if (client != null)
            {
                client.Disconnect(false);
                client.Close();
            }
        }

        public void Send(byte[] msg)
        {
            client.Send(msg);
        }

        void Receive()
        {
            while (Connect)
            {
                try
                {
                    int len = client.Receive(buffer);
                    if (len == 0)
                    {
                        Err("连接中断", null);
                        Connect = false;
                    }
                    else
                    {
                        byte[] data = new byte[len];
                        Array.Copy(buffer, data, len);
                        Msg(new NetMessage(data, m_encoding, (IPEndPoint)client.RemoteEndPoint));
                    }
                }
                catch (SocketException e)
                {
                    Connect = false;
                    Err(e.Message, ((SocketError)e.ErrorCode).ToString());
                }
            }
        }
    }
}
