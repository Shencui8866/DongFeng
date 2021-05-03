using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDefine : MonoBehaviour
{
    #region 接口常量

    public const string serverIp = "http://192.168.1.106:8088/";

    /// <summary>
    /// 规划路径开始通知
    /// </summary>
    public const string routeNotice = serverIp + "car/routeNotice";

    /// <summary>
    /// 变道超车开始通知
    /// </summary>
    public const string overtakeNotice = serverIp + "car/overtakeNotice";

    /// <summary>
    /// 游戏开始通知
    /// </summary>
    public const string gameStart = serverIp + "car/gameStart";

    /// <summary>
    /// 车辆到达通知
    /// </summary>
    public const string arriveNotice = serverIp + "car/arriveNotice";

    /// <summary>
    /// 车辆到达通知
    /// </summary>
    public const string updateVideoName = serverIp + "car/updateVideoName";

    #endregion
}