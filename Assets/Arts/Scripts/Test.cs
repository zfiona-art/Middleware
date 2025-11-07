using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Test : MonoBehaviour
{
    public class ListNode
    {
        public int val;
        public ListNode next;
        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }
    private ListNode ReverseList(ListNode l)
    {
        ListNode p;
        ListNode c;
        ListNode n;

        p = null;
        c = l.next;
        while (c != null)
        {
            n = c.next;
            c.next = p;
            p = c;
            c = n;
        }
        return p;
    }

    void Start()
    {
        var list = new List<int>();
        list.Add(1);
        list.Add(2);
        var json = JsonConvert.SerializeObject(list);
        Debug.Log($"{json}");
        var data = JsonConvert.DeserializeObject<List<int>>(json);
        Debug.Log(data.Count);
        
        
    }

}
