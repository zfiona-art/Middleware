using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class ClickEffect : MonoBehaviour
{
    public int Count = 3;                   //变大的次数
    public float Size = 1.5f;                  //变大的倍数
    public float Duration = 0.3f;              //变化的时间
   
    public System.Action finishCall;    //特效执行之后调用的方法
    public void Play()               //对外接口
    {      
        StartCoroutine(effKeep());
    }

    void Start()
    {
        if (this.GetComponent<Button>() != null)
            this.GetComponent<Button>().onClick.AddListener(Play);       
    }

    void makePrefab() {
        GameObject go = Instantiate(this.gameObject);
        go.transform.SetParent(this.transform,false);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        //Button，Text，Image等控件都继承自Unity.UI.Graphic
        go.GetComponent<Graphic>().CrossFadeAlpha(0, Duration, true);
        Tweener ts = go.transform.DOScale(Vector3.one * Size, Duration); 
        ts.OnComplete(delegate 
        {
            this.transform.localScale = Vector3.one;
            Destroy(go);
        });     
        
    }
    
    IEnumerator effKeep()
    {
        int index = 0;
        while (index <Count)
        {
            makePrefab();
            ++index;
            if (Count == 1) {
            }
            yield return new WaitForSeconds(Duration);
        }
        yield return new WaitForSeconds(0.2f);
        if(finishCall != null)
            finishCall();
    }
}