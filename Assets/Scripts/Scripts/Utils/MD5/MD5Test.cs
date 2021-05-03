using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.Utils;

public class MD5Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string key = MD5Tools.GenerateKey();
        Debug.Log(key.Length);
        string md = MD5Tools.MD5Encrypt("123456", key);
        Debug.Log(md);
        string dd = MD5Tools.MD5Decrypt(md, key);
        Debug.Log(dd);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
