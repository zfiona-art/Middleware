using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Enemy", menuName = "Data/Enemy", order = 2)]
public class DataEnemy : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float damage;
    public float damageInterval;
    public float lookDistance;
    public float fireDistance;
    public float fireSpeed;
}
