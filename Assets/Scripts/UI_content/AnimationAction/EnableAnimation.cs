using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EnableAnimation : AnimationAction
{
    public override void StartPage()
    {
        foreach (var item in GetComponentsInChildren<RectTransform>(true))
        {

            string[] obj_NameAnalize = item.gameObject.name.Split('_');
            string _last_Name_Tag = obj_NameAnalize[obj_NameAnalize.Length - 1];

            if (item == this.transform ||
                item.gameObject.name == "Template" ||
                _last_Name_Tag == "dontInit") continue;

            item.gameObject.SetActive(true);
        }
        startingEvent.Invoke();
        
    }

    public override void EndPage()
    {
        foreach (var item in GetComponentsInChildren<RectTransform>(true))
        {
            string[] obj_NameAnalize = item.gameObject.name.Split('_');
            string _last_Name_Tag = obj_NameAnalize[obj_NameAnalize.Length - 1];

            if (item == this.transform ||
                item.gameObject.name == "Template" ||
                _last_Name_Tag == "dontInit") continue;

            item.gameObject.SetActive(false);
        }
        endingEvent.Invoke();
    }
    

}
