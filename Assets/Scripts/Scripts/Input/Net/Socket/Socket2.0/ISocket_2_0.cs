using Josing.Net.NetInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.Net
{
    public interface IJNet
    {
        bool isRun { get; }
        bool isError { get; }
        bool isClose { get; }
        IJNet SetParams(JNetParams netParams);
        IJNet Run();
        IJNet Send(string msg, string ip = "", int port = 0);
        IJNet Send(byte[] msg, string ip = "", int port = 0);
        IJNet SendCmd(string msg, string ip = "", int port = 0);
        void Update();
        void Close();
    }

    //Event
    public interface IUDPEvent : NetEvent { }
    public interface ITCPClientEvent : NetEvent
    {
        void OnDisConnected();
        void OnConnected();
    }
    public interface ITCPServerEvent : NetEvent
    {
        void OnConnClient(string ip, int port);
        void OnDisConnected(string msg);
    }

    public sealed class JNetUDPSet : JNetParams
    {
        public JNetUDPSet(EncodingType type, INetEvent netEvent, int port, int length)
        {
            Type = type;
            NetEvent = netEvent;
            Port = port;
            BufferLengh = length;
        }
        public void SetMaxListens(int max) { MaxListens = max; }
        public void SetTcpConnectIp(string ip) { Ip = ip; }
    }

    public sealed class JNetTClientSet : JNetParams
    {
        public JNetTClientSet(EncodingType type, INetEvent netEvent, int port, int length, string connIp)
        {
            Type = type;
            NetEvent = netEvent;
            Port = port;
            BufferLengh = length;
            Ip = connIp;
        }
    }

    public sealed class JNetTServerSet : JNetParams
    {
        public JNetTServerSet(EncodingType type, INetEvent netEvent, int port, int length, int max)
        {
            Type = type;
            NetEvent = netEvent;
            Port = port;
            BufferLengh = length;
            MaxListens = max;
        }
    }
}
