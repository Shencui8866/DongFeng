using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Josing.IO;

public class StartPath : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        JIni ini = new JIni(JPath.CfgPath);
        Debug.Log(string.Format("{0}/{1}", ini.GetString("path"), "DongFeng.exe"));
#if !UNITY_EDITOR
        System.Diagnostics.Process.Start(string.Format("{0}/{1}", ini.GetString("path"), "DongFeng.exe"));
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
