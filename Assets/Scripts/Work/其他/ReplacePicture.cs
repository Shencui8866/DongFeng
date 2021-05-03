using System;
using System.Collections;
using System.Collections.Generic;
using Josing.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 可替换图片类
/// </summary>
public class ReplacePicture : MonoBehaviour
{
    [SerializeField] private Image[] images;

    /// <summary>
    /// 如果是图片直接在Texture文件夹中则需要填写
    /// </summary>
    [SerializeField] private string path;

    [SerializeField] private string[] imagePath;
    [SerializeField] private Texture2D[] showTextures;

    private void Start()
    {
        for (int i = 0; i < images.Length; i++)
        {
            StartCoroutine(LoadImage(i));
        }
    }

    IEnumerator LoadImage(int index)
    {
        string url = JPath.TexturePath + "/" + path + "/" + imagePath[index];
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //加载到RawImage的Texture
            // Texture texture = DownloadHandlerTexture.GetContent(request);
            // rawImage.texture = texture;

            //加载到Image的Sprite
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            images[index].sprite = temp;
            showTextures[index] = tex;
        }
    }

    #region 情况1

    [ContextMenu("获取当前Image组件的Sprite")]
    public void GetCurrentImageSprite()
    {
        showTextures = new Texture2D[images.Length];
        imagePath = new string[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            imagePath[i] = images[i].GetComponent<Image>().sprite.name + ".jpg";
            StartCoroutine(TestLoadImage(i));
        }
    }

    IEnumerator TestLoadImage(int index)
    {
        string url = JPath.TexturePath + "/" + path + "/" + imagePath[index];
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //加载到Image的Sprite
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            Sprite temp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            showTextures[index] = tex;
        }
    }

    #endregion
}