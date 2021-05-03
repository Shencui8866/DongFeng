using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    float speedX = 2;
    [SerializeField]
    float speedY = 2;
    [SerializeField]
    float limitMax = 20;
    [SerializeField]
    float limitMin = 20;
    [SerializeField]
    float autoRotSpeed = 0.01f;
    [SerializeField]
    float autoRotCountTimes = 5;
    [SerializeField]
    bool init;


    Vector2 deltaPos;
    Vector2 oldPos;
    float xangle;
    float yangle;
    float countDown;
    Quaternion qRot;

    void Start()
    {
        xangle = 0;
        yangle = 0;
        qRot = Quaternion.Euler(0, autoRotSpeed, 0);
    }


    void Update()
    {

        if (!init)
        {
            if (countDown < 0)
            {
                Quaternion autoQ = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, autoQ, 0.01f);
                transform.localRotation *= qRot;
            }
            else
                countDown -= 0.02f;
        }

        if (Input.GetMouseButton(0)
#if UNITY_IOS && !UNITY_EDITOR
            && Input.touchCount == 1
#endif
            )
        {
            if (init)
            {
                deltaPos = (Vector2)Input.mousePosition - oldPos;
                float y = deltaPos.x;
                float x = deltaPos.y;
                xangle += (x * 0.02f * speedX);
                xangle = Mathf.Clamp(xangle, limitMin, limitMax);
                yangle += (y * 0.02f * -speedY);
                if (yangle > 360)
                    yangle += -360;
                if (yangle < -360)
                    yangle += 360;
                //tY.localRotation = Quaternion.Euler(0, yangle, 0) * Quaternion.Euler(xangle, 0, 0) * Quaternion.identity;
                transform.localRotation = Quaternion.Euler(xangle, yangle, 0) * Quaternion.identity;
                oldPos = Input.mousePosition;
            }
            else
            {
                init = true;
                oldPos = Input.mousePosition;
                yangle = transform.localEulerAngles.y;
                xangle = transform.localEulerAngles.x;
                if (xangle > 180)
                    xangle -= 360;
            }
        }
        else
        {
            if (init)
                countDown = autoRotCountTimes;
            init = false;
            oldPos = Input.mousePosition;
        }
    }

    private void OnEnable()
    {
        countDown = autoRotCountTimes;
    }
}
