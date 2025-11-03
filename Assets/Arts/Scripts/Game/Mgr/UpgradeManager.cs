using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private Dictionary<EUpgradeItem,int> upgradeDic;
    public Addition addition;

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
            {EUpgradeItem.BulletInterval,1},
            {EUpgradeItem.BulletCount,0},
            {EUpgradeItem.BulletDamage,10},
            {EUpgradeItem.CircleCount,4},
            {EUpgradeItem.CircleDamage,10},
        };
        addition = new Addition();
    }

    public List<EUpgradeItem> GetItems()
    {
        var list = new List<EUpgradeItem>();
        for (var i = 0; i < 3; i++)
        {
            var key = (EUpgradeItem)Random.Range(0, upgradeDic.Count);
            var val = upgradeDic[key];
            if (val <= 0 || (key==EUpgradeItem.CircleDamage && addition.cCount==0))
                i--;
            else
                list.Add(key);
        }

        return list;
    }

    public string GetDescribe(EUpgradeItem item)
    {
        var describe = "";
        switch (item)
        {
            case EUpgradeItem.MaxHealth:
                describe = $"最大生命值加成: {addition.maxHealth}% -> {addition.maxHealth + 10}%";
                break;
            case EUpgradeItem.MoveSpeed:
                describe = $"移动速度加成: {addition.moveSpeed}% -> {addition.moveSpeed + 50}%";
                break;
            case EUpgradeItem.BulletInterval:
                describe = $"攻击速度加成: {addition.bInterval}% -> {addition.bInterval + 50}%";
                break;
            case EUpgradeItem.BulletCount:
                describe =  $"篮球数量: {addition.bCount + 1} -> {addition.bCount + 2}";
                break;
            case EUpgradeItem.BulletDamage:
                describe = $"篮球伤害加成: {addition.bDamage} -> {addition.bDamage + 3}";
                break;
            case EUpgradeItem.CircleCount:
                describe = $"尖叫鸡数量: {addition.cCount + 0} -> {addition.cCount + 1}";
                break;
            case EUpgradeItem.CircleDamage:
                describe = $"尖叫鸡伤害加成: {addition.cDamage} -> {addition.cDamage + 3}";
                break;
        }

        return describe;
    }
    

    public void SetItem(EUpgradeItem item)
    {
        switch (item)
        {
            case EUpgradeItem.MaxHealth:
                addition.maxHealth += 10;
                GameManager.Instance.player.ResetHealth();
                break;
            case EUpgradeItem.MoveSpeed:
                addition.moveSpeed += 50;
                break;
            case EUpgradeItem.BulletInterval:
                addition.bInterval += 50;
                GameManager.Instance.player.ResetFire();
                break;
            case EUpgradeItem.BulletDamage:
                addition.bDamage += 3;
                break;
            case EUpgradeItem.CircleCount:
                addition.cCount += 1;
                GameManager.Instance.player.ResetWeapon2();
                break;
            case EUpgradeItem.CircleDamage:
                addition.cDamage += 3;
                break;
        }
        upgradeDic[item]--;
    }
    
    
    public enum EUpgradeItem
    {
        MaxHealth,
        MoveSpeed,
        BulletInterval,
        BulletCount,
        BulletDamage,
        CircleCount,
        CircleDamage,
    }
    
    public class Addition
    {
        public float maxHealth;
        public float moveSpeed;
        public float bInterval;
        public int bCount;
        public float bDamage;
        public int cCount;
        public int cDamage;
    }
    
}
