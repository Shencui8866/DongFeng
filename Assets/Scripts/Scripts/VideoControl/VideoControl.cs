using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

namespace Josing
{
    public class VideoControl : MonoBehaviour
    {
        [SerializeField]
        VideoPlayer videoPlayer;


        float m_seek;
        bool m_loadComplete;
        string path;

        public bool isPlaying
        {
            get
            {
                return videoPlayer.isPlaying;
            }
        }
        public float Seek
        {
            get
            {

                if (videoPlayer != null && videoPlayer.isPlaying)
                {
                    m_seek = (float)(videoPlayer.time / videoPlayer.length);
                    return m_seek;
                }
                else return m_seek;
            }
            set
            {
                m_seek = Mathf.Clamp(value, 0, 1);
                videoPlayer.time = videoPlayer.length * m_seek;
            }
        }
        public float Volume { get { return videoPlayer.GetDirectAudioVolume(0); } }

        public event Action OnLoadCompleteAndStartPlay;
        public event Action OnPlay;
        public event Action OnEnd;

        private void Awake()
        {
            videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
            videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
            videoPlayer.started += VideoPlayer_started;
        }

        public void LoadVideo(string path)
        {
            if (videoPlayer.isPlaying) videoPlayer.Stop();
             m_seek = 0;
            m_loadComplete = false;
            this.path = path;
            videoPlayer.url = path;
            videoPlayer.Play();
        }

        public void LoadVideo(string path, bool loop)
        {
            videoPlayer.isLooping = loop;
            LoadVideo(path);
        }

        public void LoadVideo(string path, bool loop, Texture2D texture2D)
        {
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
                Graphics.Blit(texture2D, videoPlayer.targetTexture);
            LoadVideo(path, loop);
        }

        public void LoadVideo(string path, bool loop, RenderTexture rt)
        {
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
                videoPlayer.targetTexture = rt;
            LoadVideo(path, loop);
        }

        public void LoadVideo(string path, bool loop ,RenderTexture rt , Texture2D texture2D)
        {
            if(videoPlayer.renderMode == VideoRenderMode.RenderTexture)
            {
                videoPlayer.targetTexture = rt;
                Graphics.Blit(texture2D, videoPlayer.targetTexture);
            }
            LoadVideo(path, loop);
        }

        public void SetTexture(Texture2D texture2D)
        {
            Graphics.Blit(texture2D, videoPlayer.targetTexture);
        }

        public void Pause() { videoPlayer.Pause(); }
        public void Play() { if (!string.IsNullOrEmpty(videoPlayer.url) && videoPlayer.isPaused) videoPlayer.Play(); }
        public void Replay() { LoadVideo(path); }

        public void Stop() { videoPlayer.Stop(); }
        public void Stop(Texture2D texture2D)
        {
            videoPlayer.Stop();
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture)
                Graphics.Blit(texture2D, videoPlayer.targetTexture);
        }

        public void SeekTo(float seek) { Seek = seek;  }

        public void SetVolume(float volume)
        {
            for (ushort i = 0; i < videoPlayer.controlledAudioTrackCount; i++)
                videoPlayer.SetDirectAudioVolume(i, Mathf.Clamp(volume + videoPlayer.GetDirectAudioVolume(i), 0, 1));
        }

        private void VideoPlayer_started(VideoPlayer source)
        {
            if (m_loadComplete)
                OnPlay?.Invoke();
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer source)
        {
            m_loadComplete = true;
            OnLoadCompleteAndStartPlay?.Invoke();
        }

        private void VideoPlayer_loopPointReached(VideoPlayer source)
        {
            OnEnd?.Invoke();
        }

        private void OnValidate()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (!videoPlayer) videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
    }
}

