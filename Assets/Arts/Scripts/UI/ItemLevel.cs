using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLevel : MonoBehaviour
{
    private Text text;
    private Toggle tog1;
    private Toggle tog2;
    private Toggle tog3;

    void Awake()
    {
        text = transform.Find("txt").GetComponent<Text>();
        tog1 = transform.Find("tog1").GetComponent<Toggle>();
        tog2 = transform.Find("tog2").GetComponent<Toggle>();
        tog3 = transform.Find("tog3").GetComponent<Toggle>();
    }

    public void Init(int id,int star)
    {
        text.text = (id + 1).ToString();
        tog1.isOn = star > 0;
        tog2.isOn = star > 1;
        tog3.isOn = star > 2;
    }
}
