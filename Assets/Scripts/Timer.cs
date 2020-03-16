using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Timer : MonoBehaviour
{

    // add action -> add listener -> event invoke
    public void onTimerCallOut(float time,UnityAction _action){

        StartCoroutine(timer(time,_action));
    }

    IEnumerator timer(float time, UnityAction callout){
        yield return new WaitForSeconds(time);
        callout.Invoke();
    }
}
