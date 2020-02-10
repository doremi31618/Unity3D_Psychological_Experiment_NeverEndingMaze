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
    public float maze_width = 100f;
    public float maze_height = 100f;

    [Header("Building Attribute")]
    public float building_interval = 5f;
    public float building_height = 10f;
    public int max_building_number = 500;

    [Header("other attribute")]
    public bool isGUI = true;

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
        public Vector3 North,East,West,South,center;
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

        InitiateBoundary(ref maze_boundary, Vector3.zero, maze_width, maze_height);//initial maze boundary
        InitiateBoundary(ref player_boundary, Vector3.zero, building_interval, building_interval);//initial player boundary

        Player = GameObject.Find("Player");

        player_boundary_visulizer.transform.position = player_boundary.center;
        maze_boundary_visulizer.transform.position = maze_boundary.center;

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
        //Direction = moveDirection.none;
        //Initiate boundary
        if (!isPointInBoundary(player_boundary, Player.transform.position)){

            playerMovingDir = CheckMoveDirection();

            InitiateBoundary(ref player_boundary, player_boundary.center + playerMovingDir, building_interval, building_interval);//initial player boundary
            InitiateBoundary(ref maze_boundary, player_boundary.center, maze_width, maze_height);//initial maze boundary

            player_boundary_visulizer.transform.position = player_boundary.center;
            maze_boundary_visulizer.transform.position = maze_boundary.center;
            //check collision
            var _select_building = (from _building in building_pool
                         where !isPointInBoundary(maze_boundary, _building.transform.position)
                         select _building).ToList();

            //reposition
            int index = 0;
            switch (Direction)
            {
                case moveDirection.right:

                    for (float z = maze_boundary.East.z - (maze_height - building_interval) / 2; z < maze_boundary.East.z + (maze_height + building_interval) / 2 && index < _select_building.Count; z += building_interval, index += 1)
                    {
                        _select_building[index].transform.position = new Vector3(
                            maze_boundary.East.x - building_interval/2,
                            _select_building[index].transform.position.y,
                            z);
                    }
                    break;

                case moveDirection.left:
                    for (float z = maze_boundary.West.z - (maze_height - building_interval) / 2; z < maze_boundary.West.z + (maze_height + building_interval) / 2 && index < _select_building.Count; z += building_interval, index += 1)
                    {
                        _select_building[index].transform.position = new Vector3(
                            maze_boundary.West.x + building_interval / 2,
                            _select_building[index].transform.position.y,
                            z);
                    }
                    break;

                case moveDirection.forward:
                    for (float x = maze_boundary.North.x - (maze_height - building_interval) / 2; x < maze_boundary.North.z + (maze_height + building_interval) / 2 && index < _select_building.Count; x += building_interval, index += 1)
                    {
                        _select_building[index].transform.position = new Vector3(
                            x,
                            _select_building[index].transform.position.y,
                            maze_boundary.North.z - building_interval / 2);
                    }
                    break;

                case moveDirection.back:
                    for (float x = maze_boundary.South.x - (maze_height - building_interval) / 2; x < maze_boundary.South.z + (maze_height + building_interval) / 2 && index < _select_building.Count; x += building_interval, index += 1)
                    {
                        _select_building[index].transform.position = new Vector3(
                            x,
                            _select_building[index].transform.position.y,
                            maze_boundary.South.z + building_interval / 2);
                    }
                    break;

            }

        }

    }
    public void RepositionBuilding(ref List<GameObject> select_buildings)
    {
        int index = 0;

        switch (Direction)
        {
            case moveDirection.right:
                
                for (float z=maze_boundary.East.z- (maze_height - building_interval) / 2; z <maze_boundary.East.z + (maze_height + building_interval) / 2 || index < select_buildings.Count; z += building_interval, index += 1)
                {
                    select_buildings[index].transform.position = new Vector3(
                        maze_boundary.East.x - building_interval,
                        select_buildings[index].transform.position.y,
                        z + building_interval);
                }
                break;

            //case moveDirection.left:
            //    for (float z = maze_boundary.West.z - (maze_height - building_interval) / 2; z < maze_boundary.West.z + (maze_height + building_interval) / 2 || index < select_buildings.Count; z += building_interval, index += 1)
            //    {
            //        select_buildings[index].transform.position = new Vector3(
            //            maze_boundary.West.x + building_interval,
            //            select_buildings[index].transform.position.y,
            //            z);
            //    }
            //    break;

            //case moveDirection.forward:
            //    for (float x = maze_boundary.North.x - (maze_height - building_interval) / 2; x < maze_boundary.North.z + (maze_height + building_interval) / 2 || index < select_buildings.Count; x += building_interval, index += 1)
            //    {
            //        select_buildings[index].transform.position = new Vector3(
            //            x,
            //            select_buildings[index].transform.position.y,
            //            maze_boundary.North.z - building_interval);
            //    }
            //    break;

            //case moveDirection.back:
            //    for (float x = maze_boundary.South.x - (maze_height - building_interval) / 2; x < maze_boundary.South.z + (maze_height + building_interval) / 2 ||index < select_buildings.Count; x += building_interval, index += 1)
            //    {
            //        select_buildings[index].transform.position = new Vector3(
            //            x,
            //            select_buildings[index].transform.position.y,
            //            maze_boundary.South.z);
            //    }
            //    break;

        }
    }
    public Vector3 CheckMoveDirection()
    {
        Vector3 playerMovingDir = Vector3.zero;
        if (Player.transform.position.x > player_boundary.East.x)
        {
            playerMovingDir = Vector3.right * building_interval;
            Direction = moveDirection.right;

        }
        else if (Player.transform.position.x < player_boundary.West.x)
        {
            playerMovingDir = Vector3.left * building_interval;
            Direction = moveDirection.left;
        }
        else if (Player.transform.position.z < player_boundary.South.z)
        {
            playerMovingDir = Vector3.back * building_interval;
            Direction = moveDirection.back;
        }
        else if (Player.transform.position.z > player_boundary.North.z)
        {
            playerMovingDir = Vector3.forward * building_interval;
            Direction = moveDirection.forward;
        }

        return playerMovingDir;
    }
    public bool isPointInBoundary(Boundary _boundary, Vector3 point)
    {
        bool result =
            (point.x < _boundary.East.x && point.x > _boundary.West.x) &&
            (point.z < _boundary.North.z && point.z > _boundary.South.z);
        
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
        foreach(var g in building_prefabs)
        {
            g.SetActive(false);
        }
    }
    public void RePositionBuiding()
    {
        int index=0;
        for(float x= -(maze_height- building_interval) / 2;x< (maze_height + building_interval) / 2;x+=building_interval)
        {
            for(float z=-(maze_height - building_interval) /2;z< (maze_height + building_interval) / 2;z+=building_interval)
            {
                building_pool[index].SetActive(true);
                building_pool[index].transform.position = new Vector3(x, building_pool[index].transform.position.y, z);
                index += 1;
            }
        }

    }
    public void GenerateBuildingPool()
    {
        for(int i=0; i< max_building_number;i++ )
        {
            building_pool.Add(GenerateBuilding());
        }

    }
    public GameObject GenerateBuilding()
    {

        GameObject clone = Instantiate(building_prefabs[RandomPickOne(building_prefabs.Count)]) as GameObject;
        clone.name = "building";
        clone.transform.parent = parents.transform;
        clone.transform.position += new Vector3(0,building_height / 2,0);
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
        if(isGUI)
        {
            Rect _player_position = new Rect(100, 100, 300, 100);
            Rect _player_boundary = new Rect(100, 150, 300, 100);
            Rect _is_still_stay_inboundary = new Rect(100, 200, 300, 100);

            string player_position_content = "[Player position]" + Player.transform.position;
            string player_boundary_content = "[player boundary center]" + player_boundary.center+ "n/"+
                                             "[player boundary east]" + player_boundary.East;
            string is_still_stay_inboundary_contetn = "[is_still_stay_inboundary]" + isPointInBoundary(player_boundary, Player.transform.position);

            GUI.Label(_player_position, player_position_content);
            GUI.Label(_player_boundary, player_boundary_content);
            GUI.Label(_is_still_stay_inboundary, is_still_stay_inboundary_contetn);

        }
    }
}