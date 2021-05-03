using System.Collections;
using System.Collections.Generic;
using Josing.IO;
using UnityEngine;

public enum ReturnDataType
{
    无反馈=-1,
    闲置=0,
    流程开始=1,
    大屏关门开始体验=2,
    大屏情景模式选择=3,
    大屏规划路径结果=4,
    大屏变道超车结果=5,
    大屏游戏反馈=6,
    大屏游戏结束,
    大屏车辆开门,
    大屏车辆关门,
    休息 = 50
}

// /// <summary>
// /// 流程开始
// /// </summary>
// public static string screenStart = "1";
//
// /// <summary>
// /// 大屏关门开始体验
// /// </summary>
// public static string screenCloseDoorExperience = "2";
//
// /// <summary>
// /// 大屏情景模式选择
// /// </summary>
// public static string screenPatternSelect = "3";
//
// /// <summary>
// /// 大屏规划路径结果
// /// </summary>
// public static string screenRouteResult = "4";
//
// /// <summary>
// /// 大屏变道超车结果
// /// </summary>
// public static string screenOvertakeResult = "5";
//
// /// <summary>
// /// 大屏游戏反馈
// /// </summary>
// public static string screenGameFeedback = "6";
//
// /// <summary>
// /// 大屏游戏结束
// /// </summary>
// public static string screenGameOver = "7";
//
// /// <summary>
// /// 大屏车辆开门
// /// </summary>
// public static string screenOpenDoor = "8";
//
// /// <summary>
// /// 大屏车辆关门
// /// </summary>
// public static string screenCloseDoor = "9";