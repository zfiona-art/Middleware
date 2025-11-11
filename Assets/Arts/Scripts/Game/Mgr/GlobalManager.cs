using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GlobalManager : Singleton<GlobalManager>
{
    public const int ChapterLevelCnt = 10;
    public int GameLevel
    {
        get => PlayerPrefs.GetInt("GameLevel", 1);
        set => PlayerPrefs.SetInt("GameLevel", value);
    }

    public int ChapterId => (GameLevel-1) / 10 + 1;

    public int Avatar
    {
        get => PlayerPrefs.GetInt("PlayerAvatar", 0);
        set => PlayerPrefs.SetInt("PlayerAvatar", value);
    }

    public int Coin
    {
        get => PlayerPrefs.GetInt("PlayerCoin", 0);
        set 
        {
            PlayerPrefs.SetInt("PlayerCoin", value);
            EventCtrl.SendEvent(EventDefine.OnCoinModify);
        }
    }

    public int Diamond
    {
        get => PlayerPrefs.GetInt("PlayerDiamond", 0);
        set 
        {
            PlayerPrefs.SetInt("PlayerDiamond", value);
            EventCtrl.SendEvent(EventDefine.OnCoinModify);
        }
    }

    public int SignDay
    {
        get => PlayerPrefs.GetInt("PlayerSignDay", 0);
        set => PlayerPrefs.SetInt("PlayerSignDay", value);
    }

    public int LastSignStamp
    {
        get => PlayerPrefs.GetInt("LastSignStamp", 0);
        set => PlayerPrefs.SetInt("LastSignStamp", value);
    }

    private string LevelStarCache
    {
        get => PlayerPrefs.GetString("LevelStarCache", null);
        set => PlayerPrefs.SetString("LevelStarCache", value);
    }
    private List<int> levelStars;
    public List<int> GetLevelStars()
    {
        levelStars ??= JsonConvert.DeserializeObject<List<int>>(LevelStarCache);
        levelStars ??= new List<int>();
        return levelStars;
    }
    public void SetLevelStar(int star)
    {
        var ll = GetLevelStars();
        if (GameLevel > ll.Count)
            ll.Add(star);
        else
            ll[GameLevel - 1] = star;
        LevelStarCache = JsonConvert.SerializeObject(ll);
    }
    
    private string AdditionCache
    {
        get => PlayerPrefs.GetString("AdditionCache", null);
        set => PlayerPrefs.SetString("AdditionCache", value);
    }
    private Addition playerAdd;
    public Addition GetPlayerAdd()
    {
        playerAdd ??= JsonConvert.DeserializeObject<Addition>(AdditionCache);
        playerAdd ??= new Addition();
        return playerAdd;
    }
    public void SetPlayerAdd(Addition addition)
    {
        var add = GetPlayerAdd();
        add.maxHealth += addition.maxHealth;
        add.moveSpeed += addition.moveSpeed;
        add.fireSpeed += addition.fireSpeed;
        add.bDistance += addition.bDistance;
        add.bDamage += addition.bDamage;
        add.cDamage += addition.cDamage;
        AdditionCache = JsonConvert.SerializeObject(add);
    }
    
    
}
