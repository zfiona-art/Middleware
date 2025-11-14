using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private Dictionary<EUpgradeItem,int> upgradeDic;
    public Addition addition;
    private int cacheSkillId = 1;

    public override void Init()
    {
        Reset();
    }
    
    public void Reset()
    {
        upgradeDic = new Dictionary<EUpgradeItem, int>()
        {
            {EUpgradeItem.MaxHealth,10},
            {EUpgradeItem.MoveSpeed,2},
            {EUpgradeItem.FireSpeed,2},
            {EUpgradeItem.BulletDistance,2},
            {EUpgradeItem.BulletDamage,10},
            {EUpgradeItem.CircleCount,4},
            {EUpgradeItem.CircleDamage,10},
            {EUpgradeItem.Skill,3},
        };
        addition = new Addition();
        if (GlobalManager.Instance.GameLevel == 1)
        {
            addition.maxHealth = 1;
            addition.moveSpeed = 1;
            addition.fireSpeed = 1;
        }
    }

    public List<EUpgradeItem> GetItems()
    {
        var list = new List<EUpgradeItem>();
        var count = 3;
        while (count > 0)
        {
            var key = (EUpgradeItem)Random.Range(0, upgradeDic.Count);
            var val = upgradeDic[key];
            if (val <= 0) continue;
            if (list.Contains(key)) continue;
            if (key == EUpgradeItem.Skill) 
                cacheSkillId =ToolUtil.IsProbabilityOk(50) ? GlobalManager.Instance.ChapterId : Random.Range(1, 6);
            list.Add(key);
            count--;
        }

        return list;
    }

    public string GetDescribe(EUpgradeItem item)
    {
        var describe = "";
        switch (item)
        {
            case EUpgradeItem.MaxHealth:
                describe = $"生命值: +1%";
                break;
            case EUpgradeItem.MoveSpeed:
                describe = $"移动速度: +1%";
                break;
            case EUpgradeItem.FireSpeed:
                describe = $"攻击速度: +1%";
                break;
            case EUpgradeItem.BulletDistance:
                describe = $"攻击距离: +1%";
                break;
            case EUpgradeItem.BulletDamage:
                describe = $"篮球伤害: +1%";
                break;
            case EUpgradeItem.CircleCount:
                describe = $"尖叫鸡数量: +1";
                break;
            case EUpgradeItem.CircleDamage:
                describe = $"尖叫鸡伤害: +1%";
                break;
            case EUpgradeItem.Skill:
                if(cacheSkillId == 1)
                    describe = "得到1个旋风技能";
                if(cacheSkillId == 2)
                    describe = "得到1个火球技能";
                if(cacheSkillId == 3)
                    describe = "得到1个地刺技能";
                if(cacheSkillId == 4)
                    describe = "得到1个闪电技能";
                if(cacheSkillId == 5)
                    describe = "得到1个雷环技能";
                break;
        }
        return describe;
    }
    

    public void SetItem(EUpgradeItem item)
    {
        switch (item)
        {
            case EUpgradeItem.MaxHealth:
                addition.maxHealth += 1;
                GameManager.Instance.player.ResetHealth();
                break;
            case EUpgradeItem.MoveSpeed:
                addition.moveSpeed += 1;
                break;
            case EUpgradeItem.FireSpeed:
                addition.fireSpeed += 1;
                GameManager.Instance.player.ResetFire();
                break;
            case EUpgradeItem.BulletDistance:
                addition.bDistance += 1;
                break;
            case EUpgradeItem.BulletDamage:
                addition.bDamage += 1;
                break;
            case EUpgradeItem.CircleCount:
                addition.cCount += 1;
                GameManager.Instance.player.RefreshWeapon2();
                break;
            case EUpgradeItem.CircleDamage:
                addition.cDamage += 1;
                break;
            case EUpgradeItem.Skill:
                OnSkillGet();
                break;
        }
        upgradeDic[item]--;
    }

    private void OnSkillGet()
    {
        EventCtrl.SendEvent(EventDefine.OnSkillGet,cacheSkillId);
    }
    
    public enum EUpgradeItem
    {
        MaxHealth,
        MoveSpeed,
        FireSpeed,
        BulletDistance,
        BulletDamage,
        CircleDamage,
        CircleCount,
        Skill,
    }
}

public class Addition
{
    public int maxHealth;
    public int moveSpeed;
    public int fireSpeed;
    public int bDistance;
    public int bDamage;
    public int cCount;
    public int cDamage;
}