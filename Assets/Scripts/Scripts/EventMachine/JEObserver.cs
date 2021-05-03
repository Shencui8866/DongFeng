using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Josing
{
    /// <summary>
    /// Josing Event Observer
    /// </summary>
    public class JEObserver
    {

        static Dictionary<string, ObserverCallback> eventPools = new Dictionary<string, ObserverCallback>();
        static bool init = false;

        public static void Add(string key, ObserverCallback call)
        {
            if (!eventPools.ContainsKey(key))
                eventPools.Add(key, call);
        }

        public static void Call(ObserverEvent oevent)
        {
            if (eventPools.ContainsKey(oevent.EType))
                eventPools[oevent.EType](oevent);
        }
    }

    public delegate void ObserverCallback(ObserverEvent oevent);

    public class ObserverEvent
    {
        public string EType { get; private set; }
        public object[] param { get; private set; }

        public ObserverEvent(string type, params object[] parms)
        {
            param = parms;
            EType = type;
        }

        public override string ToString()
        {
            string name = "";
            for (int i = 0; i < param.Length; i++)
                name += param[i].ToString();
            return string.Format("类型：{0}，参数：{1}", EType, name);
        }
    }
}

