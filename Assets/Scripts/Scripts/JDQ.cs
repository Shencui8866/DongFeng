using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Josing.Net;
using UnityEngine.UI;
using Josing.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(JDQ))]
public class JDQEditor : Editor
{
    JDQ jdq;
    private void OnEnable()
    {
        jdq = (JDQ)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (jdq.ports[0] == '0') if (GUILayout.Button("打开" + 1, GUILayout.ExpandWidth(true))) { jdq.PortOpen(1); jdq.SendCmd(); }
        if (jdq.ports[0] == '1') if (GUILayout.Button("关闭" + 1, GUILayout.ExpandWidth(true))) { jdq.PortClose(1); jdq.SendCmd(); }

        if (jdq.ports[1] == '0') if (GUILayout.Button("打开" + 2, GUILayout.ExpandWidth(true))) { jdq.PortOpen(2); jdq.SendCmd(); }
        if (jdq.ports[1] == '1') if (GUILayout.Button("关闭" + 2, GUILayout.ExpandWidth(true))) { jdq.PortClose(2); jdq.SendCmd(); }

        if (jdq.ports[2] == '0') if (GUILayout.Button("打开" + 2, GUILayout.ExpandWidth(true))) { jdq.PortOpen(3); jdq.SendCmd(); }
        if (jdq.ports[2] == '1') if (GUILayout.Button("关闭" + 2, GUILayout.ExpandWidth(true))) { jdq.PortClose(3); jdq.SendCmd(); }

        if (jdq.ports[3] == '0') if (GUILayout.Button("打开" + 3, GUILayout.ExpandWidth(true))) { jdq.PortOpen(4); jdq.SendCmd(); }
        if (jdq.ports[3] == '1') if (GUILayout.Button("关闭" + 3, GUILayout.ExpandWidth(true))) { jdq.PortClose(4); jdq.SendCmd(); }

        if (jdq.ports[4] == '0') if (GUILayout.Button("打开" + 4, GUILayout.ExpandWidth(true))) { jdq.PortOpen(5); jdq.SendCmd(); }
        if (jdq.ports[4] == '1') if (GUILayout.Button("关闭" + 4, GUILayout.ExpandWidth(true))) { jdq.PortClose(5); jdq.SendCmd(); }

        if (jdq.ports[5] == '0') if (GUILayout.Button("打开" + 5, GUILayout.ExpandWidth(true))) { jdq.PortOpen(6); jdq.SendCmd(); }
        if (jdq.ports[5] == '1') if (GUILayout.Button("关闭" + 5, GUILayout.ExpandWidth(true))) { jdq.PortClose(6); jdq.SendCmd(); }

        if (jdq.ports[6] == '0') if (GUILayout.Button("打开" + 6, GUILayout.ExpandWidth(true))) { jdq.PortOpen(7); jdq.SendCmd(); }
        if (jdq.ports[6] == '1') if (GUILayout.Button("关闭" + 6, GUILayout.ExpandWidth(true))) { jdq.PortClose(7); jdq.SendCmd(); }

        if (jdq.ports[7] == '0') if (GUILayout.Button("打开" + 7, GUILayout.ExpandWidth(true))) { jdq.PortOpen(8); jdq.SendCmd(); }
        if (jdq.ports[7] == '1') if (GUILayout.Button("关闭" + 7, GUILayout.ExpandWidth(true))) { jdq.PortClose(8); jdq.SendCmd(); }

        if (jdq.ports[8] == '0') if (GUILayout.Button("打开" + 8, GUILayout.ExpandWidth(true))) { jdq.PortOpen(9); jdq.SendCmd(); }
        if (jdq.ports[8] == '1') if (GUILayout.Button("关闭" + 8, GUILayout.ExpandWidth(true))) { jdq.PortClose(9); jdq.SendCmd(); }

        if (jdq.ports[9] == '0') if (GUILayout.Button("打开" + 9, GUILayout.ExpandWidth(true))) { jdq.PortOpen(10); jdq.SendCmd(); }
        if (jdq.ports[9] == '1') if (GUILayout.Button("关闭" + 9, GUILayout.ExpandWidth(true))) { jdq.PortClose(10); jdq.SendCmd(); }

        if (jdq.ports[10] == '0') if (GUILayout.Button("打开" + 10, GUILayout.ExpandWidth(true))) { jdq.PortOpen(11); jdq.SendCmd(); }
        if (jdq.ports[10] == '1') if (GUILayout.Button("关闭" + 10, GUILayout.ExpandWidth(true))) { jdq.PortClose(11); jdq.SendCmd(); }

        if (jdq.ports[11] == '0') if (GUILayout.Button("打开" + 11, GUILayout.ExpandWidth(true))) { jdq.PortOpen(12); jdq.SendCmd(); }
        if (jdq.ports[11] == '1') if (GUILayout.Button("关闭" + 11, GUILayout.ExpandWidth(true))) { jdq.PortClose(12); jdq.SendCmd(); }
    }
}
#endif

public class JDQ : MonoBehaviour, ITCPClientEvent
{
    public string ip;
    public int port;
    public bool awakeToStart;
    public List<char> ports = new List<char>() { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };


    IJNet client;
    string temp = "A1 01 ";

    private void Start()
    {
        if (awakeToStart)
            Run();
    }

    public void Run()
    {
        if (port == 0) throw new System.Exception("请设置port");
        if (string.IsNullOrEmpty(ip)) throw new System.Exception("请设置ip");

        client = NetCreator.Creator(Josing.Net.SocketType.TcpClient);
        JNetTClientSet netParams = new JNetTClientSet(EncodingType.ASCII, this, port, 1024, ip);
        client.SetParams(netParams);
        client.Run();
    }
    string GenerateCMD(string data)
    {
        string cmd = temp + data + " FF FF";
        List<int> list = new List<int>();
        string[] str = cmd.Split(' ');
        for (int i = 0; i < str.Length; i++)
            list.Add(System.Convert.ToInt32(str[i], 16));

        int num1 = 0;
        for (int i = 0; i < list.Count; i++)
            num1 += list[i];
        string certificate1 = num1.ToString("X2");
        certificate1 = certificate1.Substring(certificate1.Length - 2, 2);
        string certificate2 = (System.Convert.ToInt32(certificate1, 16) * 2).ToString("X2");
        certificate2 = certificate2.Substring(certificate2.Length - 2, 2);

        string result = "CC DD " + cmd + " " + certificate1 + " " + certificate2;
        return result;
    }
    public void Send(string cmd)
    {
        if (client != null)
            client.SendCmd(GenerateCMD(cmd));
        else
            Debug.LogError("client 未初始化完成。");
    }
    public void Close()
    {
        if (client != null)
            client.Close();
        else
            Debug.LogError("client 未初始化完成。");
    }
    private void OnDestroy() { Close(); }


    [ContextMenu("OpenAll")]
    public void OpenAll() { Send("FF FF"); }
    [ContextMenu("CloseAll")]
    public void CloseAll() { Send("00 00"); }
    public void PortOpen(int index)
    {
        ports[16 - index] = '1';
    }
    public void PortClose(int index)
    {
        if (index < 1 || index > 16)
            throw new System.Exception("index 超出范围");
        ports[16 - index] = '0';
    }
    public void SendCmd()
    {
        Send(GetHex());
    }

    string GetHex()
    {
        string cmd = "", cmd1 = "";
        for (int i = 0; i < 8; i++)
        {
            cmd += ports[i];
        }
        for (int i = 8; i < 16; i++)
        {
            cmd1 += ports[i];
        }
        return string.Format("{0}{1}{2}", Convert.ToInt32(cmd, 2).ToString("X2")," ", Convert.ToInt32(cmd1, 2).ToString("X2"));
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A)) OpenAll();
        if (Input.GetKeyUp(KeyCode.C)) CloseAll();
    }

    public void OnMessage(NetMessage msg)
    {

    }

    public void OnRun()
    {
        Debug.Log("OnRun");

    }

    public void OnDisConnected()
    {
        Debug.Log("OnDisConnected");
    }

    public void OnConnected()
    {
        Debug.Log("已连接继电器");
    }

    public void OnError(object error, string errType)
    {
        Debug.Log("OnError -> " + error);
    }
}
