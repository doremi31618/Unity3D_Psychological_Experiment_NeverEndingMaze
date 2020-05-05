using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class AnimationAction : MonoBehaviour
{
    public UnityEvent startingEvent;
    public UnityEvent endingEvent;
    public abstract void StartPage();
    public abstract void EndPage();
    public void onTimerCallOut(float time,UnityAction _action){

        StartCoroutine(timer(time,_action));
    }

    IEnumerator timer(float time, UnityAction callout){
        yield return new WaitForSeconds(time);
        if(this.gameObject.activeSelf) callout.Invoke();
    }

}
