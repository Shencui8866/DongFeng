using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Josing.Task
{

    public class TaskQueue : SingletonBase<TaskQueue>
    {
        public bool debug { get { return isDebug; } set { isDebug = value; } }
        static List<JTask> _TaskQueue = new List<JTask>();
        static TaskQueue instance;


        [SerializeField]
        bool isDebug;

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <param name="taskCallback">任务回调</param>
        /// <returns></returns>
        public JTask AddTask(string key, TaskCallback taskCallback)
        {
            JTask taskEntity = null;

            if (HasTask(key))
                Debug.LogError("已经存在 key : " + key);
            else
            {
                taskEntity = new JTask(0, key, taskCallback);
                _TaskQueue.Add(taskEntity);
            }
            return taskEntity;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <param name="taskCallback">任务回调</param>
        /// <param name="delay">任务延时，单位秒</param>
        /// <returns></returns>
        public JTask AddTask(string key, float delay, TaskCallback taskCallback)
        {
            JTask taskEntity = null;
            if (HasTask(key))
             Debug.LogError("已经存在 key : " + key);
            else
            {
                taskEntity = new JTask(delay, key, taskCallback);
                _TaskQueue.Add(taskEntity);
            }
            return taskEntity;
        }


        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <param name="taskCallback">任务回调</param>
        /// <param name="delay">任务延时，单位秒</param>
        /// <param name="paras">回调参数</param>
        /// <returns></returns>
        public JTask AddTask(string key, float delay, object paras, TaskCallback taskCallback)
        {
            JTask taskEntity = null;
            if (HasTask(key))
                Debug.LogError("已经存在 key : " + key);
            else
            {
                taskEntity = new JTask(delay, key, taskCallback, paras);
                _TaskQueue.Add(taskEntity);
            }
            return taskEntity;
        }
        /// <summary>
        /// 是否存在任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <returns></returns>
        public bool HasTask(string key) { return null != GetTask(key); }
        /// <summary>
        /// 获取任务
        /// </summary>
        /// <param name="key">任务名称</param>
        /// <returns></returns>
        public JTask GetTask(string key)
        {
            JTask jt = _TaskQueue.Find((x) =>
            {
                if (x.taskName == key) return true;
                return false;
            });
            if (jt != null) return jt;
            return null;
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="key">任务名称</param>
        public void RemoveTask(string key, bool executeEvent = false)
        {
            JTask jt = GetTask(key);
            _TaskQueue.Remove(jt);
        }


        private void FixedUpdate()
        {
            JTask[] jts = _TaskQueue.FindAll((x) =>
            {
                if (x.Complete) return true;
                return false;
            }).ToArray();

            for (int i = 0; i < jts.Length; i++)
                _TaskQueue.Remove(jts[i]);

            for (int i = 0; i < _TaskQueue.Count; i++)
            {
                if (!_TaskQueue[i].Complete)
                {
                    if (isDebug) Debug.Log(_TaskQueue[i]);
                    _TaskQueue[i].Update();
                }
            }
        }
    }

    public class JTask
    {
        public event TaskCallback OnTask;
        public bool Complete { get; private set; }
        public string taskName { get; private set; }
        public bool Pause { get; set; }

        bool hasParas;
        object paras;

        float delayTime;//任务延时
        float startTime;//开始时间
        float surpulsTime;//任务时间剩余

        public JTask()
        {
            Complete = false;
            startTime = Time.time;
        }

        public JTask(float time, string taskName, TaskCallback action) : this()
        {
            delayTime = time;
            this.taskName = taskName;
            OnTask += action;
        }

        public JTask(float time, string taskName, TaskCallback action, object paras) : this(time, taskName, action)
        {
            hasParas = true;
            this.paras = paras;
        }

        internal void Update()
        {
            if (Pause)
            {
                startTime = Time.time + surpulsTime - delayTime;
                return;
            }
            surpulsTime = delayTime - Time.time + startTime;
            if (surpulsTime <= 0)
            {
                if (hasParas) OnTask?.Invoke(paras);
                else OnTask?.Invoke();
                Complete = true;
            }
        }
        /// <summary>
        /// 完成任务 
        /// </summary>
        /// <param name="executeEvent">是否执行回调</param>
        public void OnComplete(bool executeEvent)
        {
            if(executeEvent)
            {
                if (hasParas) OnTask?.Invoke(paras);
                else OnTask?.Invoke();
            }
            Complete = true;
        }
        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="action"></param>
        public void OnAddEvent(TaskCallback action)
        {
            OnTask += action;
        }
        /// <summary>
        /// 移除回调
        /// </summary>
        /// <param name="action"></param>
        public void OnRemoveEvent(TaskCallback action)
        {
            OnTask -= action;
        }
        /// <summary>
        /// 重置任务
        /// </summary>
        /// <returns></returns>
        public JTask Reset()
        {
            startTime = Time.time;
            Complete = false;
            Pause = false;
            return this;
        }

        public override string ToString() { return taskName + " Task Time Has  -> " + surpulsTime; }
    }

    public delegate void TaskCallback(object paras = null);
}

