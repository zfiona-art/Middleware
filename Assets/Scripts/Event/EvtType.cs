using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 事件消息类型(统一在这里写)
/// </summary>
public class EvtType
{
    public const string UpdateCoins = "UpdateCoins";
    public const string UpdateLevel = "UpdateLevel";
    public const string UpdatePlayer = "UpdatePlayer";
    public const string UpdatePlayerAnim = "UpdatePlayerAnim";
    public const string FirstSwipeScreen = "FirstSwipeScreen";
    public const string FirstAnchorPoint = "FirstAnchorPoint";
    public const string FirstAnchorPointEnd = "FirstAnchorPointEnd";
    public const string OnGuide2Start = "OnGuide2Start";
    public const string OnGuide2End = "OnGuide2End";
    public const string ShowMainRects = "ShowMainRects";
}
