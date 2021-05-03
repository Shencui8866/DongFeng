using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : LanBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ClientInstance.Instance.OnConnect += () =>
        {
            SendToAllDevicesFunction("Test", "x1", "msg");
   
        };
        /*
         * 1. 作为局域网控制其他主机同步的服务器控制脚本
         * 2. 和外部服务器（http）进行状态同步和通讯
         * 3. 
         */
    }
}
