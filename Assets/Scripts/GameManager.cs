using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
//EPPlus tutorial
//https://www.codebyamir.com/blog/create-excel-files-in-c-sharp
//https://dotblogs.com.tw/marcus116/2015/06/20/151604
//https://einboch.pixnet.net/blog/post/272950850-使用epplus產生excel-2007-2010檔案
public enum GameMode { Constant, Landmark_2sides, Landmark_8sides, Manual }
[RequireComponent(typeof(PlayerDataRecorder))]
public class GameManager : MonoBehaviour
{
    MazeGenerator m_maze;
    RouteGenerator m_route_collector;
    PlayerDataRecorder m_recorder;
    UIManager m_UIManager;
    LandmarkManager m_landmark;
    public float version = 1.0f;
    public RandomModeGenorator m_random_mode_generator;
    public GameMode gameMode = GameMode.Constant;
    public bool isUseLandmark = false;
    public bool isUseRanodom_landmark_constant_mode = true;
    public int random_mode_index = 0;
    public bool change_random_mode { set { isUseRanodom_landmark_constant_mode = !isUseRanodom_landmark_constant_mode; } }
    [Header("Player Attributes")]
    public Mover player;

    [Range(0, 5)] public float turnSpeed = 1;
    [Range(0, 10)] public float moveSpeed = 1;

    [Header("GUI setting")]
    public GUISkin guiSkin;
    public bool isChangeValueManually = false;


    Vector3Int mode_process_index = Vector3Int.zero;
    public int get_current_constant_index
    {
        get
        {
            return mode_process_index.x;
        }
        set
        {
            mode_process_index.x = value;
        }
    }

    public int get_current_Landmark_2sides_index
    {
        get
        {
            return mode_process_index.y;
        }
        set
        {
            mode_process_index.y = value;
        }
    }

    public int get_current_Landmark_8sides_index
    {
        get
        {
            return mode_process_index.z;
        }
        set
        {
            mode_process_index.z = value;
        }
    }


    [Header("Slider")]
    public Slider Length_slider;
    public Slider Rotate_slider;
    public Slider speed_slider;
    public Slider rotate_speed_slider;

    [Header("Text")]
    public Text Length_text;
    public Text Rotate_text;
    public Text speed_text;
    public Text rotate_speed_text;
    public List<Text> version_title_text;
    [Header("Button")]
    public Button goTravel;
    public Button backButton;
    public Button leftButton;
    public Button straightButton;
    public Button rightButton;
    public Button ExportButton;

    public Button StartButton;
    public Button exitButton;
    [Header("Dropdown")]
    public Dropdown dropdown;

    public bool isUseGUI = true;

    private void Start()
    {
        //find component
        GameManagerInitiate();

        m_maze.Initiate();
        m_route_collector.Initiate();

        m_landmark.InitLandmark(m_maze.building_interval, m_maze.building_width);
        m_landmark.ChangeLandmarkNumber(0);

        add_buttonListener();
    }



    void GameManagerInitiate()
    {
        m_maze = GetComponent<MazeGenerator>();
        m_route_collector = GetComponent<RouteGenerator>();
        m_recorder = GetComponent<PlayerDataRecorder>();
        m_UIManager = GetComponent<UIManager>();
        m_landmark = GetComponent<LandmarkManager>();

        //last time :  m_random_mode_generator = new RandomModeGenorator();
        m_random_mode_generator.generate();

        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Mover>();
            player.turnSpeed = turnSpeed;
            player.moveSpeed = moveSpeed;


        }
        speed_slider.maxValue = 20;
        speed_slider.minValue = 1;
        speed_slider.value = moveSpeed;

        rotate_speed_slider.maxValue = 10;
        rotate_speed_slider.minValue = 0;
        rotate_speed_slider.value = turnSpeed;

        foreach(var text in version_title_text)
        {
            text.text = "ver. " + version;
        }
        


    }

    private void Update()
    {
        ListenPlayerState();

        if (player != null)
        {
            player.turnSpeed = turnSpeed;
            player.moveSpeed = moveSpeed;
        }

        m_maze.DynamicMazeGenerator();
    }
    void add_buttonListener()
    {
        if (goTravel != null)
        {
            goTravel.onClick.AddListener(Generate);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(GoBack);

        }

        if (leftButton != null)
        {
            leftButton.onClick.AddListener(left_direction);
        }

        if (straightButton != null)
        {
            straightButton.onClick.AddListener(straight_direction);
        } 

        if (rightButton != null)
        {
            rightButton.onClick.AddListener(right_direction);
        }

        if (ExportButton != null)
        {
            ExportButton.onClick.AddListener(Export_data_to_Excel);
        }

        if (StartButton != null)
        {
            UnityEngine.Events.UnityAction _event = () => m_UIManager.ChangePage(1);
            StartButton.onClick.AddListener(_event);
        }

        if (exitButton != null)
        {
            UnityEngine.Events.UnityAction _event = () => quit();

            // UnityEngine.Events.UnityAction _event = () => m_UIManager.ChangePage(0);
            exitButton.onClick.AddListener(_event);
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(changeGameMode);
        }

    }
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif  
    }
    public void changeGameMode(int mode)
    {

        int landmark_number = 0;
        switch (mode)
        {
            case 0:
                landmark_number = 0;
                gameMode = GameMode.Constant;
                break;
            case 1:
                landmark_number = 1;
                gameMode = GameMode.Landmark_2sides;
                break;
            case 2:
                landmark_number = 4;
                gameMode = GameMode.Landmark_8sides;
                break;
            case 3:
                gameMode = GameMode.Manual;
                break;
        }

        m_landmark.ChangeLandmarkNumber(landmark_number);


        //reset route data
        m_route_collector.total_length = 14;
        m_route_collector.total_rotate_time = 6;
    }
    void Export_data_to_Excel()
    {
        m_recorder.ExportPlayerData();
    }
    bool isPlayerOnJourney()
    {
        return player.isOnJourney == JourneyStage.OnJourney ||
                       player.isOnJourney == JourneyStage.OnJourney_goStraight ||
                       player.isOnJourney == JourneyStage.OnJourney_onPause ||
                       player.isOnJourney == JourneyStage.OnJourney_turnAround;
    }


    public void EndGame()
    {

        m_UIManager.Manage_Pages[2].gameObject.SetActive(true);
        m_UIManager.ChangePage(2);

        UnityAction quitGame = () => quit();
        onTimerCallOut(2.9f, quitGame);
    }
    public void onTimerCallOut(float time, UnityAction _action)
    {

        StartCoroutine(timer(time, _action));
    }

    IEnumerator timer(float time, UnityAction callout)
    {
        yield return new WaitForSeconds(time);
        callout.Invoke();
    }


    void ListenPlayerState()
    {
        bool isGo = player.journeyType == JourneyType.go;
        bool isPause = player.isOnJourney == JourneyStage.OnJourney_onPause;
        bool isRouteDataNotEmpty = m_recorder.playerData.results.Count > 0;

        isChangeValueManually = (gameMode == GameMode.Manual);
        isUseLandmark = (gameMode == GameMode.Landmark_2sides || gameMode == GameMode.Landmark_8sides);

        ExportButton.interactable = !isPlayerOnJourney() && isGo && isRouteDataNotEmpty;
        dropdown.interactable = !isPlayerOnJourney() && isGo;

        goTravel.interactable = isGo && !isPlayerOnJourney();
        backButton.interactable = !isGo && !isPlayerOnJourney();

        bool playerDirectionButton = !isGo && isPause && isPlayerOnJourney();
        leftButton.transform.parent.gameObject.SetActive(playerDirectionButton);
        m_UIManager.Manage_Pages[1].EnableSingleLayer(isChangeValueManually, 0);

        m_landmark.isActiveLandmark(isUseLandmark && isPlayerOnJourney());

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

    ////player control event - choose right direciton 
    void straight_direction()
    {
        PlayerChooseDirection(2);
    }

    //record player current position & choise made by player 
    void PlayerChooseDirection(int dir)
    {
        
        //record player choise
        Vector3 next_direction = player.get_nextDirection;
        Vector3 previous_direction = player.get_previousDirection;
        int current_index = player.get_reverse_previousPositionIndex;

        //match answer
        Vector3 player_choise_direction = Vector3.Cross(previous_direction, Vector3.down * dir);
        // float angle_between_choise_direction_and_next_direction = Vector3.Angle(next_direction,player_choise_direction);

        if(dir == 2 )player_choise_direction = previous_direction;
        int isAnswerCorrect = (Vector3.Angle(next_direction, player_choise_direction) < 5) ? 1 : 0;
        
        m_recorder.RecordPlayerChoise(current_index, isAnswerCorrect, dir);
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

    void read_next_random_mode()
    {
        int mode = m_random_mode_generator.mode_index[random_mode_index];
        gameMode = intToGameMode(mode);
        changeGameMode(mode);
        ListenPlayerState();

        random_mode_index++;

        if (random_mode_index > 1) Export_data_to_Excel();
        if (random_mode_index > m_random_mode_generator.total_run - 1)
        {
            Debug.Log("End game");
            m_UIManager.Manage_Pages[2].gameObject.SetActive(true);
            m_UIManager.ChangePage(2);
            EndGame();
        }
    }

    GameMode intToGameMode(int index)
    {

        GameMode _gameMode_ = GameMode.Constant;
        switch (index)
        {
            case 0:
                _gameMode_ = GameMode.Constant;
                break;
            case 1:
                _gameMode_ = GameMode.Landmark_2sides;
                break;
            case 2:
                _gameMode_ = GameMode.Landmark_8sides;
                break;
            default:
                _gameMode_ = GameMode.Manual;
                break;
        }
        return _gameMode_;
    }


    void Generate()
    {
        //if use random mode 
        if (isUseRanodom_landmark_constant_mode) read_next_random_mode();


        //version 1.03 : if (random_mode_index > m_random_mode_generator.total_run - 1)
        if (random_mode_index > m_random_mode_generator.total_run)
        {
            Debug.Log("End game");


            EndGame();
            return;
        }

        m_route_collector.Generate(isChangeValueManually, isUseLandmark);
        Route current_route = m_route_collector.get_CurrentRoute;

        //landmark
        record_mode_usage_count();
        GenerateLandmark(current_route);

        //player data & mover 
        player.startJourney(current_route, JourneyType.go);

        m_recorder.RecordRouteData(current_route);
        if (isUseLandmark)
        {
            int number_of_landmark = 0;

            if (gameMode == GameMode.Landmark_2sides)
            {
                number_of_landmark = 1;
            }
            else
            {
                number_of_landmark = 4;
            }
            m_recorder.RecordLandmarkType(number_of_landmark);

        }


    }

    //[need to optimize] 
    void record_mode_usage_count()
    {
        switch (gameMode)
        {
            case GameMode.Constant:
                get_current_constant_index += 1;
                break;
            case GameMode.Landmark_2sides:
                m_landmark.ChangeLandmarkPic(get_current_Landmark_2sides_index);
                get_current_Landmark_2sides_index += 1;

                break;
            case GameMode.Landmark_8sides:
                m_landmark.ChangeLandmarkPic(get_current_Landmark_8sides_index);
                get_current_Landmark_8sides_index += 1;

                break;
            default:
                break;
        }
    }

    void GenerateLandmark(Route _CurrentRoute)
    {
        if (isUseLandmark && !isChangeValueManually)
        {
            Vector3 landmark_position = _CurrentRoute.get_landmark_position;
            m_landmark.Landmark_offset.transform.position = landmark_position;
        }
    }


    private void OnGUI()
    {
        if (isUseGUI)
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
        else
        {
            Length_slider.maxValue = 20;
            Length_slider.minValue = 3;
            if (isChangeValueManually) m_route_collector.total_length = (int)Length_slider.value;
            Length_text.text = (int)Length_slider.value + "";


            Rotate_slider.maxValue = m_route_collector.total_length - 1;
            Rotate_slider.minValue = 0;
            if (isChangeValueManually) m_route_collector.total_rotate_time = (int)Rotate_slider.value;
            Rotate_text.text = (int)Rotate_slider.value + "";


            moveSpeed = speed_slider.value;
            speed_text.text = speed_slider.value + "";


            turnSpeed = rotate_speed_slider.value;
            rotate_speed_text.text = rotate_speed_slider.value + "";

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.shift && e.keyCode == KeyCode.Space)
            {
                // shift + space
                bool isLayerActive = m_UIManager.Manage_Pages[1].Layer2[1].activeSelf;
                m_UIManager.Manage_Pages[1].EnableSingleLayer(!isLayerActive, 1);
            }

        }
    }


}
[System.Serializable]
public class RandomModeGenorator
{
   
    public int total_run = 15;
    public int constant_mode_ratio = 1;
    public int landmarkv1_mode_ratio = 1;
    public int landmarkv2_mode_ratio = 1;

    public int[] mode_index;

    public RandomModeGenorator()
    {
        generate();
    }
    public void generate()
    {
        mode_index = generate_ramdom_mode_index(total_run, constant_mode_ratio, landmarkv1_mode_ratio, landmarkv2_mode_ratio);
    }

    int[] generate_ramdom_mode_index(int total, int constant_radio, int landmarkv1_ratio, int landmarkv2_ration)
    {

        int[] new_mode_index = new int[total];
        int total_radio = constant_radio + landmarkv1_ratio + landmarkv2_ration;

        int[] mode_number = {
            constant_radio*total/total_radio,
            landmarkv1_ratio*total/total_radio,
            landmarkv2_ration*total/total_radio};


        for (int i = 0; i < total; i++)
        {
            float l = mode_number.Length;
            System.Random randomSeed = new System.Random();
            int rnd = (int)Mathf.Clamp(randomSeed.Next(0,3), 0, l - 1);

            //check mode number 
            while (mode_number[rnd] == 0)
            {
                rnd += 1;
                if (rnd >= mode_number.Length) rnd = 0;
                if (mode_number[rnd] == 0) continue;

                bool complete = true;
                for (int j = 0; j < mode_number.Length; j++)
                {
                    complete = complete && (mode_number[rnd] == 0);
                }
                if (complete) break;
            }
            // Debug.Log("RND" + rnd);
            mode_number[rnd] -= 1;
            new_mode_index[i] = rnd;

        }

        return new_mode_index;
    }


}