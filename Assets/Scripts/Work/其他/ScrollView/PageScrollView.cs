using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PageScrollType
{
    /// <summary>
    /// 水平
    /// </summary>
    Horizontal,

    /// <summary>
    /// 竖直
    /// </summary>
    Vertical
}

/// <summary>
/// 页面滚动
/// </summary>
public class PageScrollView : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public PageScrollType PageScrollType = PageScrollType.Horizontal;
    [SerializeField] protected PointSwitch pointSwitch;
    private ScrollRect scrollRect;
    protected RectTransform content;

    /// <summary>
    /// 页面个数
    /// </summary>
    protected int pageCount;

    /// <summary>
    /// 每个页面对应0-1的值
    /// </summary>
    private float[] pages;

    /// <summary>
    /// 滚动效果时间
    /// </summary>
    public float pollingTime = 0.3f;

    /// <summary>
    /// 计时器
    /// </summary>
    private float timer;

    /// <summary>
    /// 每次开始跳转时跳转的起始页
    /// </summary>
    private float startPos;

    /// <summary>
    /// 当前页
    /// </summary>
    private int currentPage;


    /// <summary>
    /// 是不是开启自动滚动
    /// </summary>
    public bool isAutoScroll;

    /// <summary>
    /// 自动滚动间隔
    /// </summary>
    public float autoScrollTime = 2;

    /// <summary>
    /// 自动滚动计时器
    /// </summary>
    private float autoScrollTimer;

    /// <summary>
    /// 是否正在拖拽
    /// </summary>
    private bool isDragging;

    /// <summary>
    /// 页面改变时的事件
    /// </summary>
    private Action<int> OnPageChange;


    protected virtual void Start()
    {
        scrollRect = transform.GetComponent<ScrollRect>();
        content = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        pageCount = content.childCount;
        pages = new float[pageCount];

        if (isAutoScroll && pageCount == 1)
        {
            Debug.Log("只有一页不需要自动滚动！");
        }

        for (int i = 0; i < pages.Length; i++)
        {
            switch (PageScrollType)
            {
                case PageScrollType.Horizontal:
                    pages[i] = i * (1 / (pageCount - 1f));
                    break;
                case PageScrollType.Vertical:
                    pages[i] = 1 - i * (1 / (pageCount - 1f)); //竖直滚动时verticalNormalizedPosition是从 1->0,正好相反，所以用 1-
                    break;
            }
        }
    }

    protected virtual void Update()
    {
        ListenerAutoScroll();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ScrollToPage(CalculateTheNearestPage());
        autoScrollTimer = 0;
        isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }


    #region private Method

    /// <summary>
    /// 监听自动滚动
    /// </summary>
    private void ListenerAutoScroll()
    {
        if (isDragging)
            return;

        if (isAutoScroll)
        {
            autoScrollTimer += Time.deltaTime;
            if (autoScrollTimer > autoScrollTime)
            {
                currentPage++;
                currentPage %= pageCount;
                ScrollToPage(currentPage);
                autoScrollTimer = 0;
            }
        }
    }

    /// <summary>
    /// 滚动到指定页
    /// </summary>
    /// <param name="page">页面索引</param>
    protected void ScrollToPage(int page)
    {
        StartCoroutine(ListenerScroll());
        currentPage = page;
        pointSwitch.SetPage(page);
        timer = 0;
        switch (PageScrollType)
        {
            case PageScrollType.Horizontal:
                startPos = scrollRect.horizontalNormalizedPosition;
                break;
            case PageScrollType.Vertical:
                startPos = scrollRect.verticalNormalizedPosition;
                break;
        }

        OnPageChange?.Invoke(currentPage);
    }

    /// <summary>
    /// 监听滚动。如果可以滚动，则执行页面滚动的效果
    /// </summary>
    IEnumerator ListenerScroll()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime * 1 / (pollingTime);
            switch (PageScrollType)
            {
                case PageScrollType.Horizontal:
                    scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPos, pages[currentPage], timer);
                    break;
                case PageScrollType.Vertical:
                    scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, pages[currentPage], timer);
                    break;
            }

            if (timer >= 1)
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// 计算拖拽页面时距离最近的页面
    /// </summary>
    /// <returns></returns>
    private int CalculateTheNearestPage()
    {
        int minPage = 0;
        for (int i = 1; i < pages.Length; i++)
        {
            switch (PageScrollType)
            {
                case PageScrollType.Horizontal:
                    if (Mathf.Abs(pages[i] - scrollRect.horizontalNormalizedPosition) <
                        Mathf.Abs(pages[minPage] - scrollRect.horizontalNormalizedPosition))
                    {
                        minPage = i;
                    }

                    break;
                case PageScrollType.Vertical:

                    if (Mathf.Abs(pages[i] - scrollRect.verticalNormalizedPosition) <
                        Mathf.Abs(pages[minPage] - scrollRect.verticalNormalizedPosition))
                    {
                        minPage = i;
                    }

                    break;
            }
        }

        return minPage;
    }

    #endregion
}