using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Data/Game", order = 3)]
public class DataGame : ScriptableObject
{
    public int maxTimeSeconds;
    public int maxPropNum;
    public int maxEnemyNum;
    public float enemyInterval;
    public float panelWidth;
}
