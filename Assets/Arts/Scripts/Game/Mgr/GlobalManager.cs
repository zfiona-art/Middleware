using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    public const int ChapterLevelCnt = 10;
    public static int PlayerLevel;
    public int GameLevel
    {
        get => PlayerPrefs.GetInt("GameLevel", 1);
        set => PlayerPrefs.SetInt("GameLevel", value);
    }

    public int ChapterId => GameLevel / 10 + 1;

    public int Avatar
    {
        get => PlayerPrefs.GetInt("PlayerAvatar", 0);
        set => PlayerPrefs.SetInt("PlayerAvatar", value);
    }
    
}
