#if UNITY_STANDALONE
using UnityEngine;
using System.Runtime.InteropServices;
using System;


/// <summary>
/// 文件控制脚本
/// </summary>
public class FileController
{
    /// <summary>
    /// 打开项目
    /// </summary>
    public static void OpenProject(Action<string> onResult)
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "All files (*.*)|*.*";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath.Replace("/", "\\") + "\\Resources"; //默认路径
        pth.title = "打开项目";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            string filepath = pth.file; //选择的文件路径;  
            onResult(filepath);
        }
    }


    /// <summary>
    /// 保存文件项目
    /// </summary>
    public static void SaveProject(Action<string> onResult)
    {
        SaveFileDlg pth = new SaveFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "All files (*.*)|*.*";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath; //默认路径
        pth.title = "保存项目";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(pth))
        {
            string filepath = pth.file; //选择的文件路径;  
            onResult(filepath);
        }
    }
}
#endif
