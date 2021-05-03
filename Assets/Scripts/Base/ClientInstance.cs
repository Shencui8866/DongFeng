using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing;
using Josing.Net.UNetWork;
using UnityEngine.Networking;
using System;

public class ClientInstance : MonoSingletonBase<ClientInstance>
{
    [SerializeField]
    UNetWorkClient workClient;

    public event Action OnConnect;
    public event Action OnDisconnect;

    Dictionary<string, LanBase> callbacks = new Dictionary<string, LanBase>();

    private void Awake()
    {
        workClient.OnConnect += WorkClient_OnConnect;
        workClient.OnDisconnect += WorkClient_OnDisconnect;
    }
    void Start()
    {
        workClient.RegisterHandler(60, ReceiveData);
    }

    private void WorkClient_OnDisconnect()
    {
        OnDisconnect?.Invoke();
    }

    private void WorkClient_OnConnect()
    {
        OnConnect?.Invoke();
    }

    public void SendMessage( DeviceMessageBase messageBase)
    {
        string msg = JsonUtility.ToJson(messageBase);
        workClient.SendToAll(60, msg);
    }

    public void RegisterPage(string pageName, LanBase viewBase)
    {
        try { callbacks.Add(pageName, viewBase); }
        catch (Exception e) { Debug.Log(e.Message); }
    }

    void ReceiveData(NetworkMessage message)
    {
        string data = message.reader.ReadString();
        Debug.Log("ReceiveData -> " + data);

        DeviceMessageBase messageBase = JsonUtility.FromJson<DeviceMessageBase>(data);
        foreach(var page in callbacks)
        {
            if (page.Value != null && page.Value.callbacks.ContainsKey(messageBase.Function))
            {
                Action<DeviceMessageBase> onCallback = null;
                page.Value.callbacks[messageBase.Function]?.Invoke(messageBase);
            }
        }
    }
}

public static class EnumConvert
{
    public static T StringToEnum<T>(string val) where T : Enum
    {
        T e = (T)Enum.Parse(typeof(T), val);
        return e;
    }
}
