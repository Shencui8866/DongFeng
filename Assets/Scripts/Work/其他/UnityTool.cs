using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityTool : MonoBehaviour
{
    /// <summary>
    /// 在层级未知的情况下查找子物体（利用递归）
    /// </summary>
    /// <param name="parentTF">父物体变换组件</param>
    /// <param name="childName">子物体名称</param>
    /// <returns></returns>
    public static Transform GetChild(Transform parentTF, string childName)
    {
        //在子物体中查找
        Transform childTF = parentTF.Find(childName); //find() 利用子物体的名称查找，找不到则返回null
        if (childTF != null) return childTF; //如果他不是空，那么就说明找到了，直接返回

        //如果没有找到，将问题交给子物体继续寻找
        //childTF = GetChild(parentTF.GetChild(0),childName); //再利用GetChild()方法查找，参数就由原来的父物体变成了，它兄弟。再从它兄弟的孩子中找，以此类推（循环）
        int count = parentTF.childCount; //要想知道循环多少次那么就要知道他有多少孩子。
        for (int i = 0; i < count; i++)
        {
            childTF = GetChild(parentTF.GetChild(i), childName);
            if (childTF != null) return childTF;
        }

        return null; //实在没有则返回空
    }
}