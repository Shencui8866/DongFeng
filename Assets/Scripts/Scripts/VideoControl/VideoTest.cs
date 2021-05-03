using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Josing;

public class VideoTest : MonoBehaviour
{
    [SerializeField]
    VideoControl videoPlayer;
    [SerializeField]
    float seek;
    [SerializeField]
    Slider slider;

    private void Awake()
    {
        videoPlayer.OnLoadCompleteAndStartPlay += VideoPlayer_OnLoadCompleteAndStartPlay;
        videoPlayer.OnPlay += VideoPlayer_OnPlay;
        videoPlayer.OnEnd += VideoPlayer_OnEnd;
        //videoPlayer.url = "http://localhost/video/061.mp4";
    }

    private void VideoPlayer_OnEnd()
    {
        Debug.Log("VideoPlayer_OnEnd");
    }

    private void VideoPlayer_OnPlay()
    {
        Debug.Log("VideoPlayer_OnPlay");
    }

    private void VideoPlayer_OnLoadCompleteAndStartPlay()
    {
        Debug.Log("VideoPlayer_OnLoadCompleteAndStartPlay");
    }

    public void Down()
    {
        videoPlayer.Pause();
    }
    public void Drag()
    {
        videoPlayer.Seek = slider.value;
    }
    public void End()
    {
        videoPlayer.Play();
    }

    [ContextMenu("Play")]
    void Play() { videoPlayer.LoadVideo("http://localhost/video/061.mp4"); }
    [ContextMenu("Pause")]
    void Pause() { videoPlayer.Pause(); }
    [ContextMenu("Stop")]
    void Stop() { videoPlayer.Stop(); }
    [ContextMenu("Seek")]
    void Seek() { videoPlayer.Seek = seek; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.isPlaying)
            slider.value = videoPlayer.Seek;
    }
}
