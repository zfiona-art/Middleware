using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Chapter", menuName = "Data/Chapter", order = 5)]
public class DataChapter : ScriptableObject
{
    public List<Data> array = new();
    
    [Serializable]
    public class Data
    {
        public Sprite bg;
        public List<PropData> props;
    }
    
    [Serializable]
    public class PropData
    {
        public Sprite sprite;
        public bool isBlock;
        public int sort;
    }
}
