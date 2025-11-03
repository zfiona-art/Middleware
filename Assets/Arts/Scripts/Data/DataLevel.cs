using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Data/Level", order = 4)]
public class DataLevel : ScriptableObject
{
    public List<Data> array = new ();
    
    [Serializable]
    public class Data
    {
        public int groundCnt;
        public int enemyCnt;
        public float enemyAdd;
    }
}
