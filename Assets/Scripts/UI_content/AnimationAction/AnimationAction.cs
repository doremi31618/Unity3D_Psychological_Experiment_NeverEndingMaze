using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
public abstract class AnimationAction : MonoBehaviour
{
    public UnityEvent startingEvent;
    public UnityEvent endingEvent;
    public abstract void StartPage();
    public abstract void EndPage();
    public void onTimerCallOut(float time,UnityAction _action){

        // StartCoroutine(timer(time,_action));
        float countDownTime = time;
        DOTween.To(()=>countDownTime, x=>countDownTime=x,0,time).OnComplete(()=>{
            if(this.gameObject.activeSelf) _action.Invoke();
        });
    }

    //not use now (version 1.03)
    IEnumerator timer(float time, UnityAction callout){
        yield return new WaitForSeconds(time);
        if(this.gameObject.activeSelf) callout.Invoke();
    }

}
