using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleScreen : MonoBehaviour
{
    private void Awake()
    {
    }
    void Start()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate(1920, 1080, 30);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
