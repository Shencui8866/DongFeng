using Josing.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Josing
{
    public enum JLogType
    {
        Error = 0,
        Assert = 1,
        Warning = 2,
        Log = 3,
        Exception = 4
    }
    public class LogGUI : SingletonBase<LogGUI>
    {
        [SerializeField]
        bool show;

        [SerializeField]
        int maxCount;

        Vector2 pos;
        LogPools logPools;
        Queue<string> logsCarsh = new Queue<string>();

        private void Awake()
        {
            IOUtils.DeFile(JPath.LogPath);
            logPools = new LogPools(maxCount);
            Application.logMessageReceived += Application_logMessageReceived;
        }
        
        public void LogAddScreen(string log) { logPools.Add(log); }

        public void LogWrite(JLogType type, string tagInfo, string log)
        {
            IOUtils.WriteAppend(JPath.LogPath, new string[] { string.Format("[{0} | {1}]\t{2}", System.DateTime.Now.ToLongTimeString(), type.ToString(), tagInfo), log });
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            IOUtils.WriteAppend(JPath.LogPath, new string[] { string.Format("[{0} | {1}]\t{2}", System.DateTime.Now.ToLongTimeString(), type.ToString(), condition, stackTrace) });
            switch (type)
            {
                case LogType.Exception: logPools.Add(string.Format("<color=green>{0}\n{1}</color>", condition, stackTrace)); break;
                case LogType.Log: logPools.Add(string.Format("{0}\n{1}", condition, stackTrace)); break;
                case LogType.Warning: logPools.Add(string.Format("<color=yellow>{0}\n{1}</color>", condition, stackTrace)); break;
                case LogType.Error: logPools.Add(string.Format("<color=red>{0}\n{1}</color>", condition, stackTrace)); break;
                default: logPools.Add(string.Format("{0}\n{1}", condition, stackTrace)); break;
            }
        }

        private void OnGUI()
        {
            if (!show) return;
            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

            GUI.BeginGroup(new Rect(Screen.width * 0.3f, Screen.height * 0.8f, Screen.width * 0.3f, Screen.height * 0.2f));
            GUI.Box(new Rect(0, 0, Screen.width * 0.3f, Screen.height * 0.2f), "");
            GUI.Label(new Rect(0, Screen.height * 0.1f, Screen.width * 0.3f, Screen.height * 0.2f), Time.time.ToString());
            GUI.EndGroup();

            GUI.Box(new Rect(0, 0, Screen.width * 0.3f, Screen.height), "Log");
            pos = GUILayout.BeginScrollView(pos, GUILayout.Width(Screen.width * 0.3f));
            for (int i = 0; i < logPools.Pools.Count; i++)
                GUILayout.Label(logPools.Pools[i]);
            GUILayout.EndScrollView();


            GUI.EndGroup();
        }
    }
}

