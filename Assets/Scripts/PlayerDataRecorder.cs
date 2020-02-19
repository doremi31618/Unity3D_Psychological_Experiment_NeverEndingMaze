using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an interface of player data 
public class PlayerDataRecorder : MonoBehaviour
{
    public string player_name;
    public PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        if (player_name == "") player_name = DateTime.Now + "_Tester";
        playerData = new PlayerData(player_name);
    }
    public void ChangePlayerName(string name)
    {
        playerData.player_name = name;
    }
    public void RecordRouteData(Route route)
	{
        playerData.PlayerDataGenerator(route);
    }
}

//save all the player data
[System.Serializable]
public class PlayerData
{
    public string player_name;
    public List<Result> results;

    public PlayerData(string name)
	{
        player_name = name;
        results = new List<Result>();
    }


    public void PlayerDataGenerator(Route route)
    {
        Result new_result = new Result(results.Count, route);
        results.Add(new_result);

    }

    public void exportToExcel(string filePath)
	{

	}

}

//save player testing data in each run
[System.Serializable]
public class Result
{

    public int test_index;
    public int length;
    public int rotate_times;

    public Route route;

    public Result(int index, Route _route)
	{
        test_index = index;
        route = _route;

        length = route.get_total_length;
        rotate_times = route.get_rotate_length;

        data = new PlayerTestData();

    }

    public PlayerTestData data;//錯誤次數、玩家錯誤座標
    
}

public struct PlayerTestData
{
    public int wrong_time { get { return wrong_position.Count; } }
    public List<Vector3> wrong_position;
}