using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    void ListenPlayerState()
    {
        if (goTravel != null && backButton != null)
        {
            switch (player.isOnJourney)
            {

                case JourneyStage.Start:
                    goTravel.interactable = true;
                    backButton.interactable = false;
                    break;

                case JourneyStage.end:
                    backButton.interactable = true;
                    goTravel.interactable = false;
                    break;

                default://JourneyStage.OnJourney || JourneyStage.OnJourney_onPause || JourneyStage.OnJourney_turnAround
                    goTravel.interactable = false;
                    backButton.interactable = false;
                    break;
            }
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

        
    }

    void GoBack()
    {
        player.startJourney(m_route_collector.get_CurrentRoute.getBackTravelRoute,JourneyType.back);
        
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
            m_route_collector.total_length = (int)GUI.HorizontalSlider(new Rect(100, 100, 200, 50), m_route_collector.total_length, 3, 15);

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
