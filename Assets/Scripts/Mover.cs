using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JourneyStage { Start, OnJourney, OnJourney_onPause, OnJourney_goStraight, OnJourney_turnAround, end }
public enum JourneyType { go, back }
//[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    [Header("Motion Attributes")]
    [Range(0, 5)] public float turnSpeed = 1;
    [Range(0, 5)] public float moveSpeed = 1;

    [Tooltip("暫停有沒有時間限制")] public bool isTimelimitPause = false;
    [Tooltip("暫停時：時間倒數幾秒開始")] public float PauseTime = 5f;

    Route current_route;

    float pauseTimer = 0f;
    float _position;//the position record
    float _rotation;
    int previousPositionIndex;
    int totalDestinationNumber;
    public Vector3 get_nextDirection
    {
        get
        {
            return current_route.get_route_direction[getNextDirectionIndex()];
        }
    }

    public Vector3 get_previousDirection
    {
        get
        {
            return current_route.get_route_direction[getNextDirectionIndex()-1];
        }
    }
    public int get_previousPositionIndex{
        get{
            return getNextDirectionIndex();
        }
    }
    public int get_reverse_previousPositionIndex{
        get{
            return current_route.get_total_length - getNextDirectionIndex();
        }
    }

    [SerializeField]JourneyStage journey_stage;
    public JourneyStage isOnJourney { get { return journey_stage; } }

    [SerializeField]JourneyType _journeyType;
    public JourneyType journeyType { get { return _journeyType; } }

    //Rigidbody rigidbody;

    private void Start()
    {
        //rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        _move();
    }
    #region main function
    public void startJourney(Route _route, JourneyType journeyType)
    {

        current_route = _route;
        _journeyType = journeyType;

        totalDestinationNumber = current_route.get_route_vertex.Length;

        if(_journeyType == JourneyType.go) transform.forward = current_route.get_route_direction[0];
        transform.position = current_route.get_route_vertex[0];

        _position = 0f;
        _rotation = 0f;
        previousPositionIndex = 0;

        //setting journey stage start
        journey_stage = JourneyStage.Start;
    }

    public void _move()
    {


        //check if ending
        isTraveling();
        if (current_route == null) return;
        int direction_index = previousPositionIndex < current_route.get_route_direction.Length ? previousPositionIndex : current_route.get_route_direction.Length - 1;

        move_checkStatus(direction_index);
        move_stateMachine();


    }
    #endregion

    #region sub function
    public void isTraveling()
    {
        if (current_route == null) return;

        //double check index is correct
        //------------------------------------------------------------------------------------

        int index = previousPositionIndex + 1;
        if (current_route.get_route_vertex.Length - 1 < previousPositionIndex + 1) index -= 1;

        Vector3 nextPosition = current_route.get_route_vertex[index];
        float distanceToEndingPosition = Vector3.Distance(transform.position, nextPosition);
        
        journey_stage = JourneyStage.OnJourney;//setting journey stage processing
        //------------------------------------------------------------------------------------


        //if player isn't arrive at end of route segment
        if (distanceToEndingPosition > 0.001f) return;

        //if player at end of route segment
        //------------------------------------------------------------------------------------
        _position = 0;
        _rotation = 0;

        //check if travel is finished
        if (previousPositionIndex == totalDestinationNumber - 1 )
        {
            //Debug.Log("Travel ending");
            current_route = null;

            //setting journey stage end
            journey_stage = JourneyStage.end;
            if (_journeyType == JourneyType.back)
            {
                _journeyType = JourneyType.go;
            }else
            {
                _journeyType = JourneyType.back;
            }
        }
        //check segment ending 
        else if (previousPositionIndex != totalDestinationNumber - 1)
        {
            //Debug.Log("Segment ending");
            previousPositionIndex = index;

            //only pause when travel is back
            if(isNextStageIsTurn(getNextDirectionIndex()))PauseMove();
        }

        //------------------------------------------------------------------------------------

    }
    void move_checkStatus(int direction_index)
    {
        //if player at begin or end of journey
        if (journey_stage != JourneyStage.OnJourney) return;

        //check if the conditions are met
        bool isNeedToTurn = transform.forward != current_route.get_route_direction[direction_index];
        bool isPause = pauseTimer > 0.001 || pauseTimer == -1;

        if (isPause) journey_stage = JourneyStage.OnJourney_onPause;
        else if (isNeedToTurn) journey_stage = JourneyStage.OnJourney_turnAround;
        else journey_stage = JourneyStage.OnJourney_goStraight;

    }

    void move_stateMachine()
    {
        switch (journey_stage)
        {
            case JourneyStage.OnJourney_goStraight:
                go_straight(previousPositionIndex);
                break;

            case JourneyStage.OnJourney_turnAround:
                do_rotate(previousPositionIndex);
                break;

            case JourneyStage.OnJourney_onPause:
                if (pauseTimer == -1) return;
                else if (pauseTimer > 0.01) pauseTimer -= Time.deltaTime;
                else CancelPause();
                break;

            default:
                break;
        }
    }
    #endregion
    #region utilities tool
    void go_straight(int index)
    {
        //move
        if (index == current_route.get_route_vertex.Length - 1) return;

        Vector3 previousPosition = current_route.get_route_vertex[index];
        Vector3 nextPosition = current_route.get_route_vertex[index + 1];

        float speed = moveSpeed * Time.deltaTime / (Vector3.Distance(previousPosition, nextPosition));
        _position += speed;
        _position = Mathf.Clamp(_position, 0, 1);

        transform.position = Vector3.Lerp(previousPosition, nextPosition, _position);

    }

    void do_rotate(int index)
    {
        //turn to right angle
        //compare two direction
        Vector3 next_forward = current_route.get_route_direction[index];
        Vector3 previous_direction = index == 0 ?
            new Vector3(-next_forward.x, next_forward.y, -next_forward.z) :
            current_route.get_route_direction[index - 1];

        //do roate 
        float _speed = turnSpeed * Time.deltaTime;
        _rotation += _speed;
        _rotation = Mathf.Clamp(_rotation, 0, 1);

        transform.forward = Vector3.Slerp(previous_direction, next_forward, _rotation);

    }
    bool isNextStageIsTurn(int direction_index)
    {
        return transform.forward != current_route.get_route_direction[direction_index];
    }
    int getNextDirectionIndex()
    {
        return previousPositionIndex < current_route.get_route_direction.Length ? previousPositionIndex : current_route.get_route_direction.Length - 1;
    }
    #endregion
    #region pause function
    //when pause timer = 0 
    public void CancelPause()
    {
        pauseTimer = 0f;
    }

    public void PauseMove()
    {
        if (journeyType != JourneyType.back) return;
        if (isTimelimitPause) pauseTimer = PauseTime;
        else pauseTimer = -1;

    }
    #endregion

    

}