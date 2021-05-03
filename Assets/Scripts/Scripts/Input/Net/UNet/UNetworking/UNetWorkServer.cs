using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using Josing.Net;

namespace Josing.Net.UNetWork
{
    //#if !UNITY_2019
    public class UNetWorkServer : MonoBehaviour, IUDPEvent
    {
        [SerializeField]
        string ip;
        [SerializeField]
        string boartcastIp;
        [SerializeField]
        int port;
        [SerializeField]
        bool isUse = true;

        IJNet udpClient;

        public static UNetWorkServer instance;

        private void Awake()
        {
            if (!isUse)
            {
                gameObject.SetActive(false);
                return;
            }
            instance = this;

            ip = NetHelper.GetGateway();
            boartcastIp = "255.255.255.255";
            var config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(config, 36);
            NetworkServer.Listen(ip, port);
            NetworkServer.RegisterHandler(UnityEngine.Networking.MsgType.Connect, Connect);
            NetworkServer.RegisterHandler(UnityEngine.Networking.MsgType.Disconnect, DisConnect);
            NetworkServer.RegisterHandler(UnityEngine.Networking.MsgType.Error, Error);
            NetworkServer.RegisterHandler(50, SendAll);

            udpClient = NetCreator.Creator(SocketType.UDPServer);
            JNetUDPSet netParams = new JNetUDPSet(EncodingType.ASCII, this, 19951, 1024);
            udpClient.SetParams(netParams);
            udpClient.Run();
        }

        void Start() { StartCoroutine(SendIp()); }
        public void RegisterHandler(short msgType, NetworkMessageDelegate handler) { NetworkServer.RegisterHandler(msgType, handler); }

        void SendAll(NetworkMessage message)
        {
            short id = message.reader.ReadInt16();
            FLMessageBase fLMessageBase = new FLMessageBase(message.reader.ReadString());
            NetworkServer.SendToAll(id, fLMessageBase);
        }
        void Connect(NetworkMessage message) { Debug.Log("Server Message Connect -> " + message.conn.ToString()); }
        void DisConnect(NetworkMessage message) { Debug.Log("Server Message DisConnect -> " + message.conn.ToString()); }
        void Error(NetworkMessage message) { Debug.Log("Server Message Error -> " + message.conn.ToString()); }



        IEnumerator SendIp()
        {
            while (true)
            {
                udpClient.Send(ip, boartcastIp, 19952);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void OnDestroy()
        {
            if(udpClient != null) udpClient.Close();
        }

        public void OnRun()
        {

        }

        public void OnMessage(NetMessage msg)
        {

        }

        public void OnError(object error, string errType)
        {

        }
    }
//#endif
}

