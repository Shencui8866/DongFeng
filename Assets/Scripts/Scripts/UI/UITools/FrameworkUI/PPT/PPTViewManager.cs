using Josing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.UI.Framework
{
    public class PPTViewManager : SingletonBase<PPTViewManager>
    {

        [SerializeField]
        BaseView[] baseViews;
        [SerializeField]
        BaseView curView;
        [SerializeField]
        int initView;

        private void Start()
        {
            StartCoroutine(enumerator());
        }

        IEnumerator enumerator()
        {
            yield return null;
            for (int i = 0; i < baseViews.Length; i++)
                if (i != initView)
                    baseViews[i].OnExit();
            curView = baseViews[initView];
            curView.OnEnter();
        }

        public void SetPage(int index)
        {
            curView.OnExit();
            curView = baseViews[index];
            curView.OnEnter();
        }
    }
}

