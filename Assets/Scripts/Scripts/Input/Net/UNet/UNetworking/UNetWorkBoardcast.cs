using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



namespace Josing.Net.UNetWork
{
    #if !UNITY_2019
    public class UNetWorkBoardcast : NetworkDiscovery
    { 

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            Debug.Log(fromAddress + "  " + data);
        }
    }
#endif
}

