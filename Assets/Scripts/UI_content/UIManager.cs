using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIAnimation current_page;
    public List<UIAnimation> Manage_Pages;
    
    // Start is called before the first frame update
    void Start()
    {
        Init_all_page();
    }

    void Init_all_page()
    {
        int index = 0;
        current_page = Manage_Pages[0];
        foreach(var item in Manage_Pages)
        {
            if(index == 0)item.StartPage();
            else item.EndPage();
            index ++;
        }
    }

    public void ChangePage(int index)
    {
        current_page.EndPage();
        Manage_Pages[index].StartPage();
        current_page =  Manage_Pages[index];
    }
}
