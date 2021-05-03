using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class ScreenVideoControl : LanBase
{
    [SerializeField] private VideoMgr[] videoMgrs;
    [SerializeField] private RawImage[] raw_cover;
    [SerializeField] private CanvasGroup cg_games;
    [SerializeField] private bool isGround;
    [SerializeField] bool isMirror;
    [SerializeField] VideoPlayer[] vp;
    [SerializeField] Texture2D tex_cover;
    [SerializeField] string idle = "I-Cabin LOGO循环动画.mp4";

    Coroutine cor_notify;

    protected override void Start()
    {
        base.Start();
        foreach (var v in raw_cover)
            v.enabled = false;

        ClientInstance.Instance.OnConnect += () =>
        {
            //SendToAllDevicesFunction("RouteNotice");
        };
        AddCallback("流程开始", 流程开始);
        AddCallback("闲置", 闲置);
        AddCallback("大屏关门开始体验", 大屏关门开始体验);
        AddCallback("大屏情景模式选择", 大屏情景模式选择);
        AddCallback("大屏规划路径结果", 大屏规划路径结果);
        AddCallback("大屏变道超车结果", 大屏变道超车结果);
        AddCallback("大屏游戏反馈", 大屏游戏反馈);
        AddCallback("大屏游戏结束", 大屏游戏结束);
        AddCallback("大屏车辆开门", 大屏车辆开门);
        AddCallback("大屏车辆关门", 大屏车辆关门);
        AddCallback("休息", 休息);

        if (!isMirror)
        {
            videoMgrs[0].OnCarDrive += ScreenVideoControl_OnCarDrive;
            videoMgrs[0].OnGameend += ScreenVideoControl_OnGameend;
            videoMgrs[0].OnGamestart += ScreenVideoControl_OnGamestart;
            videoMgrs[0].OnVideoPlay += ScreenVideoControl_OnVideoPlay;
            videoMgrs[0].OnVideoNotify += ScreenVideoControl_OnVideoNotify;
            videoMgrs[0].OnVideoLoad += ScreenVideoControl_OnVideoLoad;
            videoMgrs[0].OnNotifyVName += ScreenVideoControl_OnNotifyVName;
            videoMgrs[0].OnGameAutoOver += ScreenVideoControl_OnGameAutoOver;
        }
        AddCallback("VideoLoad", VideoLoad);
        AddCallback("VideoPlay", VideoPlay);
        AddCallback("GameReStart", GameReStart);
        AddCallback("GameOver", GameOver);
        AddCallback("CarDrive", CarDrive);
        cg_games.DOFade(0, 0f);

        foreach (var v in vp)
            Graphics.Blit(tex_cover, v.targetTexture);

        //DriveCar();

        ClientInstance.Instance.OnConnect += () =>
        {

        };
        //foreach (var v in videoMgrs)
        //    v.LoadVideoMPUnderGround();
        Invoke("LoadDefualt", 0.1f);

        foreach (var v in vp)
        {
            v.url = string.Format("{0}/{1}", Josing.IO.JPath.VideoPath, idle);
            v.isLooping = true;
        }
    }

    private void ScreenVideoControl_OnGameAutoOver()
    {
        SendToAllDevicesFunction("大屏游戏结束");
    }

    private void ScreenVideoControl_OnNotifyVName(string obj)
    {
        Debug.LogWarning("通知播放视频名称 -> " + obj);
        DeviceMessageBase msg = new DeviceMessageBase("", "PlayVideoNotify");
        msg.AddField("vName", obj);
        SendMessageToAll(msg);
    }

    void LoadDefualt()
    {
        foreach (var v in videoMgrs)
            v.LoadVideoMPUnderGround();
    }

    private void ScreenVideoControl_OnCarDrive()
    {
        DeviceMessageBase deviceMessage = new DeviceMessageBase("", "CarDrive");
        SendMessageToAll(deviceMessage);
    }
    private void ScreenVideoControl_OnVideoLoad(StorySequenceType arg1, bool arg2, bool arg3, bool arg4)
    {
        DeviceMessageBase deviceMessage = new DeviceMessageBase("", "VideoLoad");
        deviceMessage.AddField("type", arg1.ToString());
        deviceMessage.AddField("autoPlay", arg2.ToString());
        deviceMessage.AddField("top", arg3.ToString());
        deviceMessage.AddField("loop", arg4.ToString());
        SendMessageToAll(deviceMessage);
    }
    private void ScreenVideoControl_OnVideoPlay(StorySequenceType arg1, SelectType arg2)
    {
        DeviceMessageBase deviceMessage = new DeviceMessageBase("", "VideoPlay");
        deviceMessage.AddField("type", arg1.ToString());
        deviceMessage.AddField("select", arg2.ToString());
        SendMessageToAll(deviceMessage);
    }
    private void ScreenVideoControl_OnVideoNotify(StorySequenceType obj)
    {
        //Debug.LogError("OnVideoNotify  -> " + obj);
        switch (obj)
        {
            case StorySequenceType.出发前准备:
                Debug.LogWarning("Notify RouteNotice");
                SendToAllDevicesFunction("RouteNotice");
                break;
            case StorySequenceType.前方拥堵直线:
                Debug.LogWarning("Notify OvertakeNotice");
                SendToAllDevicesFunction("OvertakeNotice");
                break;
            case StorySequenceType.前车缓行直线:
                Debug.LogWarning("Notify GameStart");
                SendToAllDevicesFunction("GameStart");
                break;
            case StorySequenceType.用户下车:
                Debug.LogWarning("Notify ArriveNotice");
                SendToAllDevicesFunction("ArriveNotice");
                break;
        }
    }
    private void ScreenVideoControl_OnGamestart()
    {
        SendToAllDevicesFunction("GameReStart");
    }
    private void ScreenVideoControl_OnGameend()
    {
        SendToAllDevicesFunction("GameOver");
    }
    void VideoLoad(DeviceMessageBase msg)
    {
        if (!isMirror) return;
        StorySequenceType sequenceType = (StorySequenceType)Enum.Parse(typeof(StorySequenceType), msg.GetField("type"));
        bool auto = bool.Parse(msg.GetField("autoPlay"));
        bool top = bool.Parse(msg.GetField("top"));
        bool loop = bool.Parse(msg.GetField("loop"));
        LoadVideoMP(sequenceType, auto, top, loop);
    }
    void VideoPlay(DeviceMessageBase msg)
    {
        if (!isMirror) return;
        StorySequenceType sequenceType = (StorySequenceType)Enum.Parse(typeof(StorySequenceType), msg.GetField("type"));
        SelectType selectType = (SelectType)Enum.Parse(typeof(SelectType), msg.GetField("select"));
        switch (sequenceType)
        {
            case StorySequenceType.车辆出库:
                SetSelect(sequenceType, selectType);
                break;
            case StorySequenceType.前方拥堵:
                SetSelect(sequenceType, selectType);
                break;
            case StorySequenceType.前车缓行:
                SetSelect(sequenceType, selectType);
                break;
            case StorySequenceType.进入游戏:
                SetSelect(sequenceType, selectType);
                break;
            case StorySequenceType.车辆入库:
                SetSelect(sequenceType, selectType);
                break;
        }
        PlayVideoMP((int)selectType);
    }
    void GameReStart(DeviceMessageBase msg)
    {
        if (cg_games)
        {
            cg_games.alpha = 0;
            cg_games.DOFade(1, 2f);
        }
    }
    void GameOver(DeviceMessageBase msg)
    {
        if (cg_games)
        {
            cg_games.DOFade(0, 2f);
        }
    }
    void CarDrive(DeviceMessageBase msg)
    {
        foreach (var v in vp)
            v.Pause();
        foreach (var v in raw_cover)
            v.enabled = false;
    }

    void 闲置(DeviceMessageBase msg)
    {
        //raw_cover.enabled = true;
        //if (cg_games) cg_games.alpha = 0;
        Debug.LogError("闲置");
        StopCar();
        foreach (var v in videoMgrs)
        {
            v.LoadVideoMPUnderGround();
        }
    }
    void 流程开始(DeviceMessageBase msg)
    {
        DriveCar();
        if (cg_games) cg_games.alpha = 0;
        switch (msg.GetField("msg"))
        {
            case "0":
                isGround = false;
                //Debug.LogError("流程开始:地下到地上");
                videoMgrs[0].SetSelectType(StorySequenceType.车辆出库, SelectType.MP1);
                break;
            case "1":
                isGround = true;
                //Debug.LogError("流程开始:地上到地上");
                videoMgrs[0].SetSelectType(StorySequenceType.车辆出库, SelectType.MP2);
                break;
        }
    }
    void 大屏关门开始体验(DeviceMessageBase msg)
    {
        //Debug.LogError("大屏关门开始体验");
    }
    void 大屏情景模式选择(DeviceMessageBase msg)
    {
        //循环视频结束，下个视频开始播放
        SetOrder(false);
    }
    void 大屏规划路径结果(DeviceMessageBase msg)
    {
        switch (msg.GetField("msg"))
        {
            case "0":
                //Debug.LogError("大屏规划路径结果:否");
                videoMgrs[0].SetSelectType(StorySequenceType.前方拥堵, SelectType.MP1);
                break;
            case "1":
                //Debug.LogError("大屏规划路径结果:是");
                videoMgrs[0].SetSelectType(StorySequenceType.前方拥堵, SelectType.MP2);
                break;
        }
    }
    void 大屏变道超车结果(DeviceMessageBase msg)
    {
        //PlayVideo(StorySequenceType.大屏变道超车结果);
        switch (msg.GetField("msg"))
        {
            case "0":
                //Debug.LogError("大屏变道超车结果:否");
                videoMgrs[0].SetSelectType(StorySequenceType.前车缓行, SelectType.MP1);
                break;
            case "1":
                //Debug.LogError("大屏变道超车结果:是");
                videoMgrs[0].SetSelectType(StorySequenceType.前车缓行, SelectType.MP2);
                break;
        }
    }
    void 大屏游戏反馈(DeviceMessageBase msg)
    {
        switch (msg.GetField("msg"))
        {
            case "0":
                //Debug.LogError("大屏游戏反馈:否");
                videoMgrs[0].SetSelectType(StorySequenceType.进入游戏, SelectType.MP1);
                videoMgrs[0].SetSelectType(StorySequenceType.游戏结束, SelectType.MP1);
                break;
            case "1":
                //Debug.LogError("大屏游戏反馈:是");
                videoMgrs[0].SetSelectType(StorySequenceType.进入游戏, SelectType.MP2);
                videoMgrs[0].SetSelectType(StorySequenceType.游戏结束, SelectType.MP2);
                break;
        }
    }
    void 大屏游戏结束(DeviceMessageBase msg)
    {
        //if (cg_games) cg_games.alpha = 0;
        //Debug.LogError("大屏游戏结束");
        SetOrder(false);
    }
    void 大屏车辆开门(DeviceMessageBase msg)
    {
    }
    void 大屏车辆关门(DeviceMessageBase msg)
    {
        if(isGround)
        {
            //Debug.LogError("大屏车辆关门:从地上到地下");
            videoMgrs[0].SetSelectType(StorySequenceType.车辆入库, SelectType.MP1);
            videoMgrs[0].SetSelectType(StorySequenceType.车辆入库循环, SelectType.MP1);
        }
        else
        {
            //Debug.LogError("大屏车辆关门:从地上到地上");
            videoMgrs[0].SetSelectType(StorySequenceType.车辆入库, SelectType.MP2);
            videoMgrs[0].SetSelectType(StorySequenceType.车辆入库循环, SelectType.MP2);
        }
        SetOrder(false);
    }
    void 休息(DeviceMessageBase msg)
    {
        foreach (var v in raw_cover)
            v.enabled = true;
        Relax();
        foreach (var v in vp)
            v.Play();
    }
    void SetOrder(bool wait)
    {
        foreach(var v in videoMgrs)
        {
            v.SetWaitOrder(wait);
        }
    }
    void SetSelect(StorySequenceType sequenceType, SelectType selectType)
    {
        foreach (var v in videoMgrs)
        {
            v.SetSelectType(sequenceType, selectType);
        }
    }
    void LoadVideoMP(StorySequenceType sequenceType, bool auto, bool top, bool loop)
    {
        foreach (var v in videoMgrs)
        {
            v.LoadVideoMP(sequenceType, auto, top, loop);
        }
    }
    void PlayVideoMP(int index)
    {
        foreach (var v in videoMgrs)
        {
            v.PlayVideoMP(index);
        }
    }
    void DriveCar()
    {
        if (isMirror) return;
        foreach (var v in videoMgrs)
        {
            v.CarDrive();
        }
    }
    void StopCar()
    {
        foreach(var v in videoMgrs)
        {
            v.Stop(true);
        }
    }
    void Relax()
    {
        foreach (var v in videoMgrs)
        {
            v.Relax();
        }
    }
}