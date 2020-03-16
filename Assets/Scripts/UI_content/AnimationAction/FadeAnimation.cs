using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
public class FadeAnimation : AnimationAction
{
    
    public float fadeInTime =0;
    public float fadeOutTime = 0.5f;
    public AnimationCurve curve;
    public override void StartPage(){
        startingEvent.Invoke();
        foreach (var item in GetComponentsInChildren<RectTransform>(true))
        {

            string[] obj_NameAnalize = item.gameObject.name.Split('_');
            string _last_Name_Tag = obj_NameAnalize[obj_NameAnalize.Length-1];

            if (item == this.transform  || 
                item.gameObject.name == "Template" ||
                _last_Name_Tag == "dontInit") continue;

            Fade(1,item.gameObject);
        }

        EndPage();
    }

    void Fade(int mode, GameObject obj){

        mode = (int)Mathf.Clamp(mode,0,1);
        float fadeTime = mode == 0 ? fadeOutTime : fadeInTime;
        
        if(obj.GetComponent<Image>()){
            obj.GetComponent<Image>().DOFade(mode,fadeTime).SetEase(curve);
        }
        else if( obj.GetComponent<Text>()){
            obj.GetComponent<Text>().DOFade(mode,fadeTime).SetEase(curve);
        }
    }

    public override void EndPage(){
        foreach (var item in GetComponentsInChildren<RectTransform>(true))
        {

            string[] obj_NameAnalize = item.gameObject.name.Split('_');
            string _last_Name_Tag = obj_NameAnalize[obj_NameAnalize.Length-1];

            if (item == this.transform  || 
                item.gameObject.name == "Template" ||
                _last_Name_Tag == "dontInit") continue;

            Fade(0,item.gameObject);
        }
        
        UnityAction _event = () => EndPageEvent();
        onTimerCallOut(fadeOutTime, _event);
        
    }

    void EndPageEvent()
    {
        this.gameObject.SetActive(false);
        // endingEvent.Invoke();
    }
    


}
