using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(MazeGenerator), typeof(LineRenderer))]
public class RouteGenerator : MonoBehaviour
{
    [Header("Adjustable Attribute")]
    [Range(3, 20)] public int total_length = 14;
    [Range(0, 19)] public int total_rotate_time = 6;

    [Header("Component setting")]
    public GUISkin gUISkin;
    public bool isUseVisalizer = true;
    public bool isUnitTest = true;


    [SerializeField] Route current_route;
    public Route get_CurrentRoute
    {
        get
        {
            return current_route;
        }
        set
        {
            current_route = value;
        }
    }

    MazeGenerator m_maze;
    LineRenderer m_lineRenderer;

    public void Start()
    {
        //intital attributes
        // m_maze = GetComponent<MazeGenerator>();
        // m_lineRenderer = GetComponent<LineRenderer>();

        
        //generate at first 
        //Generate();
    }
    public void Initiate()
    {
        m_maze = GetComponent<MazeGenerator>();
        m_lineRenderer = GetComponent<LineRenderer>();
    }
    
    public void RouteVisualizer(Route _route)
    {
        m_lineRenderer.SetPositions(new Vector3[0]);
        m_lineRenderer.positionCount = _route.get_route_vertex.Length;
        m_lineRenderer.SetPositions(_route.get_route_vertex);
    }

    public void Generate(bool _isManual)
    {
        float interval = m_maze.building_interval;
        Vector3 playerPosition = m_maze.Player.transform.position;
        Vector3 firstDirection = Vector3.forward;

        
        current_route = new Route(total_length, total_rotate_time, firstDirection, playerPosition, interval, _isManual);

        m_lineRenderer.enabled = isUseVisalizer;
        if (isUseVisalizer) RouteVisualizer(current_route);

    }

    private void OnGUI()
    {
        GUI.skin = gUISkin;
        if (isUnitTest)
        {
            GUI.Box(new Rect(50, 75, 350, 200), "");

            GUI.Label(new Rect(50, 75, 100, 50), "Route length : " + total_length);
            total_length = (int)GUI.HorizontalSlider(new Rect(100, 100, 200, 50), total_length, 3, 15);

            GUI.Label(new Rect(50, 125, 100, 50), "Rotate times : " + total_rotate_time);
            total_rotate_time = (int)GUI.HorizontalSlider(new Rect(100, 150, 200, 50), total_rotate_time, 0, total_length - 1);

            if (GUI.Button(new Rect(100, 200, 150, 50), "Generate Route"))
            {
                Generate(true);
            }
        }

    }


}
[System.Serializable]
public class Route
{
    //input value
    [SerializeField] int total_length;
    [SerializeField] int total_rotate_time;
    [SerializeField] bool isManual = false;
    Vector3 first_dir;
    Vector3 first_position;
    float interval;
    public bool isRotateInFirstTime;

    //go route value
    [SerializeField] bool[] route;
    [SerializeField] int[] player_choisies;// -1 :not choise ||  0:false || 1:correct
    [SerializeField] int[] player_choisies_direction;// -1 :not choise ||  0:false || 1:correct
    [SerializeField] Vector3[] route_direction;
    [SerializeField] Vector3[] route_vertex;

    //for outside call - get Array List
    public bool[] get_route { get { return route; } }
    public int[] get_player_choisies { get { return player_choisies; } }
    public int[] get_player_choisies_direction { get { return player_choisies_direction; } }
    public Vector3[] get_route_direction { get { return route_direction; } }
    public Vector3[] get_route_vertex { get { return route_vertex; } }

    //for outside call - get route attribute
    public int get_total_length { get { return get_route.Length; } }
    public int get_rotate_length
    {
        get
        {
            return total_rotate_time;
        }
        set
        {
            total_rotate_time = value;
        }
    }
    public void setPlayerChoise(int index, int playerChoise, int playerChoiseDireciton)
    {
        player_choisies[index] = playerChoise;
        player_choisies_direction[index] = playerChoiseDireciton;
    }

    public Route(int _length, int _rotate_time, Vector3 _first_dir, Vector3 _first_pos, float _interval, bool _isManual)
    {
        total_length = _length;
        total_rotate_time = _rotate_time;
        first_dir = _first_dir.normalized;
        first_position = _first_pos;
        interval = _interval;

        route = new bool[total_length];
        player_choisies = new int[total_length];
        player_choisies_direction = new int[total_length];
        for (int i = 0; i < player_choisies.Length; i++)
        {
            player_choisies[i] = -1;
            player_choisies_direction[i] = 0;
        }//init player choise

        route_direction = new Vector3[total_length];
        route_vertex = new Vector3[total_length + 1];

        isRotateInFirstTime = false;
        isManual = _isManual;
        if(isManual)RouteGenerator();
        else RouteGenerator_ver2();
    }

    public void RouteGenerator_ver2()
    {

        Debug.Log("route generator ver2");
        int _length = total_length;
        int _rotate = total_rotate_time;//rotate times 
        int _straight = _length - _rotate;//straight times 

        int rotate_interval = _length / _rotate;
        int[] rotate_index = SpaceDistribute(_rotate,new Vector2Int(0,_length-1));

        for (int i = 0; i < total_length; i++)
        {
           route[i] = isValueInsideRange(i,rotate_index);
        }

        for (int i = 0; i < route.Length; i++)
        {
            Vector3 dir = new Vector3();

            if (i == 0)
            {
                dir = first_dir;
                route_vertex[0] = first_position;
                route_vertex[1] = first_position + first_dir * interval;
            }
            else
            {
                Vector3 last_dir = route_direction[i - 1];
                //go straight
                if (!route[i])
                {
                    dir = last_dir;
                }
                else
                //go rotate
                {
                    //true : turn right
                    //false : turn left
                    //randon determin turn_left of turn right;
                    bool random_range = UnityEngine.Random.value >= 0.5f;

                    dir = random_range ?
                        Vector3.Cross(last_dir, Vector3.up) :
                        Vector3.Cross(last_dir, Vector3.down);


                }
                route_vertex[i + 1] = route_vertex[i] + dir * interval;

            }
            route_direction[i] = dir;

        }

    }
    bool isValueInsideRange(int value, int[] value_range)
    {
        for(int i=0 ;i<value_range.Length ;i++)
        {
            if(value == value_range[i])return true;
        }
        return false;
    }
    int[] SpaceDistribute(int cut_number, Vector2Int boundary)
    {
        int space_total_area = Mathf.Abs((int)(boundary.y - boundary.x) + 1);

        int[] cut_space_area = new int[cut_number + 1];
        int extra_bonus =0;
        for (int i = 0; i < cut_space_area.Length; i++)
        {
            int rnd_area;
            float random_range = UnityEngine.Random.Range(1,100);

            if(random_range>84 && extra_bonus!=0)rnd_area=1;
            else if(random_range>16 && random_range<84)rnd_area=0;
            else rnd_area=-1;

            extra_bonus -= rnd_area;

            cut_space_area[i] = 2 + rnd_area;
        }
        // print(array_sapce);
        int lastIndex = 0;
        int[] cut_index = new int[cut_number];
        for(int i=0; i<cut_space_area.Length-1; i++)
        {
            cut_index[i] = lastIndex + cut_space_area[i];
            lastIndex = cut_index[i];
        }
        return cut_index;
    }

    public void RouteGenerator()
    {
        Debug.Log("route generator ver1");
        int _length = total_length;
        int _rotate = total_rotate_time;//rotate times 
        int _straight = _length - _rotate;//straight times 

        for (int i = 0; i < total_length; i++)
        {
            if (i == 0)
            {
                route[0] = false;
                _straight = _straight - 1;
                // isRotateInFirstTime ? _straight - 1 : _straight;
            }
            else
            {
                //Debug.Log("straight times : " + _straight + " ; rotate times : " + _rotate);
                bool result = DeterminRotateOrStraight(ref _straight, ref _rotate);
                route[i] = result;
                // Debug.Log(" route : "+route[i] +" result : " + result);
            }

        }

        for (int i = 0; i < route.Length; i++)
        {
            Vector3 dir = new Vector3();

            if (i == 0)
            {
                dir = first_dir;
                route_vertex[0] = first_position;
                route_vertex[1] = first_position + first_dir * interval;
            }
            else
            {
                Vector3 last_dir = route_direction[i - 1];
                //go straight
                if (!route[i])
                {
                    dir = last_dir;
                }
                else
                //go rotate
                {
                    //true : turn right
                    //false : turn left
                    //randon determin turn_left of turn right;
                    bool random_range = UnityEngine.Random.value >= 0.5f;

                    dir = random_range ?
                        Vector3.Cross(last_dir, Vector3.up) :
                        Vector3.Cross(last_dir, Vector3.down);


                }
                route_vertex[i + 1] = route_vertex[i] + dir * interval;

            }
            route_direction[i] = dir;

        }
    }


    //"true" is mean we need to rotate
    //"false" is mean we need to go straight
    bool DeterminRotateOrStraight(ref int _straight, ref int _rotate)
    {
        if (_rotate < 0) _rotate = 0;

        int range = Mathf.RoundToInt(UnityEngine.Random.Range(0.5f, _straight + _rotate));
        bool result = (range > _rotate);
        // Debug.Log("Result : " + !(result)+" _range : "+range +" _straight : "+ _straight + " _rotate : " + _rotate) ;


        if (result) _straight -= 1;
        else _rotate -= 1;


        return !(result);

    }

    public void back_RouteGenerator()
    {
        reverseArray(ref route_vertex, 0, route_vertex.Length - 1);
        reverseArray(ref route_direction, 0, route_direction.Length - 1);

        for (int i = 0; i < route_direction.Length; i++)
        {
            route_direction[i] = new Vector3(-route_direction[i].x, route_direction[i].y, -route_direction[i].z);
        }
    }

    void reverseArray(ref Vector3[] arr, int start, int end)
    {
        Vector3 temp;

        while (start < end)
        {
            temp = arr[start];
            arr[start] = arr[end];
            arr[end] = temp;
            start++;
            end--;
        }
    }

    public void ResetRoute(int _rotateTimes, bool[] _route, Vector3[] _direction, Vector3[] _vertex)
    {
        _route.CopyTo(route, 0);
        _direction.CopyTo(route_direction, 0);
        _vertex.CopyTo(route_vertex, 0);
        total_rotate_time = _rotateTimes;
        total_length = route.Length;
    }

    public Route DeepClone()
    {
        Route new_route = new Route(total_length, total_rotate_time, Vector3.zero, Vector3.zero, interval,isManual);
        new_route.ResetRoute(total_rotate_time, route, route_direction, route_vertex);
        return new_route;
    }

}

