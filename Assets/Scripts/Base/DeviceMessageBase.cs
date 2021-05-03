using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DeviceMessageBase
{
    public string PageType;
    public string Function;
    public List<KeyValuePair> Data = new List<KeyValuePair>();

    public DeviceMessageBase(string pageType, string function)
    {
        this.PageType = pageType;
        this.Function = function;
    }
    public void AddField(string key, string value)
    {
        KeyValuePair keyValuePair = Data.Find((x) =>
        {
            if (x.key == key)
                return true;
            else
                return false;
        });
        if (keyValuePair != null)
        {
            Debug.LogError("已存在 -> " + key);
            return;
        }
        Data.Add(new KeyValuePair() { key = key, value = value });
    }
    public string GetField(string key)
    {
        KeyValuePair keyValuePair = Data.Find((x) =>
        {
            if (x.key == key)
                return true;
            else
                return false;
        });
        if (keyValuePair != null) return keyValuePair.value;
        else return null;
    }
}
[Serializable]
public class KeyValuePair
{
    public string key;
    public string value;
}