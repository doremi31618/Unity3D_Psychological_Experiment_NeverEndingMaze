using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    public AnimationType type = AnimationType.enable;
    [Tooltip("Automatically enactive at start")]public List<GameObject> Layer2;
    public enum AnimationType { Fade, slide, order_fade, enable }
    // Start is called before the first frame update
    
    public void EndPage()
    {
        switch (type)
        {
            case AnimationType.enable:
                enable_all_UI_element(false);
                break;
            default:
                enable_all_UI_element(false);
                break;
        }
    }

    public void StartPage()
    {
        switch (type)
        {
            case AnimationType.enable:
                enable_all_UI_element(true);
                EnableAllLayer(false);
                break;
            default:
                enable_all_UI_element(true);
                break;
        }
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
