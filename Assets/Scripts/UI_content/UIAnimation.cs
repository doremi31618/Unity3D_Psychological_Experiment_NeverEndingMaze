using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    public bool AutoStart;
    public AnimationType type = AnimationType.enable;
    [Tooltip("Automatically enactive at start")]public List<GameObject> Layer2;
    public enum AnimationType { Fade, slide, order_fade, enable }
    // Start is called before the first frame update

    public AnimationAction animationAction;
    private void Start() {
        if(AutoStart)StartPage();
    }
    public void EndPage()
    {
        animationAction.EndPage();
    }

    public void StartPage()
    {
        Debug.Log("start page " + this.gameObject.name);
        animationAction.StartPage();
        EnableAllLayer(false);
    }

    public void enable_all_UI_element(bool isActive)
    {
        // Debug.Log("isActive : " + isActive +" child count : " + GetComponentsInChildren<RectTransform>().Length);

        foreach (var item in GetComponentsInChildren<RectTransform>(true))
        {
            if (item == this.transform) continue;
            if(item.gameObject.name == "Template") continue;
            item.gameObject.SetActive(isActive);
        }
    }

    public void EnableAllLayer(bool isActive)
    {
        foreach (var item in Layer2)
        {
            if (item == this.transform) continue;
            item.gameObject.SetActive(isActive);
        }
    }
    public void EnableSingleLayer(bool isActive,int index)
    {
        for(int i=0; i<Layer2.Count; i++)
        {
            if(i == index)Layer2[i].SetActive(isActive);
        }
    }


}
