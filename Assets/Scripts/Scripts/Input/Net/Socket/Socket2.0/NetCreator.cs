using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Josing.Net.Sockets_2_0;


namespace Josing.Net
{
    public class NetCreator : MonoBehaviour
    {
        [SerializeField]
        int count = 0;

        static bool init = false;
        static List<IJNet> netPool = new List<IJNet>();

        static void InitManager()
        {
            if(FindObjectOfType<NetCreator>() == null)
            {
                GameObject gameObject = new GameObject("NetCreator");
                gameObject.AddComponent<NetCreator>();
                DontDestroyOnLoad(gameObject);
                init = true;
            }
        }

        public static IJNet Creator(SocketType type)
        {
            if (!init) InitManager();

            IJNet inet = null;
            switch (type)
            {
                case SocketType.TcpClient: inet = new TcpClient(); break;
                case SocketType.TcpServer: inet = new TcpServer(); break;
                case SocketType.UDPClient: inet = new UdpClient(); break;
                case SocketType.UDPServer: inet = new UdpServer(); break;
            }
            netPool.Add(inet);
            return inet;
        }

        void FixedUpdate()
        {
            count = netPool.Count;
            for (int i = 0; i < netPool.Count; i++)
            {
                netPool[i].Update();
                if (netPool[i].isError || netPool[i].isClose)
                    netPool.Remove(netPool[i]);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < netPool.Count; i++)
                netPool[i].Close();
        }
    }
}

