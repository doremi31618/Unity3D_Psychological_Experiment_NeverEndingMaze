using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//EPPlus tutorial
//https://www.codebyamir.com/blog/create-excel-files-in-c-sharp
//https://dotblogs.com.tw/marcus116/2015/06/20/151604
//https://einboch.pixnet.net/blog/post/272950850-使用epplus產生excel-2007-2010檔案

[RequireComponent(typeof(PlayerDataRecorder))]
public class GameManager : MonoBehaviour
{
    MazeGenerator m_maze;
    RouteGenerator m_route_collector;
    PlayerDataRecorder m_recorder;
    [Header("Player Attributes")]
    public Mover player;
    [Range(0, 5)] public float turnSpeed = 1;
    [Range(0, 5)] public float moveSpeed = 1;

    [Header("GUI setting")]
    public GUISkin guiSkin;

    public Button goTravel;
    public Button backButton;
    public Button leftButton;
    public Button rightButton;
    public Button ExportButton;


    public bool isUseGUI = true;

    private void Start()
    {
        m_maze = GetComponent<MazeGenerator>();
        m_route_collector = GetComponent<RouteGenerator>();
        m_recorder = GetComponent<PlayerDataRecorder>();

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Mover>();
            player.turnSpeed = turnSpeed;
            player.moveSpeed = moveSpeed;
        }

        add_buttonListener();
    }

    private void Update()
    {
        ListenPlayerState();
        if (player != null)
        {
            player.turnSpeed = turnSpeed;
            player.moveSpeed = moveSpeed;
        }
    }
    void add_buttonListener()
    {
        if(goTravel!=null)
        {
            goTravel.onClick.AddListener(Generate);
        }

        if(backButton != null)
        {
            backButton.onClick.AddListener(GoBack);

        }

        if(leftButton != null)
        {
            leftButton.onClick.AddListener(left_direction);
        }

        if(rightButton != null)
        {
            rightButton.onClick.AddListener(right_direction);
        }

        if(ExportButton != null)
        {
            ExportButton.onClick.AddListener(Export_data_to_Excel);
        }
        
    }
    void Export_data_to_Excel()
    {
        m_recorder.ExportPlayerData();
    }

    //need to optimized (next version will update to event mechanism)
    void ListenPlayerState()
    {
        if (goTravel != null && backButton != null)
        {
            //if(player.journeyType==)
            switch (player.journeyType)
            {

                case JourneyType.go:
                    if (player.isOnJourney == JourneyStage.OnJourney ||
                       player.isOnJourney == JourneyStage.OnJourney_goStraight ||
                       player.isOnJourney == JourneyStage.OnJourney_onPause ||
                       player.isOnJourney == JourneyStage.OnJourney_turnAround)
                    {
                        goTravel.interactable = false;
                        backButton.interactable = false;
                        ExportButton.interactable = false;
                        leftButton.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        goTravel.interactable = true;
                        backButton.interactable = false;
                        ExportButton.interactable = true;
                    }
                   
                    break;

                case JourneyType.back:
                    if (player.isOnJourney == JourneyStage.OnJourney ||
                      player.isOnJourney == JourneyStage.OnJourney_goStraight ||
                      player.isOnJourney == JourneyStage.OnJourney_turnAround)
                    {
                        goTravel.interactable = false;
                        backButton.interactable = false;
                        ExportButton.interactable = false;
                        leftButton.transform.parent.gameObject.SetActive(false);
                       
                    }
                    else if(player.isOnJourney == JourneyStage.OnJourney_onPause )
                    {
                        leftButton.transform.parent.gameObject.SetActive(true);
                    }
                    else
                    {
                        backButton.interactable = true;
                        goTravel.interactable = false;
                        
                    }
                    break;
                default://JourneyStage.OnJourney || JourneyStage.OnJourney_onPause || JourneyStage.OnJourney_turnAround
                    goTravel.interactable = false;
                    backButton.interactable = false;
                    leftButton.transform.parent.gameObject.SetActive(false);
                    break;
            }
        }
    }
   

    //player control event - choose left direciton 
    void left_direction()
    {
        PlayerChooseDirection(-1);
    }

    //player control event - choose right direciton 
    void right_direction()
    {
        PlayerChooseDirection(1);
    }

    //record player current position & choise made by player 
    void PlayerChooseDirection(int dir)
    {
        //record player choise
        Vector3 next_direction=player.get_nextDirection;
        Vector3 previous_direction = player.get_previousDirection;
        int current_index=player.get_reverse_previousPositionIndex;

        //match answer
        Vector3 player_choise_direction=Vector3.Cross(previous_direction,Vector3.up * dir);
        // float angle_between_choise_direction_and_next_direction = Vector3.Angle(next_direction,player_choise_direction);
        
        int isAnswerCorrect = (Vector3.Angle(next_direction,player_choise_direction) < 5 ) ? 0 : 1;
        m_recorder.RecordPlayerChoise(current_index,isAnswerCorrect,dir);
        //unlock pause
        player.CancelPause();
    }

    void GoBack()
    {
        m_route_collector.get_CurrentRoute = m_route_collector.get_CurrentRoute.DeepClone();
        m_route_collector.get_CurrentRoute.back_RouteGenerator();
        
        
        //m_route_collector.get_CurrentRoute
        player.startJourney(m_route_collector.get_CurrentRoute, JourneyType.back);
    }

    void Generate()
    {

        m_route_collector.Generate();
        Route current_route = m_route_collector.get_CurrentRoute;

        player.startJourney(current_route, JourneyType.go);
        m_recorder.RecordRouteData(current_route);

    }

    private void OnGUI()
    {
        if(isUseGUI)
        {
            GUI.Box(new Rect(50, 75, 350, 200), "");

            GUI.Label(new Rect(50, 75, 100, 50), "Route length : " + m_route_collector.total_length);
            m_route_collector.total_length = (int)GUI.HorizontalSlider(new Rect(100, 100, 200, 50), m_route_collector.total_length, 3, 20);

            GUI.Label(new Rect(50, 125, 100, 50), "Rotate times : " + m_route_collector.total_rotate_time);
            m_route_collector.total_rotate_time = (int)GUI.HorizontalSlider(new Rect(100, 150, 200, 50), m_route_collector.total_rotate_time, 0, m_route_collector.total_length - 1);

            if (goTravel == null)
            {
                if (GUI.Button(new Rect(100, 200, 150, 50), "Generate Route"))
                {
                    Generate();
                }
            }
        }
    }


}
