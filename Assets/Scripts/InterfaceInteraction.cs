using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(InterfaceInteraction))]
public class InterfaceInteractionEditor : Editor
{
    InterfaceInteraction interaction;
    private void OnEnable()
    {
        interaction = (InterfaceInteraction)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(interaction && Application.isPlaying)
        {
            if(GUILayout.Button("流程开始", GUILayout.ExpandWidth(true)))
            {
                interaction.ReturnDataType = ReturnDataType.流程开始;
                interaction.TestMsg();
            }
            if (GUILayout.Button("大屏情景模式选择", GUILayout.ExpandWidth(true)))
            {
                interaction.ReturnDataType = ReturnDataType.大屏情景模式选择;
                interaction.TestMsg();
            }
            if (GUILayout.Button("大屏车辆关门", GUILayout.ExpandWidth(true)))
            {
                interaction.ReturnDataType = ReturnDataType.大屏车辆关门;
                interaction.TestMsg();
            }
        }
    }
}
#endif

/// <summary>
/// 接口交互类
/// </summary>
public class InterfaceInteraction : LanBase
{
    [SerializeField] private ReturnDataType _returnDataType = ReturnDataType.无反馈;
    [SerializeField] bool notify;
    [SerializeField] bool loopPost;

    public ReturnDataType ReturnDataType { set { _returnDataType = value; } }

    protected override void Start()
    {
        base.Start();
        if (loopPost)
            StartCoroutine(LoopPost());
        if(notify)
        {
            AddCallback("RouteNotice", RouteNotice);
            AddCallback("OvertakeNotice", OvertakeNotice);
            AddCallback("GameStart", GameStart);
            AddCallback("ArriveNotice", ArriveNotice);
            AddCallback("PlayVideoNotify", PlayVideoNotify);
        }
    }
    void RouteNotice(DeviceMessageBase msg)
    {
        StartCoroutine(Post(SystemDefine.routeNotice));
    }
    void OvertakeNotice(DeviceMessageBase msg)
    {
        StartCoroutine(Post(SystemDefine.overtakeNotice));
    }
    void GameStart(DeviceMessageBase msg)
    {
        StartCoroutine(Post(SystemDefine.gameStart));
    }
    void ArriveNotice(DeviceMessageBase msg)
    {
        StartCoroutine(Post(SystemDefine.arriveNotice));
    }
    void PlayVideoNotify(DeviceMessageBase msg)
    {
        StartCoroutine(PostNotifyVName(SystemDefine.updateVideoName, msg.GetField("vName")));
    }

    IEnumerator Post(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(url, "{}");
        UploadHandler upload = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"))
        { contentType = "application/json" };
        webRequest.uploadHandler = upload;
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            Debug.LogErrorFormat("Post Notify: url -> {0}, \nresult -> {1} .", webRequest.url, webRequest.downloadHandler.text);
        }
    }
    IEnumerator PostNotifyVName(string url, string vName)
    {
        //Debug.LogWarning(vName);
        UnityWebRequest webRequest = UnityWebRequest.Post(url, "{}");
        webRequest.SetRequestHeader("videoName", vName);
        UploadHandler upload = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"));
        upload.contentType = "application/json";
        webRequest.uploadHandler = upload;
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError(webRequest.error);
        }
        else
        {
            //Debug.LogErrorFormat("Post Notify: url -> {0}, \nresult -> {1} .", webRequest.url, webRequest.downloadHandler.text);
        }
    }

    IEnumerator LoopPost()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(SystemDefine.serverIp + "car/screenPolling", "{}");
            UploadHandler upload = new UploadHandlerRaw(Encoding.UTF8.GetBytes("{}"))
                {contentType = "application/json"};
            webRequest.uploadHandler = upload;
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                AnalyticalData(webRequest.downloadHandler.text);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    ///  解析轮询接口数据
    /// </summary>
    /// <param name="json">数据</param>
    public void AnalyticalData(string json)
    {
        //示例：{"code":0,"message":"操作成功","model":{"status":"","message":""}}
        var jsonData = JsonMapper.ToObject(json);
        string status = jsonData["model"]["status"].ToString();
        string message = jsonData["model"]["message"].ToString();

        if (string.IsNullOrEmpty(status))
        {
            return;
        }

        var result = (ReturnDataType) Enum.Parse(typeof(ReturnDataType), status);
        if (_returnDataType == result) return;
        else _returnDataType = result;
        Debug.Log(json);
        NetMessage(_returnDataType, message);
    }
    [ContextMenu("TestMsg")]
    public void TestMsg()
    {
        NetMessage(_returnDataType, "0");
    }
    void NetMessage(ReturnDataType returnData, string message = "1")
    {
        switch (returnData)
        {
            case ReturnDataType.闲置:
                SendToAllDevicesFunction("闲置");
                break;
            case ReturnDataType.流程开始:
                SendToAllDevicesFunction("流程开始", "msg", message);
                break;
            case ReturnDataType.大屏关门开始体验:
                //SendToAllDevicesFunction("大屏关门开始体验");
                break;
            case ReturnDataType.大屏情景模式选择:
                SendToAllDevicesFunction("大屏情景模式选择", "msg", message);
                break;
            case ReturnDataType.大屏规划路径结果:
                SendToAllDevicesFunction("大屏规划路径结果", "msg", message);
                break;
            case ReturnDataType.大屏变道超车结果:
                SendToAllDevicesFunction("大屏变道超车结果", "msg", message);
                break;
            case ReturnDataType.大屏游戏反馈:
                SendToAllDevicesFunction("大屏游戏反馈", "msg", message);
                break;
            case ReturnDataType.大屏游戏结束:
                SendToAllDevicesFunction("大屏游戏结束");
                break;
            case ReturnDataType.大屏车辆开门:
                //SendToAllDevicesFunction("大屏车辆开门");
                break;
            case ReturnDataType.大屏车辆关门:
                SendToAllDevicesFunction("大屏车辆关门");
                break;
            case ReturnDataType.休息:
                SendToAllDevicesFunction("休息");
                break;
            default:
                Debug.LogError("错误！！！！！");
                break;
        }
    }
}