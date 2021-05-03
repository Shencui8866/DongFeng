// ========================================================
// 作者：wang 
// 创建时间：2020-09-07 19:30:38
// 版 本：1.0
// ========================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class PointSwitch : MonoBehaviour
{
    [SerializeField] ButtonPressEffect pressEffect;
    [SerializeField] private Color c_unselect = Color.white;
    [SerializeField] private Color c_select = Color.white;
    [SerializeField] private Sprite s_unselect;
    [SerializeField] private Sprite s_select;
    [SerializeField] private int page = 0;
    public int maxPage;
    [SerializeField] private List<Image> images;

    private void Awake()
    {
        // for (int i = 0; i < images.Count; i++)
        // {
        //     if (0 == i) images[i].color = c_select;
        //     else images[i].color = c_unselect;
        //     if (i >= maxPage)
        //         images[i].gameObject.SetActive(false);
        // }
        for (int i = 0; i < images.Count; i++)
        {
            if (0 == i)
            {
                if (pressEffect == ButtonPressEffect.Color) images[i].color = c_select;
                else images[i].sprite = s_select;
            }
            else
            {
                if (pressEffect == ButtonPressEffect.Color) images[i].color = c_unselect;
                else images[i].sprite = s_unselect;
            }

            if (i >= maxPage)
            {
                images[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetPage(int index)
    {
        page = index;
        for (int i = 0; i < images.Count; i++)
        {
            if (index == i)
            {
                if (pressEffect == ButtonPressEffect.Color) images[i].color = c_select;
                else images[i].sprite = s_select;
            }
            else
            {
                if (pressEffect == ButtonPressEffect.Color) images[i].color = c_unselect;
                else images[i].sprite = s_unselect;
            }
        }
    }

    public void Next()
    {
        page++;
        if (page >= images.Count) page = 0;
        SetPage(page);
    }

    public void Last()
    {
        page--;
        if (page < 0) page = images.Count - 1;
        SetPage(page);
    }

    [ContextMenu("Page1")]
    void Page1()
    {
        SetPage(0);
    }

    [ContextMenu("Page2")]
    void Page2()
    {
        SetPage(1);
    }

    [ContextMenu("Page3")]
    void Page3()
    {
        SetPage(2);
    }

    [ContextMenu("Page4")]
    void Page4()
    {
        SetPage(3);
    }

    [ContextMenu("Page5")]
    void Page5()
    {
        SetPage(4);
    }

    private void OnValidate()
    {
        images.Clear();
        //遍历所有的子物体以及孙物体，并且遍历包含本身
        for (int i = 0; i < GetComponentsInChildren<Image>(true).Length; i++)
        {
            images.Add(GetComponentsInChildren<Image>()[i]);
        }
    }

    public enum ButtonPressEffect
    {
        Color,
        Sprite
    }
}