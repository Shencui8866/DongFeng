using System;
using System.Collections;
using System.Collections.Generic;
using Josing.IO;
using Microsoft.SqlServer.Server;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    // private string huanMuVideoName = "huanmu.mp4";
    [SerializeField] bool isMirror;
    [SerializeField] bool isTest;
    [SerializeField] CanvasGroup cg_dummy;
    [SerializeField] private DisplayUGUI _displayUGUI;
    [SerializeField] private MediaPlayer mp1;
    [SerializeField] private MediaPlayer mp2;
    [SerializeField] private MediaPlayer mp3;
    [SerializeField] private MediaPlayer curMP;
    [SerializeField] private List<MediaPlayer> idleMP = new List<MediaPlayer>();
    [SerializeField] private SelectData _selectData;
    [SerializeField] private float alertTime;
    [SerializeField] private float playProgress;
    [SerializeField] private bool startBuffer = false;
    [SerializeField] private bool autoExecute;
    [SerializeField] private VideoPath videoPath;
    [SerializeField] private StorySequenceType _storySequenceType = StorySequenceType.闲置;
    [SerializeField] private List<VideoData> videoNames = new List<VideoData>();


    private Action OnMediaPlayComplete;
    public event Action<StorySequenceType, bool> OnLoad;
    public event Action<SelectType> OnPlaySelectType;
    public event Action<bool> OnPlay;
    public event Action<StorySequenceType, float> OnSceneNotify;

    [SerializeField] VideoMgr videoMgr;
    [ContextMenu("SetVideoMgr")]
    void SetVideoMgr()
    {
        videoMgr.SetMgr(videoNames);
    }


    private void Awake()
    {
        idleMP.Add(mp2);
        idleMP.Add(mp3);

        curMP = mp1;
        _displayUGUI._mediaPlayer = mp1;
        if(!isMirror)
        {
            InitMPAction(mp1);
            InitMPAction(mp2);
            InitMPAction(mp3);
        }
    }


    private void Start()
    {
        ClientInstance.Instance.OnConnect += () =>
        {
            if (!isTest) return;
            _storySequenceType = StorySequenceType.车辆出库;
            StartPlayMedia(StorySequenceType.车辆出库, SelectType.MP1, true);
        };
    }

    void InitMPAction(MediaPlayer _mp)
    {
        _mp.Events.AddListener(((arg0, type, code) =>
        {
            switch (type)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    if (arg0.Info.GetDurationMs() > 5000)
                        alertTime = arg0.Info.GetDurationMs() - 5000;
                    else
                        alertTime = arg0.Info.GetDurationMs() - 1000;
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:

                    if (!autoExecute)
                    {
                        OnMediaPlayComplete?.Invoke();
                        OnMediaPlayComplete = null;
                        OnMediaPlayComplete = _selectData.complete; 
                        return;
                    }
                    startBuffer = false;
                    arg0.Stop();

                    if (!_selectData.isSelectBranch)
                        _selectData.selectType = SelectType.MP1;

                    PlayMedia(_selectData.selectType);

                    if (!isMirror)
                        OnSceneNotify?.Invoke(_storySequenceType, curMP.Info.GetDurationMs() / 1000);

                    OnMediaPlayComplete?.Invoke();
                    OnMediaPlayComplete = null;
                    OnMediaPlayComplete = _selectData.complete;
                    autoExecute = _selectData.autoPlay;
                    _selectData.Reset();
                    break;
            }
        }));
    }
    public void SetEnableAutoExecute(bool enable)
    {
        autoExecute = enable;
    }
    public void SetSelectType(SelectData st)
    {
        _selectData = st;
    }

    public void LoadMedia(StorySequenceType sequenceType, bool autoPlay = true)
    {
        _storySequenceType = sequenceType;
        VideoData vd = videoNames[(int) _storySequenceType];

        Debug.LogError(string.Format("预加载 -> {0}", vd[0]));
        if (vd.video.Count > 0)
            LoadVideo(sequenceType, autoPlay);
    }
    public void StartPlayMedia(StorySequenceType sequenceType, SelectType selectType, bool autoPlay = false)
    {
        Stop();

        _storySequenceType = sequenceType;
        int scene = (int)_storySequenceType;
        if (scene > 10) return;
        VideoData vd = videoNames[scene];

        switch (selectType)
        {
            case SelectType.MP1:
                if (vd.video.Count > 0 && !string.IsNullOrEmpty(vd[0]))
                    curMP.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[0], autoPlay);
                else
                    Debug.LogError("vd.video.Count = 0 或 vd.video[0] = null");
                break;
            case SelectType.MP2:
                if (vd.video.Count > 0 && !string.IsNullOrEmpty(vd[1]))
                    curMP.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[1], autoPlay);
                else
                    Debug.LogError("vd.video.Count = 0 或 vd.video[0] = null");
                break;
        }
    }

    public void PlayMedia(bool changeMediaPlayer)
    {
        if (changeMediaPlayer)
        {
            Debug.LogError("播放完成 -> " + curMP.m_VideoPath);
            curMP.Stop();
            idleMP.Add(curMP);
            MediaPlayer mp = idleMP[0];
            curMP = mp;
            curMP.Play();
            _displayUGUI._mediaPlayer = mp;
            idleMP.RemoveAt(0);
        }
        else
            curMP.Play();
        if (!isMirror)
            OnPlay?.Invoke(changeMediaPlayer);

        Debug.LogError("正在播放 -> " + curMP.m_VideoPath);
    }
    public void PlayMedia(SelectType selectType)
    {
        Debug.LogError("播放完成 -> " + curMP.m_VideoPath);
        idleMP.Add(curMP);
        MediaPlayer mp = null;
        switch (selectType)
        {
            case SelectType.MP1:
                mp = idleMP[0];
                curMP = mp;
                curMP.Play();
                _displayUGUI._mediaPlayer = mp;
                idleMP.RemoveAt(0);
                idleMP[1].Stop();
                break;
            case SelectType.MP2:
                mp = idleMP[1];
                curMP = mp;
                curMP.Play();
                _displayUGUI._mediaPlayer = mp;
                idleMP.RemoveAt(1);
                idleMP[0].Stop();
                break;
        }
        if (!isMirror)
            OnPlaySelectType?.Invoke(selectType);
        Debug.LogError("正在播放 -> " + curMP.m_VideoPath);
    }
    public void LoadVideo(StorySequenceType sequenceType, bool autoPlay)
    {
        VideoData vd = videoNames[(int)sequenceType];
        idleMP[0].OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[0], autoPlay);
        Debug.LogError(string.Format("预加载 -> {0}", vd[0]));
        if (vd.video.Count > 1)
        {
            idleMP[1].OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, vd[1], autoPlay);
            Debug.LogError(string.Format("预加载 -> {0}", vd[1]));
        }
    }
    public void Stop()
    {
        if (curMP.Control.IsPlaying())
            curMP.CloseVideo();
        if (idleMP[1].Control.IsPlaying())
            idleMP[1].CloseVideo();
        if (idleMP[0].Control.IsPlaying())
            idleMP[0].CloseVideo();

        idleMP.Clear();
        idleMP.Add(mp2);
        idleMP.Add(mp3);
        curMP = mp1;
        _displayUGUI._mediaPlayer = curMP;
    }

    private void Update()
    {
        if (curMP.Control.IsPlaying())
        {
            playProgress = curMP.Control.GetCurrentTimeMs() / curMP.Info.GetDurationMs();
            if (isMirror) return;
            if (!curMP) return;
            if (!autoExecute) return;
            if (curMP.Control.GetCurrentTimeMs() > alertTime && !startBuffer)
            {
                if ((int) _storySequenceType < 9)
                {
                    startBuffer = true;
                    _storySequenceType++;
                    LoadVideo(_storySequenceType, false);

                    if (!isMirror)
                        OnLoad?.Invoke(_storySequenceType, false);
                }
            }
        }
        if(Input.GetKeyUp(KeyCode.RightArrow))
            curMP.Control.SetPlaybackRate(4);
        if(Input.GetKeyUp(KeyCode.LeftArrow))
            curMP.Control.SetPlaybackRate(1);
    }

    private void OnValidate()
    {
        for (int i = 0; i < videoNames.Count; i++)
        {
            videoNames[i].videoPath = videoPath;
        }
    }
}