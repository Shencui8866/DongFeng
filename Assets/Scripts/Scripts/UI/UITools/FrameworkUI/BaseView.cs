using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
 * 2020 07 by josing
 */

namespace Josing.UI.Framework
{
    public abstract class BaseView : MonoBehaviour
    {
        public virtual void OnFirstLoad()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }

        public virtual void OnResume()
        {

        }

        public virtual void OnFreeze()
        {

        }

        protected static void LoadView(Type view)
        {
            BaseViewFactory.LoadView(view);
        }
        protected static void ExitView(Type view)
        {
            BaseViewFactory.ExitView(view);
        }
    }
}
