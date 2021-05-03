using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show : LanBase
{
    private void Awake()
    {
        /*
         * 1. 接收同步控制命令，地幕led屏等可以用同一个
         * 
         */
    }
    

    protected override void Start()
    {
        base.Start();
        AddCallback("Test",Test);
    }
    void Test(DeviceMessageBase msg)
    {
        Debug.Log(msg.GetField("x1"));
    }
    
    // void xxx(DeviceMessageBase msg)
    // {
    //     for(int i = 0; i < 10; i++)
    //         Debug.Log(msg.GetField(i.ToString()));
    //    
    // }
    

}
