using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
        // ListNode head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4))));
        // head = ReverseList(head);
        // Debug.Log(head.val);
        
        
    }

}
