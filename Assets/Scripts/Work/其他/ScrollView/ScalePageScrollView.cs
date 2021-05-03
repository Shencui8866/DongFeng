using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalePageScrollView : PageScrollView
{
    /// <summary>
    /// 所有页的Obj
    /// </summary>
    public GameObject[] items;

    public Button[] btns;
    
    
    protected override void Start()
    {
        base.Start();

        items=new GameObject[pageCount];
        for (int i = 0; i < pageCount; i++)
        {
            items[i] = content.GetChild(i).gameObject;
        }


        btns = new Button[pointSwitch.maxPage];
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i] = pointSwitch.transform.GetChild(i).GetComponent<Button>();
        }

        for (int i = 0; i < btns.Length; i++)
        {
            int index = i;
            btns[i].onClick.AddListener(() =>
            {
                ScrollToPage(index);
            });
        }

    }

    protected override void Update()
    {
        base.Update();
    }
}
