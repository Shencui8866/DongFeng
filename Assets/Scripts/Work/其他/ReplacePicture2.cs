using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Josing.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 可替换图片类，使用情况：图片可以左右滑动切换
/// </summary>
public class ReplacePicture2 : MonoBehaviour
{
    private bool isLog = true;
    private Image images;

    /// <summary>
    /// 如果是图片直接在Texture文件夹中则需要填写
    /// </summary>
    [SerializeField] private string path;

    /// <summary>
    /// 图片列表
    /// </summary>
    public List<Texture2D> loadLocalImage = new List<Texture2D>();

    public Sprite[] showSprites;
    public List<string> showSpritesName;

    private string[] imagePath;


    private SwitchImages switchImages;

    private void Awake()
    {
        images = GetComponent<Image>();
        // switchImages = GetComponent<SwitchImages>();
        // switchImages.sprites=new Sprite[showSprites.Length];
        GetLocalImage();

        showSprites = new Sprite[loadLocalImage.Count];
        Debug.Log(transform.name + "showSprites=" + showSprites.Length);
        Texture2DToSprite(loadLocalImage, showSprites);

        // for (int i = 0; i < showSprites.Length; i++)
        // {
        //     switchImages.sprites[i] = showSprites[i];
        // }
    }


    #region 加载图片

    IEnumerator LoadImage(int index)
    {
        string url = JPath.TexturePath + "/" + path + "/" + imagePath[index];
        // Debug.Log(url);
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
            switchImages.sprites[index] = temp;
        }

        images.sprite = switchImages.sprites[0];
    }

    void LoadByIO(int index)
    {
        // float time = Time.time;
        string url = JPath.TexturePath + "/" + path + "/" + imagePath[index];
        FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int) fs.Length);
        fs.Close();
        fs.Dispose();
        fs = null;

        Texture2D t = new Texture2D(1, 1);
        t.LoadImage(bytes);
        //Debug.Log("IO读取图片用时：" + (Time.time-time));


        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        switchImages.sprites[index] = temp;
    }

    /// <summary>
    /// List<Texture2D>转化为 Sprite[]
    /// </summary>
    /// <param name="texture2Ds"></param>
    /// <param name="sprites"></param>
    public void Texture2DToSprite(List<Texture2D> texture2Ds, Sprite[] sprites)
    {
        if (texture2Ds.Count != sprites.Length)
        {
            Debug.LogError("两个数组长度不同，无法转换！");
            return;
        }

        for (int i = 0; i < texture2Ds.Count; i++)
        {
            sprites[i] = Sprite.Create(texture2Ds[i], new Rect(0, 0, texture2Ds[i].width, texture2Ds[i].height),
                new Vector2(0, 0));
        }
    }

    /// <summary>
    /// 加载本地图片文件
    /// </summary>
    Texture2D LoadByIO(string path)
    {
        // float time = Time.time;
        string url = path;
        FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int) fs.Length);
        fs.Close();
        fs.Dispose();
        fs = null;

        Texture2D t = new Texture2D(1, 1);
        t.LoadImage(bytes);
        //Debug.Log("IO读取图片用时：" + (Time.time-time));

        return t;
    }

    #endregion


    /// <summary>
    /// 读取对应文件夹的文件
    /// </summary>
    public void GetLocalImage()
    {
        string url = JPath.TexturePath + "/" + path;
        if (Directory.Exists(url))
        {
            //获取文件信息
            DirectoryInfo direction = new DirectoryInfo(url);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            FileInfo[] jpgFiles = direction.GetFiles("*.JPG", SearchOption.AllDirectories);

            Debug.Log(($"{url}所有文件个数  :  {jpgFiles.Length}"));
            Debug.Log(($"{url}  *.JPG文件个数  :  {jpgFiles.Length}"));


            for (int i = 0; i < files.Length; i++)
            {
                //过滤掉临时文件
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                //print(files[i].Extension); //这个是扩展名
                //获取不带扩展名的文件名
                string name = Path.GetFileNameWithoutExtension(files[i].ToString());
                if (isLog)
                {
                    Debug.Log(files[i].Name);
                    IOUtils.WriteAppend(JPath.LogPath, new string[] {files[i].Name});
                }

                // FileInfo.Name是返回带扩展名的名字 
                //cars.Add((Texture2D)Resources.Load("Car/" + name, typeof(Sprite)));

                showSpritesName.Add(name);
                loadLocalImage.Add(LoadByIO(url + "/" + name + ".JPG"));
            }
        }
    }
}