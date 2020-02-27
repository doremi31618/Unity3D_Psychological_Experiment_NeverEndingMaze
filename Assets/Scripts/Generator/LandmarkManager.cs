using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkManager : MonoBehaviour
{
    public GameObject Landmark_offset;
    public GameObject[] Landmark;
    public int landmark_logo_numbers = 1;
    public bool isUseLandmark;

    public void InitLandmark(float interval)
    {
        float height = Landmark[0].transform.localPosition.y;
        interval -= 0.01f;
        Landmark[0].transform.localPosition = new Vector3(-interval / 2, height, interval / 2);
        Landmark[1].transform.localPosition = new Vector3(-interval / 2, height, -interval / 2);
        Landmark[2].transform.localPosition = new Vector3(interval / 2, height, -interval / 2);
        Landmark[3].transform.localPosition = new Vector3(interval / 2, height, interval / 2);

    }

    public void ChangeLandmarkPosition(Vector3 new_position)
    {
        Landmark_offset.transform.position = new_position;

    }
    public void isActiveLandmark(bool isActice)
    {
        Landmark_offset.SetActive(isActice);
    }


    //now only support 0、1、4
    public void ChangeLandmarkNumber(int number)
    {
        if (number == 1)
        {
            int rnd_number = (int)Mathf.Lerp(0, 3, Random.value);
            for (int i = 0; i < Landmark.Length; i++)
            {
                Landmark[i].SetActive(i == rnd_number);
            }
        }
        else if (number == 4)
        {
            for (int i = 0; i < Landmark.Length; i++)
            {
                Landmark[i].SetActive(true);
            }
        }
        else if (number == 0)
        {
            for (int i = 0; i < Landmark.Length; i++)
            {
                Landmark[i].SetActive(false);
            }
        }


    }


}
