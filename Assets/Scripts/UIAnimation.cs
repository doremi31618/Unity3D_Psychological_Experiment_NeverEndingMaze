using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    public AnimationType type = AnimationType.enable;
    public enum AnimationType { Fade, slide, order_fade, enable }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
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
            item.gameObject.SetActive(isActive);
        }
    }

}
