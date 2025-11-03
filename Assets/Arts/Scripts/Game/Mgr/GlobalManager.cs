using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    public static int TotalLevel; //每章10关
    public int Level
    {
        get => PlayerPrefs.GetInt("PlayerLevel", 1);
        set => PlayerPrefs.SetInt("PlayerLevel", value);
    }

    public int Avatar
    {
        get => PlayerPrefs.GetInt("PlayerAvatar", 0);
        set => PlayerPrefs.SetInt("PlayerAvatar", value);
    }
}
