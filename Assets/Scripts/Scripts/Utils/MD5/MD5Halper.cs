using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Josing.Utils
{
    public class MD5Halper : MonoBehaviour
    {

        [SerializeField]
        GameObject hint;
        [SerializeField]
        string path;
        [SerializeField]
        string pwd;
        [SerializeField]
        string md5str;

        IEnumerator Start()
        {
            WWW md5 = new WWW(Application.streamingAssetsPath + "/" + path);
            yield return md5;
            if (MD5Decrypt(md5str, md5.text).Contains(pwd))
                hint.SetActive(false);
            else
                hint.SetActive(true);
        }

        void Update()
        {

        }

        /// <summary>
        /// MD5解密 
        /// </summary>
        /// <param name="pToDecrypt">解密字符</param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public string MD5Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return Encoding.Default.GetString(ms.ToArray());
        }
    }
}

