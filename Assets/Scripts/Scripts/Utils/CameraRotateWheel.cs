using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateWheel : MonoBehaviour
{
    [SerializeField]
    Camera m_Camera;
    [SerializeField]
    float viewMax;
    [SerializeField]
    float viewMin;


    [Space(30)]
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
    float smoothSpeed = 5;
    [SerializeField]
    bool smooth = false;
    [SerializeField]
    bool init;
    [SerializeField]
    bool isReady;
    [SerializeField]
    float fieldOfView;
    [SerializeField]
    float fieldOfViewSpeed;


    Vector2 deltaPos;
    float xangle;
    float yangle;
    Quaternion qTarget;


    private void Awake()
    {
        ///m_Camera = GetComponent<Camera>();
    }

    void Start()
    {
        xangle = 0;
        yangle = 0;
        fieldOfView = m_Camera.fieldOfView;
    }


    void Update()
    {
        if(isReady)
        {
            m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, fieldOfView, 0.1f);
            if (smooth) transform.localRotation = Quaternion.Lerp(transform.localRotation, qTarget, 0.01f * smoothSpeed);
        }

    }

    public void Ready(bool ready) { isReady = ready; }

    public float SetFieldOfViewTarget(float view)
    {
        fieldOfView = view;
        if (fieldOfView > viewMax) fieldOfView = viewMax;
        if (fieldOfView < viewMin) fieldOfView = viewMin;
        return fieldOfView;
    }

    public float SetFieldOfView(float view)
    {
        fieldOfView += view;
        if (fieldOfView > viewMax) fieldOfView = viewMax;
        if (fieldOfView < viewMin) fieldOfView = viewMin;
        return fieldOfView;
    }

    public void SetRotateTarget(float x, float y, float z = 0)
    {
        xangle = x;
        xangle = Mathf.Clamp(xangle, limitMin, limitMax);
        yangle = y;
        if (yangle > 360) yangle += -360;
        if (yangle < -360) yangle += 360;
        qTarget = Quaternion.Euler(xangle, yangle, z) * Quaternion.identity;
    }

    public void SetRotateTarget(Vector3 rot)
    {
        xangle = rot.x;
        xangle = Mathf.Clamp(xangle, limitMin, limitMax);
        yangle = rot.y;
        qTarget = Quaternion.Euler(xangle, yangle, rot.z) * Quaternion.identity;
    }

    public Vector3 SetRotate(float x, float y, float z = 0)
    {
        xangle += x;
        yangle += y;
        xangle = Mathf.Clamp(xangle, limitMin, limitMax);
        if (yangle > 360) yangle += -360;
        if (yangle < -360) yangle += 360;
        qTarget = Quaternion.Euler(xangle, yangle, z) * Quaternion.identity;
        return qTarget.eulerAngles;
    }

    public void OnBeginDrag(Vector2 pos)
    {
        init = true;
        yangle = transform.localEulerAngles.y;
        xangle = transform.localEulerAngles.x;
    }

    public void OnDrag(Vector2 pos)
    {
        deltaPos = pos;

        float y = deltaPos.x;
        float x = deltaPos.y;
        xangle += (x * 0.02f * speedX);
        xangle = Mathf.Clamp(xangle, limitMin, limitMax);
        yangle += (y * 0.02f * -speedY);
        if (yangle > 360) yangle += -360;
        if (yangle < -360) yangle += 360;
        qTarget = Quaternion.Euler(xangle, yangle, 0) * Quaternion.identity;
    }

    public void OnEndDrag(Vector2 pos)
    {
        deltaPos = Vector2.zero;
        init = false;

        float y = deltaPos.x;
        float x = deltaPos.y;
        xangle += (x * 0.02f * speedX);
        xangle = Mathf.Clamp(xangle, limitMin, limitMax);
        yangle += (y * 0.02f * -speedY);
        if (yangle > 360) yangle += -360;
        if (yangle < -360) yangle += 360;
        qTarget = Quaternion.Euler(xangle, yangle, 0) * Quaternion.identity;
    }
}
