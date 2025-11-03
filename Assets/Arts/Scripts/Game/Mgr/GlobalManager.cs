using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    public int Level
    {
        get => PlayerPrefs.GetInt("PlayerLevel", 1);
        set => PlayerPrefs.SetInt("PlayerLevel", value);
    }

    public int TotalLevel => 30; //3个章节，每章10关
    
    


}
