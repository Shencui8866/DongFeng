using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Josing.UNet
{
    public class JWebRequest : SingletonBase<JWebRequest>
    {
        Queue<JRequest> webRequestDatas = new Queue<JRequest>();
        JRequest curWebRequest;

        private void Start() { StartCoroutine(UpdateRequest()); }
        public void AddRequest(JRequest webRequest) { webRequestDatas.Enqueue(webRequest); }
        IEnumerator UpdateRequest()
        {
            while(true)
            {
                if (webRequestDatas.Count > 0)
                {
                    curWebRequest = webRequestDatas.Dequeue();
                    curWebRequest.SendWebRequest();
                    while (!curWebRequest.Done)
                    {
                        curWebRequest.OnProgress();
                        yield return null;
                    }
                    curWebRequest.Complete();
                }
                else yield return null;
            }
        }
    }

    public class JRequest
    {
        UnityWebRequest Request;
        Action<JRequest> onComplete;
        UnityWebRequestAsyncOperation AsyncOperation;
        Action<float> onProgress;
        /// <summary>
        /// 原始数据
        /// </summary>
        public byte[] data { get; private set; }
        /// <summary>
        /// 服务器返还文本
        /// </summary>
        public string text { get; private set; }
        /// <summary>
        /// 请求是否完成
        /// </summary>
        public bool Done { get { return AsyncOperation.isDone; } }
        public JRequest(UnityWebRequest request, Action<JRequest> complete)
        {
            Request = request;
            onComplete = complete;
        }
        public JRequest(UnityWebRequest request, Action<JRequest> complete, Action<float> progress) : this(request, complete)
        {
            onProgress = progress;
        }
        /// <summary>
        /// 发送请求，请勿主动调用
        /// </summary>
        public void SendWebRequest() { AsyncOperation = Request.SendWebRequest(); }
        /// <summary>
        /// 请求进度，请勿主动调用
        /// </summary>
        public void OnProgress() { onProgress?.Invoke(Request.uploadProgress); }
        /// <summary>
        /// 请求完成，请勿主动调用
        /// </summary>
        public void Complete()
        {
            if (Request.isNetworkError || Request.isHttpError) LogGUI.Instance.LogWrite(JLogType.Error, "网络错误", Request.error);
            if (Request.downloadHandler != null)
            {
                data = Request.downloadHandler.data;
                text = Request.downloadHandler.text;
            }
            onComplete?.Invoke(this);
        }
    }

    public class JWebRequestException : ApplicationException
    {
        public JWebRequestException(string exception) : base(exception)
        {

        }
    }
}

