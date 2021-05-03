using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 需要挂在所有可切换语言的Text的父物体上，最好是在Canvas上
/// </summary>
public class EasyLanguage : MonoBehaviour
{
    public Button btnCH;
    public Button btnEN;

    private static EasyLanguage instance;

    public static EasyLanguage Instance => instance ?? (instance = new EasyLanguage());

    public List<EasyLanguageText> easyLanguageTexts = new List<EasyLanguageText>();

    public Dictionary<string, string> dicElt = new Dictionary<string, string>();

    public enum LanguageList
    {
        CH,
        EN
    }

    [SerializeField] LanguageList currentLanguage;

    private void Awake()
    {
        foreach (EasyLanguageText child in GetComponentsInChildren<EasyLanguageText>(true))
        {
            Instance.easyLanguageTexts.Add(child);
        }

        Debug.Log($"可语言切换的Text个数：{instance.easyLanguageTexts.Count}");

        currentLanguage = LanguageList.CH;
        OnLanguageChange();
        LoadLanguage();

        btnCH.onClick.AddListener(btnCHClick);
        btnEN.onClick.AddListener(btnENClick);
    }


    public void btnCHClick()
    {
        ChangeLanguage(LanguageList.CH);
    }

    public void btnENClick()
    {
        ChangeLanguage(LanguageList.EN);
    }

    public void ChangeLanguage(LanguageList language)
    {
        if (currentLanguage == language) return;
        currentLanguage = language;

        dicElt.Clear();
        LoadLanguage();
        OnLanguageChange();
    }

    private void OnLanguageChange()
    {
        Debug.Log("OnLanguageChange");
        foreach (var elt in easyLanguageTexts)
        {
            elt.OnLanguageChange();
        }
    }

    public string GetTextKey(string key)
    {
        return dicElt[key];
    }

    public void LoadLanguage()
    {
        TextAsset asset = null;

        switch (currentLanguage)
        {
            case LanguageList.CH:
            {
                asset = Resources.Load("Language/Chinese") as TextAsset;
                Stream stream = new MemoryStream(asset.bytes);
                StreamReader streamReader = new StreamReader(stream);

                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] tempStr = line.Split('=');
                    dicElt[tempStr[0]] = tempStr[1];
                }
            }

                break;
            case LanguageList.EN:
            {
                asset = Resources.Load("Language/English") as TextAsset;
                Stream stream = new MemoryStream(asset.bytes);
                StreamReader streamReader = new StreamReader(stream);

                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    string[] tempStr = line.Split('=');
                    dicElt[tempStr[0]] = tempStr[1];
                }
            }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}