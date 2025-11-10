using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel", menuName = "Data/GameLevel", order = 4)]
public class DataLevel : ScriptableObject
{
    public List<Level> array = new ();
    
    [Serializable]
    public class Level
    {
        public int groundCnt;
        public int healthAdd;
        public int damageAdd;
        public List<Round> rounds = new ();

        public int GetEnemyCnt()
        {
            var cnt = 0;
            foreach (var round in rounds)
                cnt += round.enemies.Count;  
            return cnt;
        }
    }
    
    [Serializable]
    public class Round
    {
        public List<Enemy> enemies = new(); 
    }
    
    [Serializable]
    public class Enemy
    {
        public int id;
        public int x;
        public int y;
    }
}
