using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Data/Enemy", order = 2)]
public class DataEnemy : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float damage;
    public float lookDistance;
}
