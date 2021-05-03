using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Josing.JInput
{

    public class JUniScrInput
    {
        public bool TwoTouchReady { get; private set; }
        public bool OneTouchReady { get; private set; }

        float twoOldTouchDis;
        Vector2 oldTouchTwo01;
        Vector2 oldTouchTwo02;
        Vector2 oldTouchOne;

        bool isTouch;

        public JUniScrInput(bool isTouch)
        {
            this.isTouch = isTouch;
        }


        public float UpdateTwoTouch()
        {
            if (!isTouch) return 0;
            if (Input.touchCount != 2)
            {
                TwoTouchReady = false;
                return 0;
            }
            if (!TwoTouchReady)
            {
                oldTouchTwo01 = Input.GetTouch(0).position;
                oldTouchTwo02 = Input.GetTouch(1).position;
                twoOldTouchDis = Vector2.Distance(oldTouchTwo02 - oldTouchTwo01, Vector2.zero);
                TwoTouchReady = true;
                return 0;
            }
            else
            {
                float newTouchDis = Vector2.Distance(Input.GetTouch(1).position - Input.GetTouch(0).position, Vector2.zero);
                oldTouchTwo01 = Input.GetTouch(0).position;
                oldTouchTwo02 = Input.GetTouch(1).position;
                return newTouchDis;
            }
        }

        public Vector2 UpdateOneTouch()
        {
            if (isTouch)
            {
                if (Input.touchCount != 1)
                {
                    OneTouchReady = false;
                    return Vector2.zero;
                }
                if (!OneTouchReady)
                {
                    OneTouchReady = true;
                    oldTouchOne = Input.GetTouch(0).position;
                    return Vector2.zero;
                }
                else
                {
                    Vector2 newVec = oldTouchOne - Input.GetTouch(0).position;
                    oldTouchOne = Input.GetTouch(0).position;
                    return newVec;
                }
            }
            else
            {
                if (!OneTouchReady)
                {
                    OneTouchReady = true;
                    oldTouchOne = Input.mousePosition;
                    return Vector2.zero;
                }
                else
                {
                    Vector2 newVec = oldTouchOne - (Vector2)Input.mousePosition;
                    oldTouchOne = Input.mousePosition;
                    return newVec;
                }
            }
        }
    }
}

