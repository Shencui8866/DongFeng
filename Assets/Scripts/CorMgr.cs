using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorMgr : MonoBehaviour
{
    public static CorMgr instance;
    public CorMgr()
    {
        instance = this;
    }

    public Coroutine startCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }
}
