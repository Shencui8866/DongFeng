using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CusDisPlay : MonoBehaviour
{
    [SerializeField] int index;
    // Start is called before the first frame update
    void Start()
    {
        if (Display.displays.Length > index)
            Display.displays[index].Activate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
