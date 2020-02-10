using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator))]
public class RouteGenerator : MonoBehaviour
{
    [Range(3,10)]public int total_length;
    [Range(0, 10)] public int total_rotate_time;

    public bool isUseVisalizer = true;
    public bool isUseGUI = true;


    public List<Route> route_collection;

    public void Start()
    {
        route_collection = new List<Route>();
    }

    public void RouteVisualizer(Route _route)
    {
        Vector3[] routePoint = _route.route_direction;
        //寫到這裡
    }

    public void Generate()
    {
        Route new_route = new Route(total_length, total_rotate_time, Vector3.forward);
        route_collection.Add(new_route);
    }

    private void OnGUI()
    {
        if(isUseGUI)
        {
            total_length = (int)GUI.HorizontalSlider(new Rect(100, 100, 200, 50), total_length,3,15);
            total_rotate_time = (int)GUI.HorizontalSlider(new Rect(100, 150, 200, 50), total_rotate_time, 0, 15);
            if(GUI.Button(new Rect(100, 200, 150, 50), "Generate Route"))
            {
                Generate();
            }
        }

    }


}
public class Route
{
    int total_length;
    int total_rotate_time;

    public int get_total_length{get { return total_length; }}
    public int get_rotate_length { get { return total_rotate_time; } }

    public bool[] route;
    public Vector3 first_dir;
    public Vector3[] route_direction;

    public bool isRotateInFirstTime = false;
    public Route(int _length, int _rotate_time,Vector3 _first_dir)
    {
        total_length = _length;
        total_rotate_time = _rotate_time;
        first_dir = _first_dir.normalized;

        route = new bool[total_length];
        route_direction = new Vector3[total_length];
        isRotateInFirstTime = false;
    }

    public void RouteGenerator()
    {
        int _length = total_length;
        int _rotate = total_rotate_time;
        int _straight = _length - _rotate;

        for (int i=0; i<total_length; i++ )
        {
            if (i == 0) {
                route[0] = isRotateInFirstTime;
                _straight = isRotateInFirstTime ? _straight : _straight - 1;
            }
            else {
                route[i] = DeterminRotateOrStraight(ref _straight,ref _rotate);
            }

        }

        for(int i=0; i< route.Length; i++)
        {
            Vector3 dir = new Vector3();
            
            if (i == 0)
            {
                dir = first_dir;
            }
            else {
                Vector3 last_dir = route_direction[i - 1];
                //go straight
                if (!route[i])
                {
                    dir = last_dir;
                }
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
            }

            route_direction[i] = dir;
        }
    }

    //"true" is mean we need to rotate
    //"false" is mean we need to go straight
    bool DeterminRotateOrStraight(ref int _straight,ref int _rotate)
    {
        int range = (int)Random.Range(0, _straight + _rotate);

        if(range > _rotate) _straight -= 1;
        else  _rotate -= 1;

        return !(range >= _rotate);

    }

}
