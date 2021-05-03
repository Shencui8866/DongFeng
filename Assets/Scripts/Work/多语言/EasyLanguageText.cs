using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// </summary>
public class EasyLanguageText : MonoBehaviour
{
    public string key;
    private Text text;

    // private void Start()
    // {
    //     text = GetComponent<Text>();
    //     //EasyLanguage.Instance.RegisterELT(this);  弃用
    // }

    public void OnLanguageChange()
    {
        Debug.Log("OnLanguageChange");
        text = GetComponent<Text>();
        text.text = EasyLanguage.Instance.GetTextKey(key);
    }
}