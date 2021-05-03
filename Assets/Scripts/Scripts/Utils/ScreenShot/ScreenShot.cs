using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


namespace Josing.Utils
{
    public class ScreenShot : MonoBehaviour
    {
        [SerializeField]
        ImageType imageType = ImageType.JPG;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine(Shot());
            }
        }

        IEnumerator Shot()
        {
            yield return new WaitForEndOfFrame();
            Texture2D texture2D = new Texture2D(Screen.width, Screen.height);
            texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture2D.Apply();
            byte[] tex;
            if (imageType == ImageType.JPG)
                tex = texture2D.EncodeToJPG();
            else
                tex = texture2D.EncodeToPNG();
            string path = "";
            if (imageType == ImageType.JPG)
                path = Application.streamingAssetsPath + "/" + DateTime.Now.ToString("hh_mm_ss") + ".jpg";
            else
                path = Application.streamingAssetsPath + "/" + DateTime.Now.ToString("hh_mm_ss") + ".png";

            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Write(tex, 0, tex.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
            else
            {
                File.Delete(path);
                FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Write(tex, 0, tex.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }
    }

    public enum ImageType
    {
        PNG,
        JPG
    }
}

