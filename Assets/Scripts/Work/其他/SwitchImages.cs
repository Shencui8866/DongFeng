using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// 外国书图片切换
/// </summary>
public class SwitchImages : MonoBehaviour, IPointerEnterHandler, IDragHandler, IEndDragHandler
{
    private string cfgName = "CFG.txt";

    /// <summary>
    /// 要显示的图片
    /// </summary>
    [SerializeField] private Image showImage;

    [SerializeField] private Button leftBtn;

    [SerializeField] private Button rightBtn;

    //[SerializeField] protected PointSwitch pointSwitch;
    [SerializeField] private bool changeImage;


    /// <summary>
    /// 可以显示的精灵
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// 当前显示图片的索引
    /// </summary>
    [SerializeField] private int index;

    [SerializeField] private Button[] btns;


    private int autoSwitchTime;

    private void Start()
    {
        if (leftBtn != null)
            leftBtn.onClick.AddListener(ClickLeft);
        if (rightBtn != null)
            rightBtn.onClick.AddListener(ClickRight);

        for (int i = 0; i < btns.Length; i++)
        {
            int index = i;
            btns[i].onClick.AddListener(()=> showImage.sprite=sprites[index]);
        }
    }

    private void OnEnable()
    {
        showImage.sprite = sprites[0];
    }


    private void OnDisable()
    {
        showImage.sprite = sprites[0];


        //StopCoroutine(AutoPlay());
    }

    IEnumerator AutoPlay()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSwitchTime);
            ClickRight();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        changeImage = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!changeImage) return;

        if (eventData.delta.x < -30)
        {
            ClickRight();
        }

        if (eventData.delta.x > 30)
        {
            ClickLeft();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        changeImage = true;
    }


    /// <summary>
    /// 左按钮被点击时
    /// </summary>
    private void ClickLeft()
    {
        index--;
        if (index < 0)
        {
            index = sprites.Length - 1;
        }

        showImage.sprite = sprites[index];
        //pointSwitch.SetPage(index);
        changeImage = false;
    }

    /// <summary>
    /// 右按钮被点击时
    /// </summary>
    private void ClickRight()
    {
        index++;

        if (index >= sprites.Length)
        {
            index = 0;
        }

        showImage.sprite = sprites[index];
        changeImage = false;
        // Debug.Log(index);
    }

    // public void OnPageEnter()
    // {
    //     pointSwitch.SetPage(0);
    // }

    
    private void OnValidate()
    {
        // showImage = GetComponent<UnityEngine.UI.Image>();
        // leftBtn = GameObject.Find("LeftBtn").GetComponent<Button>();
        // rightBtn = GameObject.Find("RightBtn").GetComponent<Button>();
    }
}