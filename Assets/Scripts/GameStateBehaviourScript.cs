using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Init,
    Intro,
    Pause,
    Start,
    Finish
}

public enum GameLevel
{
    None,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Done
}

public class GameStateBehaviourScript : MonoBehaviour
{
    public static int LoadLevel = -1;
    
    //UnityEvents
    public UnityEvent<GameLevel,GameState> onGameStateChange;
    public UnityEvent<GameObject[],GameObject[]> onCurrentLevelListChange;
    
    [Header("Level")]
    [SerializeField] private GameObject[] listContainerForLevelOne;
    [SerializeField] private GameObject[] listStationForLevelOne;
    
    [SerializeField] private GameObject[] listContainerForLevelTwo;
    [SerializeField] private GameObject[] listStationForLevelTwo;
    
    [SerializeField] private GameObject[] listContainerForLevelThree;
    [SerializeField] private GameObject[] listStationForLevelThree;
    
    [SerializeField] private GameObject[] listContainerForLevelFour;
    [SerializeField] private GameObject[] listStationForLevelFour;
    
    [SerializeField] private GameObject[] listContainerForLevelFive;
    [SerializeField] private GameObject[] listStationForLevelFive;
    
    [SerializeField] private GameObject[] listContainerForLevelSix;
    [SerializeField] private GameObject[] listStationForLevelSix;
    
    //Stats
    [Header("Stats")]
    [SerializeField] private GameState gameState = GameState.Init;
    [SerializeField] private GameLevel gameLevel = GameLevel.None;

    // misc
    public ShipController player;
    public MainCameraController cameraController;
    public MusicManager musicManager;
    
    [Header("Debug")]
    [SerializeField] private DumpableBehaviourScript[] listOfDump;
    [SerializeField] private DumpStationBehaviourScript[] listOfDumpStations;

    [SerializeField] private GameObject[] currentLevelListOfContainer;
    [SerializeField] private GameObject[] currentLevelListOfStations;
    [SerializeField] private Dictionary<string, int> points;

    private int playerRequestsQuitCounter = 0;
    private bool playerRequestsQuitWasReleased = true;
    
    private void Awake()
    {
        player = FindObjectOfType<ShipController>();
        cameraController = FindObjectOfType<MainCameraController>();
        musicManager = FindObjectOfType<MusicManager>();
        
        if (onGameStateChange == null)
            onGameStateChange = new UnityEvent<GameLevel, GameState>();
        if (onCurrentLevelListChange == null)
            onCurrentLevelListChange = new UnityEvent<GameObject[], GameObject[]>();

        points = new Dictionary<string, int>();
    }

    void Start()
    {
        musicManager.PlaySongLoop();
        GameLevel levelToLoad = gameLevel;
        GameState stateToLoad = gameState;

        if (LoadLevel != -1)
        {
            gameLevel = GameLevel.None;
            gameState = GameState.Init;
        }
        
        if (LoadLevel == 0)
        {
            levelToLoad = GameLevel.None;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 1)
        {
            levelToLoad = GameLevel.One;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 2)
        {
            levelToLoad = GameLevel.Two;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 3)
        {
            levelToLoad = GameLevel.Three;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 4)
        {
            levelToLoad = GameLevel.Four;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 5)
        {
            levelToLoad = GameLevel.Five;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 6)
        {
            levelToLoad = GameLevel.Six;
            stateToLoad = GameState.Init;
        }
        else if (LoadLevel == 7)
        {
            levelToLoad = GameLevel.Done;
            stateToLoad = GameState.Init;
        }
        
        ChangeGameLevelAndGameState(levelToLoad, stateToLoad);
        LoadLevel = -1;
    }

    private void Update()
    {
        if (gameState == GameState.Init)
        {
            ChangeGameState(GameState.Intro);
        }
        else if (gameState == GameState.Finish && gameLevel != GameLevel.Done)
        {
            ChangeGameLevelAndGameState(NextLevel(),GameState.Init);
        }
        else if (gameState == GameState.Start)
        {
            CheckPointCompleteness();
        }
        
        bool playerRequestsQuit = Keyboard.current.escapeKey.isPressed;
        if (playerRequestsQuit && playerRequestsQuitWasReleased) 
        {
            playerRequestsQuitCounter++;
            playerRequestsQuitWasReleased = false;
            
            if (playerRequestsQuitCounter > 1)
            {
                //Second Press
                LoadLevel = -1;
                SceneManager.LoadScene("MainMenuScene");
            }
        }
        else
        {
            playerRequestsQuitWasReleased = true;
        }


        if (playerRequestsQuit)
        {
            LoadLevel = -1;
            SceneManager.LoadScene("MainMenuScene");
        }

        bool playerRequestsReset = Keyboard.current.backspaceKey.isPressed;
        if (playerRequestsReset)
        {
            LoadLevel = -1;
            if(gameLevel == GameLevel.One) LoadLevel = 1;
            if(gameLevel == GameLevel.Two) LoadLevel = 2;
            if(gameLevel == GameLevel.Three) LoadLevel = 3;
            if(gameLevel == GameLevel.Four) LoadLevel = 4;
            if(gameLevel == GameLevel.Five) LoadLevel = 5;
            if(gameLevel == GameLevel.Six) LoadLevel = 6;
            if(gameLevel == GameLevel.Done) LoadLevel = 7;
            SceneManager.LoadScene("KairosStation");
        }
    }

    private void CheckPointCompleteness()
    {
        int sumOfContainerInStation = 0;
        points = new Dictionary<string, int>();
        for (int i = 0; i < currentLevelListOfStations.Length; i++)
        {
            GameObject obj = currentLevelListOfStations[i];
            DumpStationBehaviourScript station = obj.GetComponentInChildren<DumpStationBehaviourScript>();
            if (station != null)
            {
                points.Add(station.GetStationName(), station.GetContainerCount());
                sumOfContainerInStation += station.GetContainerCount();

            }
        }
            
        bool isComplet = (sumOfContainerInStation == currentLevelListOfContainer.Length);
        if (isComplet && gameState == GameState.Start)
        {
            ChangeGameState(GameState.Finish);
        }
    }

    private GameState NextState()
    {
        if (gameState == GameState.Init)
        {
            return GameState.Intro;
        }
        
        if (gameState == GameState.Intro)
        {
            return GameState.Start;
        }
        
        if (gameState == GameState.Start)
        {
            return GameState.Finish;
        }

        return gameState;
    }

    private GameLevel NextLevel()
    {
        if (gameLevel == GameLevel.None)
        {
            return GameLevel.One;
        }
        
        if (gameLevel == GameLevel.One)
        {
            return GameLevel.Two;
        }
        
        if (gameLevel == GameLevel.Two)
        {
            return GameLevel.Three;
        }
        
        if (gameLevel == GameLevel.Three)
        {
            return GameLevel.Four;
        }
        
        if (gameLevel == GameLevel.Four)
        {
            return GameLevel.Five;
        }
        
        if (gameLevel == GameLevel.Five)
        {
            return GameLevel.Six;
        }
        
        if (gameLevel == GameLevel.Six)
        {
            return GameLevel.Done;
        }

        return GameLevel.Done;
    }

    public void ChangeGameLevel(GameLevel level)
    {
        ChangeGameLevelAndGameState(level, GameState.Init);
    }
    
    public void ChangeGameState(GameState state)
    {
        ChangeGameLevelAndGameState(gameLevel, state);
    }

    public void ChangeGameLevelAndGameState(GameLevel level, GameState state)
    {
        if (level != gameLevel)
        {
            DisableCurrentLevel();
            EnableLevel(level);
        }
        
        gameLevel = level;
        gameState = state;
        //Fire Event 
        onGameStateChange.Invoke(gameLevel, gameState);
        onCurrentLevelListChange.Invoke(currentLevelListOfStations, currentLevelListOfContainer);
    }

    public void EnableLevel(GameLevel level)
    {
        GameObject[] listStation = new GameObject[0];
        GameObject[] listContainer = new GameObject[0];

        Dictionary<int, int> counter = new Dictionary<int, int>();
        
        if (level == GameLevel.One)
        {
            listStation = listStationForLevelOne;
            listContainer = listContainerForLevelOne;
        }
        else if (level == GameLevel.Two)
        {
            listStation = listStationForLevelTwo;
            listContainer = listContainerForLevelTwo;
        }
        else if (level == GameLevel.Three)
        {
            listStation = listStationForLevelThree;
            listContainer = listContainerForLevelThree;
        }
        else if (level == GameLevel.Four)
        {
            listStation = listStationForLevelFour;
            listContainer = listContainerForLevelFour;
        }
        else if (level == GameLevel.Five)
        {
            listStation = listStationForLevelFive;
            listContainer = listContainerForLevelFive;
        }
        else if (level == GameLevel.Six)
        {
            listStation = listStationForLevelSix;
            listContainer = listContainerForLevelSix;
        }
        currentLevelListOfContainer = listContainer;
        for (int i = 0; i < listContainer.Length; i++)
        {
            GameObject objToEnable = listContainer[i];
            DumpableBehaviourScript dump = objToEnable.GetComponent<DumpableBehaviourScript>();
            if (dump != null)
            {
                if(!counter.ContainsKey(dump.DumpStationIndex)) counter.Add(dump.DumpStationIndex, 0);
                counter[dump.DumpStationIndex] = counter[dump.DumpStationIndex] + 1;
            }
            objToEnable.SetActive(true);
        }

        //Set Max Count
        currentLevelListOfStations = listStation;
        for (int i = 0; i < listStation.Length; i++)
        {
            GameObject objToEnable = listStation[i];
            DumpStationBehaviourScript station = objToEnable.GetComponentInChildren<DumpStationBehaviourScript>();
            if (station != null)
            {
                if (counter.ContainsKey(station.StationIndex))
                {
                    station.SetMaxCountContainer(counter[station.StationIndex]);
                }
            }
        }
    }
    
    public void DisableCurrentLevel()
    {
        for (int i = 0; i < currentLevelListOfContainer.Length; i++)
        {
            GameObject objToDisable = currentLevelListOfContainer[i];
            //Trigger Script
            objToDisable.SetActive(false);
        }
        
        currentLevelListOfContainer = new GameObject[0];
        currentLevelListOfStations = new GameObject[0];
    }

    #region EventFunctions
    
    public void ChangeToLevelNone()
    {
        ChangeGameLevelAndGameState(GameLevel.None, GameState.Init);
    }
    
    public void ChangeToLevelDone()
    {
        ChangeGameLevelAndGameState(GameLevel.Done, GameState.Init);
    }
    
    public void ChangeToLevelOne()
    {
        ChangeGameLevelAndGameState(GameLevel.One, GameState.Init);
    }
    
    public void ChangeToLevelTwo()
    {
        ChangeGameLevelAndGameState(GameLevel.Two, GameState.Init);
    }
    
    public void ChangeToLevelThree()
    {
        ChangeGameLevelAndGameState(GameLevel.Three, GameState.Init);
    }
    
    public void ChangeToLeveFour()
    {
        ChangeGameLevelAndGameState(GameLevel.Four, GameState.Init);
    }
    
    #endregion

    public void ChangeToLevelFour()
    {
        ChangeGameLevelAndGameState(GameLevel.Five, GameState.Init);
    }

    public void ChangeToLevelSix()
    {
        ChangeGameLevelAndGameState(GameLevel.Six, GameState.Init);
    }
}
