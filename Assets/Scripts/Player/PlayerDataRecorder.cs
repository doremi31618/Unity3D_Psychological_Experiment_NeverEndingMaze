
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using SFB;
//https://github.com/gkngkc/UnityStandaloneFileBrowser
//an interface of player data 
public class PlayerDataRecorder : MonoBehaviour
{
    public string player_name;
    public string fileName = "MyExcel.xls";
    public PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        if (player_name == "") player_name = DateTime.Now + "_Tester";

        playerData = new PlayerData(player_name);
        fileName = "Tester_" + playerData.player_name + ".xls";
        // ExportPlayerData();
    }
    public void ChangePlayerName(string name)
    {
        playerData.player_name = player_name = name;
        fileName = name + ".xls";
    }
    public void RecordLandmarkType(int count_of_landmark)
    {
        playerData.get_newest_results.landmark_number = count_of_landmark;
    }
    public void RecordRouteData(Route route)
    {
        playerData.PlayerDataGenerator(route);
    }

    public void RecordPlayerChoise(int index, int player_choise, int player_choise_direction)
    {
        Result current = getCurrentResult();
        current.route.setPlayerChoise(index, player_choise, player_choise_direction);
    }
    public Result getCurrentResult()
    {
        List<Result> player_data_results = playerData.results;
        return player_data_results[player_data_results.Count - 1];
    }
    public void ExportPlayerData()
    {
//         string _path;

//         //mac os Streaming Assetes address 
// #if UNITY_STANDALONE_OSX || !UNITY_EDITOR
//         _path = Application.dataPath + "/Resources/Data/StreamingAssets" + fileName;
// #endif

//         //Windows & Unity Editor Streaming Assetes address 
// #if UNITY_EDITOR || UNITY_STANDALONE_WIN
//         _path = Application.dataPath + "/StreamingAssets/" + fileName;
// #endif

        // var path =  StandaloneFileBrowser.SaveFilePanel(
        //     "Save file as xls",
        //     "",
        //     fileName + ".xls",
        //     "xls");
        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", fileName + ".xls", "xls", (string path) => { playerData.saveToExcelFile(path);});
        // var _path = StandaloneFileBrowser.SaveFilePanel("Save File", "",fileName + ".xls", "xls");
        // playerData.saveToExcelFile(_path);

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
    public Result get_newest_results{get{return results[results.Count-1];}}


    public void PlayerDataGenerator(Route route)
    {
        Result new_result = new Result(results.Count, route);
        results.Add(new_result);

    }
    public void saveToExcelFile(string filePath)
    {
        using (ExcelPackage excel = new ExcelPackage())
        {
            for (int i = 0; i < results.Count; i++)
            {
                Result _results = results[i];
                Route _route = _results.route;

                Vector3[] vertex = _route.get_route_vertex;
                Vector3[] direction = _route.get_route_direction;
                bool[] rotate = _route.get_route;
                int[] player_answer = _route.get_player_choisies;
                int[] player_answer_direction = _route.get_player_choisies_direction;
                int landmark_index = _route.get_landmark_index;
                string Title = "[Constatn]_";
                if(_results.route.UseLandmark )
                {
                    Title =  "[With_Landmark_"+_results.landmark_number+"]_";
                }else if(_results.route.Manual)
                {
                    Title =  "[Manual]_";
                }
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add(Title + "Index_" + _results.test_index + "Length_" + _results.length + "Rotate_" + _results.rotate_times);
                worksheet.Cells[1, 1].Value = "座標";
                worksheet.Cells[1, 2].Value = "方向";
                worksheet.Cells[1, 3].Value = "是否轉彎";
                worksheet.Cells[1, 4].Value = "受測者選擇";
                worksheet.Cells[1, 5].Value = "受測者選則的方向";
                worksheet.Cells[1, 6].Value = "測試時間";
                worksheet.Cells[2, 6].Value = DateTime.Now + "";
                //讀取座標、寫入座標
                for (int v = 0; v < vertex.Length; v++)
                {
                    worksheet.Cells[2 + v, 1].Value = vertex[v];
                }

                for (int _index = 0; _index < direction.Length; _index++)
                {


                    //讀取方向、寫入方向
                    worksheet.Cells[2 + _index, 2].Value = direction[_index];

                    //讀取路線選擇
                    worksheet.Cells[2 + _index, 3].Value = rotate[_index];

                    //讀取受測者選擇結果
                    string answer = "";
                    if (player_answer[_index] == 1)
                    {
                        answer = "正確";

                    }
                    else if (player_answer[_index] == 0)
                    {
                        answer = "錯誤";
                    }
                    else answer = "None";
                    worksheet.Cells[2 + _index, 4].Value = answer;

                    //讀取受測者選擇方向
                    string direction_content = "";
                    if (player_answer_direction[_index] == 1)
                    {
                        direction_content = "右邊";

                    }
                    else if (player_answer_direction[_index] == -1)
                    {
                        direction_content = "左邊";
                    }
                    else answer = "None";
                    worksheet.Cells[2 + _index, 5].Value = direction_content;
                }
            }
            
            FileInfo excelFile = new FileInfo(filePath);
            excel.SaveAs(excelFile);
        }
    }


}

//save player testing data in each run
[System.Serializable]
public class Result
{

    public int test_index;
    public int length;
    public int rotate_times;
    public int landmark_number = 0;

    public Route route;

    public Result(int index, Route _route)
    {
        test_index = index;
        route = _route;

        length = route.get_total_length;
        rotate_times = route.get_rotate_length;

    }

}