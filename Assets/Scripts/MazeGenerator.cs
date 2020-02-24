using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeGenerator : MonoBehaviour
{
    public moveDirection Direction;
    [Header("GameObject content")]
    public GameObject player_boundary_visulizer;
    public GameObject maze_boundary_visulizer;
    public List<GameObject> building_prefabs;

    [Header("Maze Attribute")]
    public float square_width_building_number = 12;
    public float maze_width_threshold = 0.01f;

    [Header("Building Attribute")]
    public float building_interval = 5f;
    public int max_building_number = 500;

    [Header("other attribute")]
    public bool isUnitTest = true;

    [HideInInspector] public List<GameObject> building_pool = new List<GameObject>();
    [HideInInspector] public GameObject Player;

    GameObject parents;

    public enum moveDirection
    {
        none,
        right,
        left,
        forward,
        back
    }
    Boundary maze_boundary = new Boundary();
    Boundary player_boundary = new Boundary();

    //[System.Serializable]
    public struct Boundary
    {
        public Vector3 North, East, West, South, center;
        public float width, height;
    }
    private void Start()
    {
        Initiate();
    }
    private void Update()
    {
        DynamicMazeGenerator();
    }
    public void Initiate()
    {
        float _width_number = square_width_building_number - 1;
        float _width = (_width_number * building_interval);
        float _width_half = _width / 2;

        InitiateBoundary(ref maze_boundary, Vector3.zero, _width + maze_width_threshold, _width + maze_width_threshold);//initial maze boundary
        InitiateBoundary(ref player_boundary, Vector3.zero, building_interval, building_interval);//initial player boundary

        Player = GameObject.Find("Player");

        player_boundary_visulizer.transform.position = player_boundary.center;
        maze_boundary_visulizer.transform.position = maze_boundary.center;
        maze_boundary_visulizer.transform.localScale = new Vector3(_width + maze_width_threshold, 1, _width + maze_width_threshold);

        GenerateMaze();
    }
    void InitiateBoundary(ref Boundary _boundary, Vector3 _center, float _width, float _height)
    {
        _boundary.center = _center;

        _boundary.width = _width;
        _boundary.height = _height;

        _boundary.North = _center + Vector3.forward * (_height / 2);
        _boundary.East = _center + Vector3.right * (_width / 2);
        _boundary.West = _center + Vector3.left * (_width / 2);
        _boundary.South = _center + Vector3.back * (_height / 2);
    }
    public void DynamicMazeGenerator()
    {
        Vector3 playerMovingDir = Vector3.zero;
        float _width_number = square_width_building_number - 1;
        float _width = (_width_number * building_interval);
        float _width_half = _width / 2;
        //Direction = moveDirection.none;
        //Initiate boundary
        if (!isPointInBoundary(player_boundary, Player.transform.position))
        {

            playerMovingDir = CheckMoveDirection();
            //check if every buildings are still staty in boundary

            InitiateBoundary(ref player_boundary, player_boundary.center + playerMovingDir * building_interval, building_interval, building_interval);//initial player boundary
            InitiateBoundary(ref maze_boundary, player_boundary.center, _width + maze_width_threshold, _width + maze_width_threshold);//initial maze boundary

            player_boundary_visulizer.transform.position = player_boundary.center;
            maze_boundary_visulizer.transform.position = maze_boundary.center;
            maze_boundary_visulizer.transform.localScale = new Vector3(_width + maze_width_threshold, 1, _width + maze_width_threshold);
        }

        var _select_building = (from _building in building_pool
                                where !isPointInBoundary(maze_boundary, _building.transform.position)
                                select _building).ToList();

        foreach (var item in _select_building)
        {
            Vector3 translate_vector = playerMovingDir * (square_width_building_number * building_interval);
            item.transform.position += translate_vector;
        }


    }

    public Vector3 CheckMoveDirection()
    {
        Vector3 playerMovingDir = Vector3.zero;
        playerMovingDir = (Player.transform.position - player_boundary.center).normalized;
        if (Mathf.Abs(Vector3.Dot(playerMovingDir, Vector3.right) - 1) < 0.1f)
        {
            Direction = moveDirection.right;
        }
        else if (Mathf.Abs(Vector3.Dot(playerMovingDir, Vector3.left) - 1) < 0.1f)
        {
            Direction = moveDirection.left;
        }
        else if (Mathf.Abs(Vector3.Dot(playerMovingDir, Vector3.back) - 1) < 0.1f)
        {
            Direction = moveDirection.back;
        }
        else if (Mathf.Abs(Vector3.Dot(playerMovingDir, Vector3.forward) - 1) < 0.1f)
        {
            Direction = moveDirection.forward;

        }
        return playerMovingDir;
    }
    public bool isPointInBoundary(Boundary _boundary, Vector3 point)
    {
        bool result =
            (point.x <= _boundary.East.x && point.x >= _boundary.West.x) &&
            (point.z <= _boundary.North.z && point.z >= _boundary.South.z);

        return result;
    }

    //process controller
    public void GenerateMaze()
    {
        //generate parents
        parents = new GameObject();
        parents.transform.parent = this.transform;
        parents.name = "pool";

        //generate building pool
        GenerateBuildingPool();

        //place building to maze position
        RePositionBuiding();

        //set prefabs active false
        foreach (var g in building_prefabs)
        {
            g.SetActive(false);
        }
    }

    public void RePositionBuiding()
    {
        int index = 0;

        float _width_number = square_width_building_number - 1;
        float _width = (_width_number * building_interval);
        float _width_half = _width / 2;

        for (float x = -_width_half; x <= _width_half; x += building_interval)
        {
            for (float z = -_width_half; z <= _width_half; z += building_interval)
            {
                building_pool[index].SetActive(true);
                building_pool[index].transform.position = new Vector3(x, building_pool[index].transform.position.y, z);
                index += 1;
            }
        }

    }
    public void GenerateBuildingPool()
    {
        for (int i = 0; i < Math.Pow(square_width_building_number, 2); i++)
        {
            building_pool.Add(GenerateBuilding());
        }

    }
    public GameObject GenerateBuilding()
    {

        GameObject clone = Instantiate(building_prefabs[RandomPickOne(building_prefabs.Count)]) as GameObject;
        clone.name = "building";
        clone.transform.parent = parents.transform;
        clone.SetActive(false);
        return clone;

    }
    public int RandomPickOne(int length)
    {
        if (length == 0) return 0;
        int index = UnityEngine.Random.Range(0, length - 1); ;
        return index;

    }
    private void OnGUI()
    {
        if (isUnitTest)
        {
            Rect _player_position = new Rect(100, 100, 300, 100);
            Rect _player_boundary = new Rect(100, 150, 300, 100);
            Rect _is_still_stay_inboundary = new Rect(100, 200, 300, 100);

            string player_position_content = "[Player position]" + Player.transform.position;
            string player_boundary_content = "[player boundary center]" + player_boundary.center + "n/" +
                                             "[player boundary east]" + player_boundary.East;
            string is_still_stay_inboundary_contetn = "[is_still_stay_inboundary]" + isPointInBoundary(player_boundary, Player.transform.position);

            GUI.Label(_player_position, player_position_content);
            GUI.Label(_player_boundary, player_boundary_content);
            GUI.Label(_is_still_stay_inboundary, is_still_stay_inboundary_contetn);

        }
    }
}