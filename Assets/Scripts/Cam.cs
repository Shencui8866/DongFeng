// ========================================================
// 作者：wang 
// 创建时间：2020-08-07 14:21:10
// 版 本：1.0
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Cam : MonoBehaviour
{
    [SerializeField] private int heiht = 1080;
    [SerializeField] int width = 1920;


    public RawImage image;
    private WebCamTexture camTexture;
    private CanvasGroup _canvasGroup;
    int deviceIndex;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Play();
    }

    private void Play()
    {
        if (camTexture == null)
        {
            if (WebCamTexture.devices.Length > 0)
            {
                camTexture = new WebCamTexture(WebCamTexture.devices[0].name, width, heiht, 30);
            }
            else
            {
                Debug.LogError($"Index out of range");
                return;
            }
        }

        if (camTexture != null && !camTexture.isPlaying)
        {
            camTexture.Play();
            image.texture = camTexture;
        }
        else
        {
            Debug.LogError($"Index out of range{deviceIndex}");
        }
    }


    private void OnDestroy()
    {
        camTexture.Stop();
    }

    [ContextMenu("OpenCamera")]
    public void OpenCamera()
    {
        _canvasGroup.DOFade(1, 1);
    }

    [ContextMenu("CloseCamera")]
    public void CloseCamera()
    {
        _canvasGroup.DOFade(0, 1);
    }
}