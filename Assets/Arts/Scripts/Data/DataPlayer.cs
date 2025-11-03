

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Data/Player", order = 1)]
public class DataPlayer : ScriptableObject
{
    public float health;
    public float moveSpeed;

    public float bulletDistance;//射击距离
    public float bulletInterval;//射击间隔
    public float bulletSpeed;//飞行速度
    public float bulletDamage;//子弹伤害

    public float circleSpeed;//旋转速度
    public float circleDamage;//旋转伤害
    
    public List<int> levelUpExps = new ();//升级所需能量
}
