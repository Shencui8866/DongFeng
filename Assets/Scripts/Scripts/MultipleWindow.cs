using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine;
using System.Xml.Serialization;


public class MultipleWindow : MonoBehaviour
{
    // [HideInInspector]
    public Rect screenPosition;
    public int index = 0;


    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    private int i = 0;

    void Start()
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        SetWindowLong(GetActiveWindow(), GWL_STYLE, WS_BORDER);
        SetWindowPos(GetActiveWindow(), index, (int)screenPosition.x, (int)screenPosition.y,
          (int)screenPosition.width, (int)screenPosition.height, SWP_SHOWWINDOW);
#endif
    }

}
