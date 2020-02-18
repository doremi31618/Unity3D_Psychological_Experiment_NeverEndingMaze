using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator),typeof(LineRenderer))]
public class RouteGenerator : MonoBehaviour
{
    [Header("Adjustable Attribute")]
    [Range(3, 10)] public int total_length = 3;
    [Range(0, 10)] public int total_rotate_time = 1;

    [Header("Component setting")]
    public GUISkin gUISkin;
    public bool isUseVisalizer = true;
    public bool isUnitTest = true;


   [SerializeField]Route current_route;
    public Route get_CurrentRoute{ get{ return current_route; } }

    MazeGenerator m_maze;
    LineRenderer m_lineRenderer;

    public void Start()
    {
        //intital attributes
        m_maze = GetComponent<MazeGenerator>();
        m_lineRenderer = GetComponent<LineRenderer>();

        //generate at first 
        //Generate();
    }
    

    public void RouteVisualizer(Route _route)
    {
        m_lineRenderer.SetPositions(new Vector3[0]);
        m_lineRenderer.positionCount = _route.get_route_vertex.Length;
        m_lineRenderer.SetPositions(_route.get_route_vertex);
    }

    public void Generate()
    {
        float interval = m_maze.building_interval;
        Vector3 playerPosition = m_maze.Player.transform.position;
        Vector3 firstDirection = Vector3.forward;

        current_route  = new Route(total_length, total_rotate_time, firstDirection, playerPosition, interval);

        m_lineRenderer.enabled = isUseVisalizer;
        if (isUseVisalizer) RouteVisualizer(current_route);

    }

    private void OnGUI()
    {
        GUI.skin = gUISkin;
        if(isUnitTest)
        {
            GUI.Box(new Rect(50, 75, 350, 200), "");

            GUI.Label(new Rect(50, 75, 100, 50), "Route length : " + total_length);
            total_length = (int)GUI.HorizontalSlider(new Rect(100, 100, 200, 50), total_length,3,15);

            GUI.Label(new Rect(50, 125, 100, 50), "Rotate times : " + total_rotate_time);
            total_rotate_time = (int)GUI.HorizontalSlider(new Rect(100, 150, 200, 50), total_rotate_time, 0, total_length-1);

            if(GUI.Button(new Rect(100, 200, 150, 50), "Generate Route"))
            {
                Generate();
            }
        }

    }


}
[System.Serializable]
public class Route
{
    int total_length;
    int total_rotate_time;

    //for outside read 
    public int get_total_length{get { return total_length; }}
    public int get_rotate_length { get { return total_rotate_time; } }

    //input value
    Vector3 first_dir;
    Vector3 first_position;
    float interval;

    //output value
    public bool[] route;
    public Vector3[] route_direction;
    public Vector3[] route_vertex;

    public Route getBackTravelRoute { get { return backTravelRouteGenerator();  } }
    public bool[] get_route { get { return route; } }
    public Vector3[] get_route_direction { get { return route_direction; } }
    public Vector3[] get_route_vertex { get { return route_vertex; } }

    public bool isRotateInFirstTime;
    public Route(int _length, int _rotate_time,Vector3 _first_dir, Vector3 _first_pos, float _interval)
    {
        total_length = _length;
        total_rotate_time = _rotate_time;
        first_dir = _first_dir.normalized;
        first_position = _first_pos;
        interval = _interval;

        route = new bool[total_length];
        route_direction = new Vector3[total_length];
        route_vertex = new Vector3[total_length + 1];
        isRotateInFirstTime = false;

        RouteGenerator();
    }

    public void RouteGenerator()
    {
        int _length = total_length;
        int _rotate = total_rotate_time;//rotate times 
        int _straight = _length - _rotate;//straight times 

        for (int i=0; i<total_length; i++ )
        {
            if (i == 0) {
                route[0] = isRotateInFirstTime;
                _straight = isRotateInFirstTime ? _straight - 1 : _straight;
            }
            else {
                //Debug.Log("straight times : " + _straight + " ; rotate times : " + _rotate);
                route[i] = DeterminRotateOrStraight(ref _straight,ref _rotate);
            }

        }

        for(int i=0; i< route.Length; i++)
        {
            Vector3 dir = new Vector3();
            
            if (i == 0)
            {
                dir = first_dir;
                route_vertex[0] = first_position;
                route_vertex[1] = first_position + first_dir * interval;
            }
            else {
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
                    bool random_range = Random.value >= 0.5f;

                    dir = random_range ?
                        Vector3.Cross(last_dir, Vector3.up) :
                        Vector3.Cross(last_dir, Vector3.down);


                }
                route_vertex[i+1] = route_vertex[i] + dir * interval;

            }
            route_direction[i] = dir;
            
        }
    }
    Route backTravelRouteGenerator()
    {
        Vector3 back_route_pos = new Vector3();
        Vector3 back_route_firstDir = new Vector3();
        Route back_route = new Route(total_length, total_rotate_time, back_route_firstDir, back_route_pos,interval);
        return back_route;
    }

    //"true" is mean we need to rotate
    //"false" is mean we need to go straight
    bool DeterminRotateOrStraight(ref int _straight,ref int _rotate)
    {
        int range = Random.Range(0, _straight + _rotate);

        if(range > _rotate) _straight -= 1;
        else  _rotate -= 1;

        return (range <= _rotate);

    }

}
