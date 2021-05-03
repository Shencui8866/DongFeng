using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.UI.Framework;
using System;

public class LanBase : BaseView
{
    public Dictionary<string, Action<DeviceMessageBase>> callbacks = new Dictionary<string, Action<DeviceMessageBase>>();
    
    [SerializeField]
    protected string PageName;
    [SerializeField]
    protected bool init;


    protected Coroutine coroutine_delay;


    protected virtual void Start()
    {
        if(string.IsNullOrEmpty(PageName))
        {
            Debug.LogError(name + " PageName是空的，请赋值！");
            return;
        }
        ClientInstance.Instance.RegisterPage(PageName, this);
        init = true;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
    public override void OnExit()
    {
        base.OnExit();
    }

    protected void AddCallback(string funcName, Action<DeviceMessageBase> callback)
    {
        try { callbacks.Add(funcName, callback); }
        catch(Exception e) { Debug.Log(e.Message); }
    }

    


    //-------------------消息发送
    protected void SendToAllDevicesFunction(string func, string key1 = null, string val1 = null, string key2 = null, string val2 = null)
    {
        DeviceMessageBase messageBase1 = new DeviceMessageBase("", func);
        if (!string.IsNullOrEmpty(key1))  messageBase1.AddField(key1, val1);
        if (!string.IsNullOrEmpty(key2)) messageBase1.AddField(key2, val2);
        SendMessageToAll(messageBase1);
    }
    protected void SendMessageToAll(DeviceMessageBase messageBase)
    {
        ClientInstance.Instance.SendMessage( messageBase);
    }

    //-------------------消息发送


    //-------------------延时运行
    protected void StartDelayScene(float time, FordDelayAction delayAction)
    {
        coroutine_delay = StartCoroutine(DelayAction(delayAction, time));
    }

    IEnumerator DelayAction(FordDelayAction delayAction, float time)
    {
        yield return new WaitForSeconds(time);
        delayAction?.Invoke();
    }

    //-------------------延时运行

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(PageName)) PageName = GetType().Name;
    }
#endif
}

public delegate void FordDelayAction();
public class SceneConst
{
    public const string LincoinAPPEnter = "LincoinAPPEnter";
    public const string LincoinAPPExit = "LincoinAPPExit";
    public const string Music = "Music";
    public const string Scene = "Scene";
    public const string SceneDetial = "SceneDetial";
    public const string SceneMedia = "SceneMedia";
}
