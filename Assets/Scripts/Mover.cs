using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [Header("Motion Attributes")]
    [Range(0, 5)] public float turnSpeed = 1;
    [Range(0, 5)] public float moveSpeed = 1;

    Route current_route;

    float _position;//the position record
    float _rotation;
    int previousPositionIndex;
    int totalDestinationNumber;

    //Rigidbody rigidbody;

    private void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        _move();
    }
    public void startJourney(Route _route)
    {
        current_route = _route;

        totalDestinationNumber = current_route.get_route_vertex.Length;
        transform.forward = current_route.get_route_direction[0];
        transform.position = current_route.get_route_vertex[0];

        _position = 0f;
        _rotation = 0f;
        previousPositionIndex = 0;
    }

    public void isTraveling()
    {
        if (current_route == null) return;

        //------------------------------------------------------------------------------------
        int index = previousPositionIndex + 1;
        if (current_route.get_route_vertex.Length - 1 < previousPositionIndex + 1) index -= 1;

        Vector3 nextPosition = current_route.get_route_vertex[index];
        float distanceToEndingPosition = Vector3.Distance(transform.position, nextPosition);
        //------------------------------------------------------------------------------------

        if (distanceToEndingPosition > 0.001f) return;

        _position = 0;
        _rotation = 0;

        //check if travel is finished
        if (previousPositionIndex == totalDestinationNumber - 1 )
        {
            //Debug.Log("Travel ending");
            current_route = null;
        }
        //check segment ending 
        else if (previousPositionIndex != totalDestinationNumber - 1)
        {
            //Debug.Log("Segment ending");
            previousPositionIndex = index;
        }

        
    }

    public void _move()
    {
        

        //check if ending
        isTraveling();
        if (current_route == null) return;
        int direction_index = previousPositionIndex < current_route.get_route_direction.Length ? previousPositionIndex : current_route.get_route_direction.Length - 1;

        if (transform.forward != current_route.get_route_direction[direction_index])
        {
            //turn to right angle
            Vector3 previous_direction = current_route.get_route_direction[previousPositionIndex-1];
            Vector3 next_forward = current_route.get_route_direction[previousPositionIndex];

            float speed = turnSpeed * Time.deltaTime;
            _rotation += speed;
            _rotation = Mathf.Clamp(_rotation, 0, 1);

            transform.forward = Vector3.Slerp(previous_direction, next_forward, _rotation);
        }
        else
        {
            //move
            if (previousPositionIndex == current_route.get_route_vertex.Length - 1) return;

            Vector3 previousPosition = current_route.get_route_vertex[previousPositionIndex];
            Vector3 nextPosition = current_route.get_route_vertex[previousPositionIndex + 1];

            float speed = moveSpeed * Time.deltaTime / (Vector3.Distance(previousPosition, nextPosition));
            _position += speed;
            _position = Mathf.Clamp(_position, 0, 1);

            transform.position = Vector3.Lerp(previousPosition, nextPosition, _position);
            //rigidbody.MovePosition(Vector3.Lerp(previousPosition, nextPosition, _position));
        }

        

    }

}