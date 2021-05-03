using Josing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.IO;

public class JApplication : MonoBehaviour
{
    [SerializeField]
    KeyCode keyCode = KeyCode.Escape;

    public static JIni ini;
    public static JIni Jini { get { if (ini == null) ini = new JIni(JPath.CfgPath); return ini; } }
    public static string DataPath { get { return JPath.DataPath; } }
    public static string VideoPath { get { return JPath.VideoPath; } }
    public static string TexturePath { get { return JPath.TexturePath; } }
    public static string AudioPath { get { return JPath.AudioPath; } }
    public static string LogPath { get { return JPath.LogPath; } }
    public static string CfgPath { get { return JPath.CfgPath; } }

    private void Awake()
    {
        Debug.Log(CfgPath);
    }
    void Start()
    {

    }
    private void OnGUI()
    {
        if (Event.current != null && Event.current.control && Input.GetKeyUp(keyCode))  { Application.Quit(); }
    }
}
