using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Josing.Net;
using System;


namespace Josing.Net.UNetWork
{
    //#if !UNITY_2019
    public class UNetWorkClient : MonoBehaviour, IUDPEvent
    {
        [SerializeField]
        string ip;
        [SerializeField]
        int port;
        [SerializeField]
        bool test;
        NetworkClient networkClient;

        IJNet udpClient;

        public event Action OnConnect;
        public event Action OnDisconnect;


        public static UNetWorkClient instance;

        bool init;

        private void Awake()
        {
            instance = this;
            InitNet();
        }

        void InitNet()
        {
            networkClient = new NetworkClient();
            networkClient.RegisterHandler(UnityEngine.Networking.MsgType.Connect, Connect);
            networkClient.RegisterHandler(UnityEngine.Networking.MsgType.Disconnect, DisConnect);
            networkClient.RegisterHandler(UnityEngine.Networking.MsgType.Error, Error);
        }

        private void Start()
        {
            if (!test)
            {
                ip = null;
                udpClient = NetCreator.Creator(SocketType.UDPClient);
                JNetUDPSet netParams = new JNetUDPSet(EncodingType.ASCII, this, 19952, 1024);
                udpClient.SetParams(netParams).Run();
            }
            else
            {
                networkClient.Connect(ip, port);
            }
            init = true;
        }

        public void RegisterHandler(short msgType, NetworkMessageDelegate handler)
        {
            networkClient.RegisterHandler(msgType, handler);
        }

        public void SendToAll(short msgType, string data)
        {
            networkClient.Send(50, new FLMessageSendToAllBase(msgType,  data));
        }



        void Connect(NetworkMessage message)
        {
            Debug.Log("Client Message Connect -> " + message.conn.ToString());
            OnConnect?.Invoke();
        }
        void DisConnect(NetworkMessage message)
        {
            Debug.Log("Client Message DisConnect -> " + message.conn.ToString());
            ip = string.Empty;
            OnDisconnect?.Invoke();
        }
        void Error(NetworkMessage message)
        {
            Debug.Log("Client Message Error -> " + message.conn.ToString());
            ip = string.Empty;
        }

        // Update is called once per frame
        void Update()
        {
        }
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        private void OnApplicationFocus(bool focus)
        {
            if (!init) return;
            if (!focus)
            {
                ip = string.Empty;
                networkClient.Disconnect();
                udpClient.Close();
            }
            else
            {
                udpClient = NetCreator.Creator(SocketType.UDPClient);
                JNetUDPSet netParams = new JNetUDPSet(EncodingType.ASCII, this, 19952, 1024);
                udpClient.SetParams(netParams).Run();
            }
        }
#endif

        public void OnRun()
        {

        }

        public void OnMessage(NetMessage msg)
        {
            if(string.IsNullOrEmpty(ip))
            {
                Debug.Log(msg.Msg);
                ip = msg.Msg;
                networkClient.Connect(ip, port);
            }
        }

        public void OnError(object error, string errType)
        {

        }
    }
//#endif
}

