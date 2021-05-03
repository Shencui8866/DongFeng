using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Josing.IO
{
    public class JPath
    {
        public static string CfgPath { get { return DataPath + "/cfg.txt"; } }
        /// <summary>
        /// 日志路径
        /// </summary>
        public static string LogPath { get { return DataPath + "/log.txt"; } }
        /// <summary>
        /// 视频路径
        /// </summary>
        public static string VideoPath { get { return DataPath + "/Video"; } }
        /// <summary>
        /// 音频路径
        /// </summary>
        public static string AudioPath { get { return DataPath + "/Audio"; } }
        /// <summary>
        /// 贴图路径
        /// </summary>
        public static string TexturePath { get { return DataPath + "/Texture"; } }
        /// <summary>
        /// 数据路径
        /// </summary>
        public static string DataPath
        {
            get
            {
#if UNITY_STANDALONE && !UNITY_EDITOR
                int index = Application.dataPath.LastIndexOf('/');
                string path = Application.dataPath.Substring(0, index);
                if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
                return path;
#elif UNITY_EDITOR
                int index = Application.dataPath.LastIndexOf('/');
                string path = Application.dataPath.Substring(0, index);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
#elif !UNITY_EDITOR
                return null;
#endif
            }
        }
    }
}

