//#define VPlayer//AV_Pro
#define AV_Pro
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AV_Pro
using RenderHeads.Media.AVProVideo;
#else
using UnityEngine.UI;
using UnityEngine.Video;
#endif

public class VideoMgr : MonoBehaviour
{
    [Header("isMirror")]
    [SerializeField] bool isTest;
    [SerializeField] CanvasGroup cg_dummy;
    [SerializeField] bool isMirror;
    [SerializeField] private float playProgress;
    [SerializeField] float gamePlayTime;
    [SerializeField] private VideoPath videoPath;

#if AV_Pro
    [Header("AV Pro")]
    [SerializeField] private DisplayUGUI _displayUGUI;
    [SerializeField] private MediaPlayer mp1;
    [SerializeField] private MediaPlayer mp2;
    [SerializeField] private MediaPlayer mp3;
    [SerializeField] private MediaPlayer curMP;
    [SerializeField] private List<MediaPlayer> idleMP = new List<MediaPlayer>();
#else
    [Header("VideoPlayer")]
    [SerializeField] CanvasGroup[] cg_images;
    [SerializeField] private RawImage[] rawImages;
    [SerializeField] private VideoPlayer mp1;
    [SerializeField] private VideoPlayer mp2;
    [SerializeField] private VideoPlayer mp3;
    [SerializeField] private VideoPlayer curMP;
    [SerializeField] private List<VideoPlayer> idleMP = new List<VideoPlayer>();
#endif


    [Header("new")]
    [SerializeField] bool waitForOrder;
    [SerializeField] StorySequenceType curReady;
    [SerializeField] StorySequenceType curPlay;
    [SerializeField] float curVideoLength;
    [SerializeField] float curReadyTime;
    [SerializeField] VideoData curReadyVd;
    [SerializeField] VideoData curPlayVd;
    [SerializeField] VideoMgr videoMgr;
    [SerializeField] SelectType defualtParking = SelectType.MP2;

    [SerializeField] private List<VideoData> videoNames = new List<VideoData>();

    public event Action<StorySequenceType, bool, bool, bool> OnVideoLoad;
    public event Action<StorySequenceType, SelectType> OnVideoPlay;
    public event Action<StorySequenceType> OnVideoNotify;
    public event Action<string> OnNotifyVName;
    public event Action OnGamestart;
    public event Action OnGameend;
    public event Action OnCarDrive;
    public event Action OnGameAutoOver;

    Coroutine coroutine;

    public const float WaitFirst = 1f;

    public void SetMgr(List<VideoData> vd)
    {
        videoNames = vd;
    }

    private void Awake()
    {
#if AV_Pro
        idleMP.Add(mp2);
        idleMP.Add(mp3);
        curMP = mp1;
        _displayUGUI._mediaPlayer = mp1;
#else
        idleMP.Add(mp2);
        idleMP.Add(mp3);
        curMP = mp1;
        ShowRawVP(0);
        mp1.prepareCompleted += Mp1_prepareCompleted;
        mp2.prepareCompleted += Mp2_prepareCompleted;
        mp3.prepareCompleted += Mp3_prepareCompleted;
#endif
    }
#if !AV_Pro
    private void Mp1_prepareCompleted(VideoPlayer source)
    {
        StartCoroutine(StopVideo(source));
    }
    private void Mp2_prepareCompleted(VideoPlayer source)
    {
        StartCoroutine(StopVideo(source));
    }
    private void Mp3_prepareCompleted(VideoPlayer source)
    {
        StartCoroutine(StopVideo(source));
    }
    IEnumerator StopVideo(VideoPlayer source)
    {
        while(source.time == 0f)
        {
            yield return null;
        }
        if (!curReadyVd.autoPlay)
            source.Pause();
        Debug.LogWarningFormat("source {0}, vName {1}, time {2}", source.name, GetShortVName(source.url), source.time);
    }
#endif
    private void Start()
    {
        ClientInstance.Instance.OnConnect += () =>
        {
            LoadVideoMPUnderGround();
            //if (isTest)
            //    StartCoroutine(PlayVideo());

        };
        //StartCoroutine(PlayVideo());
    }
    void Update()
    {
#if AV_Pro
        if (curMP.Control.IsPlaying())
        {
            playProgress = curMP.Control.GetCurrentTimeMs() / curMP.Info.GetDurationMs();
        }
#else
        if (curMP.isPlaying)
        {
            playProgress = (float)(curMP.time / curMP.length);
        }
#endif

    }
    public void SetSelectType(StorySequenceType sequenceType, SelectType selectType)
    {
        if (sequenceType >= 0)
            videoNames[(int)sequenceType].selectType = selectType;
    }
    public void SetWaitOrder(bool wait)
    {
        waitForOrder = wait;
        gamePlayTime = 0;
    }
    public void Stop(bool stopCur)
    {
        if (coroutine != null) StopCoroutine(coroutine);
#if AV_Pro
        if (stopCur)
            if (curMP.Control != null && curMP.Control.IsPlaying())
                curMP.CloseVideo();
        if (idleMP[1].Control != null && idleMP[1].Control.IsPlaying())
            idleMP[1].CloseVideo();
        if (idleMP[0].Control != null && idleMP[0].Control.IsPlaying())
            idleMP[0].CloseVideo();
#else
        if (stopCur)
            if (curMP.isPlaying)
                curMP.Stop();
        if (idleMP[1].isPlaying)
            idleMP[1].Stop();
        if (idleMP[0].isPlaying)
            idleMP[0].Stop();
#endif

    }
    public void Relax()
    {
        StopAllCoroutines();
        if (videoPath == VideoPath.后视镜屏)
            cg_dummy.alpha = 1;
    }
    public void CarDrive()
    {
        Stop(false);
        StopAllCoroutines();
        coroutine = StartCoroutine(PlayVideo());
    }
    public void CarDriveFollow()
    {

    }
    public IEnumerator PlayVideo()
    {
        gamePlayTime = 0;
        OnCarDrive?.Invoke();
        yield return null;
        yield return null;
        SetReady(StorySequenceType.车辆出库);
        SetPlay(StorySequenceType.闲置);
        LoadVideoMP(curReady, false, false);
        //车辆出库
#if AV_Pro
        yield return new WaitForSeconds(WaitFirst);
        yield return new WaitForSeconds((curMP.Info.GetDurationMs() - curMP.Control.GetCurrentTimeMs()) / 1000);
#else
        Debug.Log(curMP.length);
        Debug.Log(curMP.time);
        Debug.Log(curMP.length - curMP.time);
        if (curMP.length - curMP.time < WaitFirst)
            yield return new WaitForSeconds(WaitFirst + 0.5f);
        yield return new WaitForSeconds((float)(curMP.length - curMP.time));
#endif

        SetPlay(StorySequenceType.车辆出库);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.用户上车);// ready
        LoadVideoMP(curReady, false, false, true);

        //用户上车
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.用户上车);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(2);
        SetReady(StorySequenceType.出发前准备);// ready
        LoadVideoMP(curReady, false, false);

        waitForOrder = true;
        while (waitForOrder)
            yield return null;
#if AV_Pro
        curMP.m_Loop = false;
        yield return new WaitForSeconds((curMP.Info.GetDurationMs() - curMP.Control.GetCurrentTimeMs()) / 1000);
#else
        curMP.isLooping = false;
        yield return new WaitForSeconds((float)(curMP.length - curMP.time));
#endif


        //出发前准备

        SetPlay(StorySequenceType.出发前准备);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.前方拥堵);// ready
        LoadVideoMP(curReady, false, false);
        OnVideoNotify?.Invoke(curPlay);

        //前方拥堵
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.前方拥堵);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.前方拥堵直线);// ready
        LoadVideoMP(curReady, false, false);

        //前方拥堵直线
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.前方拥堵直线);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.前车缓行);// ready
        LoadVideoMP(curReady, false, false);
        OnVideoNotify?.Invoke(curPlay);

        //前车缓行
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.前车缓行);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.前车缓行直线);// ready
        LoadVideoMP(curReady, false, false);

        //前车缓行直线
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.前车缓行直线);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.进入游戏);// ready
        LoadVideoMP(curReady, false, false);
        OnVideoNotify?.Invoke(curPlay);

        //进入游戏
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.进入游戏);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);
        //Debug.Log("视频时长 -> " + curVideoLength);
        //Debug.Log(Time.time);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.游戏结束);// ready
        LoadVideoMP(curReady, false, false);


        //游戏结束
        if (curPlayVd.selectType == SelectType.MP2)
        {
            yield return new WaitForSeconds(curVideoLength - curReadyTime - 2f);
            waitForOrder = true;
            // ready  选择分支
            OnGamestart?.Invoke();
            while (waitForOrder)
            {
                gamePlayTime += Time.deltaTime;
                yield return null;
                if (gamePlayTime > 70)
                {
                    OnGameAutoOver?.Invoke();
                    gamePlayTime = 0;
                }
            }
            OnGameend?.Invoke();
        }
        else
        {
            yield return new WaitForSeconds(curVideoLength - curReadyTime);
        }
        // ready  未选择分支
        //Debug.Log("视频时长 -> " + curVideoLength);
        //Debug.Log(Time.time);
        SetPlay(StorySequenceType.游戏结束);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.用户下车);// ready
        LoadVideoMP(curReady, false, false);

        //用户下车
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.用户下车);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.等待用户下车);// ready
        LoadVideoMP(curReady, false, false, true);
        OnVideoNotify?.Invoke(curPlay);

        //用户下车
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.等待用户下车);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(2);
        SetReady(StorySequenceType.车辆入库);// ready
        LoadVideoMP(curReady, false, false);

        waitForOrder = true;

        while (waitForOrder)
            yield return null;
#if AV_Pro
        curMP.m_Loop = false;
        yield return new WaitForSeconds((curMP.Info.GetDurationMs() - curMP.Control.GetCurrentTimeMs()) / 1000);
#else
        curMP.isLooping = false;
        yield return new WaitForSeconds((float)(curMP.length - curMP.time));
#endif


        //车辆入库
        SetPlay(StorySequenceType.车辆入库);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);

        yield return new WaitForSeconds(curReadyTime);
        SetReady(StorySequenceType.车辆入库循环);// ready
        LoadVideoMP(curReady, false, false, true);

        //用户下车
        yield return new WaitForSeconds(curVideoLength - curReadyTime);
        SetPlay(StorySequenceType.车辆入库循环);// play
        ReadyTime(PlayVideoMP((int)curPlayVd.selectType), curPlay);
    }
    void ReadyTime(float time, StorySequenceType sequenceType)
    {
        curVideoLength = time;
        switch(sequenceType)
        {
            case StorySequenceType.出发前准备:
                curReadyTime = time - 14;
                break;
            case StorySequenceType.前方拥堵直线:
                curReadyTime = time - 14;
                break;
            case StorySequenceType.前车缓行直线:
                curReadyTime = time - 9;
                break;
            case StorySequenceType.用户下车:
                curReadyTime = time - 9;
                break;
            default:
                if (time > 7)
                    curReadyTime = time - 9;
                else
                    curReadyTime = time - 1;
                break;
        }

    }
    void SetReady(StorySequenceType ready)
    {
        if (ready >= 0)
        {
            curReady = ready;
            curReadyVd = videoNames[(int)ready];
        }
    }
    void SetPlay(StorySequenceType play)
    {
        if (play >= 0)
        {
            curPlay = play;
            curPlayVd = videoNames[(int)play];
        }
    }
    public void LoadVideoMP(StorySequenceType sequenceType, bool autoPlay, bool top, bool loop = false)
    {
        OnVideoLoad?.Invoke(curReady, autoPlay, top, loop);
        curReady = sequenceType;
        if (sequenceType < 0) return;
        VideoData vd = videoNames[(int)sequenceType];
        if (videoPath == VideoPath.后视镜屏 && curReady <= StorySequenceType.用户上车)
            cg_dummy.alpha = 1;
#if AV_Pro
        idleMP[0].OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[0], autoPlay);
        idleMP[0].Control.SetLooping(loop);
        Debug.Log(string.Format("预加载 -> {0}", vd[0]));
        if (vd.video.Count > 1)
        {
            idleMP[1].OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[1], autoPlay);
            idleMP[1].Control.SetLooping(loop);
            Debug.Log(string.Format("预加载 -> {0}", vd[1]));
        }
#else
        StartCoroutine(LoadVideoDelay(() =>
        {
            idleMP[0].url = vd[0];
            curReadyVd.autoPlay = autoPlay;
            idleMP[0].isLooping = loop;
            Debug.Log(string.Format("预加载 -> {0}", vd[0]));
        }, 0));
        if (vd.video.Count > 1)
        {
            StartCoroutine(LoadVideoDelay(() =>
            {
                idleMP[1].url = vd[1];
                idleMP[1].isLooping = loop;
                Debug.Log(string.Format("预加载 -> {0}", vd[1]));
            }, 2));
        }
#endif

    }
    IEnumerator LoadVideoDelay(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
    public void LoadVideoMPUnderGround()
    {
        VideoData vd = videoNames[(int)StorySequenceType.车辆入库循环];
#if AV_Pro
        curMP.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[(int)defualtParking], true);
        curMP.Control.SetLooping(true);
        _displayUGUI._mediaPlayer = curMP;
#else
        curMP.url = vd[0];
        curMP.isLooping = true;
        curMP.Play();
        StartCoroutine(WaitForFrameOfVideo());
#endif

    }
    public float PlayVideoMP(int index)
    {
        OnVideoPlay?.Invoke(curPlay, curPlayVd.selectType);
        if(videoPath == VideoPath.后视镜屏)
            if (curReady >= StorySequenceType.用户上车 && curReady <= StorySequenceType.等待用户下车)
            {
                cg_dummy.alpha = 0;
            }
            else
            {
                cg_dummy.alpha = 1;
            }
#if AV_Pro
        Debug.Log("播放完成 -> " + curMP.m_VideoPath);
        idleMP.Add(curMP);
        MediaPlayer mp = null;
        mp = idleMP[index];
        curMP = mp;
        curMP.Play();
        _displayUGUI._mediaPlayer = mp;
        idleMP.RemoveAt(index);
        for (int i = 0; i < idleMP.Count; i++)
            if (i != index)
                idleMP[i].Stop();
        Debug.Log("正在播放 -> " + curMP.m_VideoPath);
        OnNotifyVName?.Invoke(GetShortVName(curPlayVd[index]));
        return curMP.Info.GetDurationMs() / 1000;
#else
        Debug.Log("播放完成 -> " + curMP.url + "  length -> " + curMP.length);
        idleMP.Add(curMP);
        VideoPlayer mp = null;
        mp = idleMP[index];
        curMP = mp;
        curMP.Play();
        StartCoroutine(WaitForFrameOfVideo());
        idleMP.RemoveAt(index);
        for (int i = 0; i < idleMP.Count; i++)
            if (i != index)
                idleMP[i].Stop();
        Debug.Log("正在播放 -> " + curMP.url + "  length -> " + curMP.length);
        OnNotifyVName?.Invoke(GetShortVName(curPlayVd[index]));
        //Debug.Break();
        return (float)curMP.length;
#endif
    }
    string GetShortVName(string name)
    {
        int start = name.LastIndexOf('/') + 1;
        int len = name.Length - start;
        return name.Substring(start, len).Split('.')[0];
    }
#if !AV_Pro
    IEnumerator WaitForFrameOfVideo()
    {
        switch (curMP.name)
        {
            case "vp1":
                ShowRawVP(0);
                break;
            case "vp2":
                ShowRawVP(1);
                break;
            case "vp3":
                ShowRawVP(2);
                break;
        }
        yield return new WaitForEndOfFrame();
    }
    void ShowRawVP(int index)
    {
        for(int i = 0; i < rawImages.Length; i++)
        {
            if (index == i)
                cg_images[i].alpha = 1;
            else
                cg_images[i].alpha = 0;
        }
    }
#endif

    private void OnValidate()
    {
        for (int i = 0; i < videoNames.Count; i++)
        {
            videoNames[i].videoPath = videoPath;
        }
        if (videoMgr)
            videoMgr.SetMgr(videoNames);
    }
}

public enum StorySequenceType
{
    闲置 = -1,
    车辆出库,
    用户上车,
    出发前准备,
    前方拥堵,
    前方拥堵直线,
    前车缓行,
    前车缓行直线,
    进入游戏,
    游戏结束,
    用户下车,
    等待用户下车,
    车辆入库,
    车辆入库循环
}
[Serializable]
public class VideoData
{
    [SerializeField] private string name;
    public bool loop;
    public bool autoPlay = true;
    public VideoPath videoPath;
    public List<string> video = null;
    public SelectType selectType;
    public Action complete;
    public string this[int index]
    {
        get
        {
            if (video.Count <= index || string.IsNullOrEmpty(video[index])) return string.Empty;
            switch (videoPath)
            {
                case VideoPath.地幕:
                    return string.Format("{0}/{1}/{2}", Josing.IO.JPath.VideoPath, "地幕", video[index]);
                case VideoPath.环幕:
                    return string.Format("{0}/{1}/{2}", Josing.IO.JPath.VideoPath, "环幕", video[index]);
                case VideoPath.后视镜屏:
                    return string.Format("{0}/{1}/{2}", Josing.IO.JPath.VideoPath, "后视镜屏", video[index]);
                default:
                    return "";
            }
        }
        set
        {

        }
    }

    public void Reset()
    {
        complete = null;
        autoPlay = true;
        selectType = SelectType.MP1;
    }
}
public enum VideoPath
{
    地幕,
    环幕,
    后视镜屏
}

public enum SelectType
{
    MP1,
    MP2
}
[Serializable]
public class SelectData
{
    public bool isSelectBranch;
    public bool autoPlay = true;
    public Action complete;
    public SelectType selectType;

    public void Reset()
    {
        isSelectBranch = false;
        complete = null;
        autoPlay = true;
        selectType = SelectType.MP1;
    }

    public override string ToString()
    {
        return string.Format("isSelectBranch -> {0}, selectType -> {1}", isSelectBranch, selectType);
    }
}
