using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.Net.NetInterface
{
    public interface INetEvent { }
    public interface NetEvent : INetEvent
    {
        void OnRun();
        void OnMessage(NetMessage msg);
        void OnError(object error, string errType);
    }
    public abstract class JNetParams
    {
        public EncodingType Type { get; internal set; }
        public INetEvent NetEvent { get; internal set; }
        public int Port { get; internal set; }
        public int BufferLengh { get; internal set; }
        public string Ip { get; internal set; }
        public int MaxListens { get; internal set; }
    }
}
