//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSeek : MonoBehaviour {


    [SerializeField]
    Animator animator;
    [SerializeField]
    float kDuration = 0;
    [SerializeField]
    float m_CurTime = 0f;
    [SerializeField]
    int forward;
    [SerializeField]
    float target;
    [SerializeField]
    bool isUpdate = false;

    bool init = false;

    public float CurTime { get { return m_CurTime; } }

    //Sequence mSequence;

    IEnumerator Start()
    {
        float frameRate = 25f;
        kDuration = animator.runtimeAnimatorController.animationClips[0].length;
        int frameCount = (int)((kDuration * frameRate) + 2);
        animator.Rebind();
        animator.StopPlayback();
        animator.recorderStartTime = 0;
        // 开始记录指定的帧数
        animator.StartRecording(frameCount);

        for (var i = 0; i < frameCount - 1; i++)
        {
            // 记录每一帧
            animator.Update(1.0f / frameRate);
        }
        animator.speed = 0;
        yield return new WaitForEndOfFrame();
        animator.speed = 1f;
        // 完成记录
        animator.StopRecording();

        // 开启回放模式
        animator.StartPlayback();
        init = true;
    }

    private void FixedUpdate()
    {
        if (!init)
            return;
        if (!isUpdate)
            return;
        if(m_CurTime < 0)
        {
            m_CurTime = 0;
            animator.playbackTime = m_CurTime;
            animator.Update(0);
            isUpdate = false;
        }

        //Debug.Log(m_CurTime);
        animator.playbackTime = m_CurTime;
        animator.Update(0);
        m_CurTime += (1 / 50f) * forward;

        if (Math.Abs(m_CurTime - target) < 0.05f)
        {
            isUpdate = false;
        }
    }

    /// <summary>
    /// 外部调用不断更新动画时间，范围-1到1
    /// </summary>
    /// <param name="i"></param>
    void Seek(float seek)
    {
        if (isActiveAndEnabled && init)
        {
            m_CurTime = seek * kDuration;
            animator.playbackTime = seek * kDuration;
            animator.Update(0);
        }
    }
    /// <summary>
    /// 单位 秒
    /// </summary>
    /// <param name="time"></param>
    public void SeekToTarget(float time)
    {
        if (Math.Abs(CurTime - time) < 0.1f)
            return;
        target = time;
        if (target <= 0)
            target = 0;
        if(target > kDuration)
            target = kDuration;
        if (m_CurTime >= target)
            forward = -1;
        else
            forward = 1;
        isUpdate = true;
    }
    /// <summary>
    /// 单位 秒
    /// </summary>
    /// <param name="time"></param>
    public void SeekToNow(float time)
    {
        m_CurTime = time;
        animator.playbackTime = m_CurTime; 
        animator.Update(0);
    }
    /// <summary>
    /// 归一化值
    /// </summary>
    /// <param name="tick"></param>
    public void SeekToAdd(float tick)
    {
        isUpdate = false;
        m_CurTime += tick;
        if (m_CurTime < 0)
        {
            m_CurTime = kDuration;
            animator.playbackTime = m_CurTime;
        }
        else if (m_CurTime > kDuration)
        {
            m_CurTime = 00;
            animator.playbackTime = m_CurTime;
        }
        else
            animator.playbackTime = m_CurTime;
        animator.Update(0);
    }

    private void OnEnable()
    {
        init = false;
        StartCoroutine(Start());
    }

    private void OnValidate()
    {
        animator = GetComponent<Animator>();
    }
}
