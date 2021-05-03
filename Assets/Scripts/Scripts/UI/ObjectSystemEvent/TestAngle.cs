using Josing.UI.ObjectSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAngle : MonoBehaviour
{
    [SerializeField]
    Transform cursor;
    [SerializeField]
    Transform pos1;
    [SerializeField]
    Transform pos2;
    [SerializeField]
    ObjectEvent objectEvent;

    private void Start()
    {
        objectEvent.onClick.AddListener(delegate
        {
            Debug.Log("onClick");
        });
        objectEvent.onLeave.AddListener(delegate
        {
            Debug.Log("onLeave");
        });
        ObjectSystemManager.instance.AddEvent(objectEvent);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.A))
            cursor.position += new Vector3(-1f, 0);
        if (Input.GetKey(KeyCode.D))
            cursor.position += new Vector3(1f, 0);
        if (Input.GetKey(KeyCode.W))
            cursor.position += new Vector3(0, -1f);
        if (Input.GetKey(KeyCode.S))
            cursor.position += new Vector3(0, 1f);
        if (Input.GetKey(KeyCode.LeftArrow))
            cursor.Rotate(0, 0, 1);
        if (Input.GetKey(KeyCode.RightArrow))
            cursor.Rotate(0, 0, -1);
    }

    private void LateUpdate()
    {
        ObjectSystemManager.instance.ObjectDrag(pos1.position, pos2.position, cursor);
    }
}
