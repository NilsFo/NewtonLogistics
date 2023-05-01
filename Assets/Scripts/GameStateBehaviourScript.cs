using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    Done
}

public class GameStateBehaviourScript : MonoBehaviour
{
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
        ChangeGameLevelAndGameState(gameLevel, gameState);
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
    }

    private void CheckPointCompleteness()
    {
        int sumOfContainerInStation = 0;
        points = new Dictionary<string, int>();
        for (int i = 0; i < currentLevelListOfStations.Length; i++)
        {
            GameObject obj = currentLevelListOfStations[i];
            DumpStationBehaviourScript station = obj.GetComponent<DumpStationBehaviourScript>();
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
        if (state == GameState.Init)
        {
            EnableLevel(level);
        }

        if (state == GameState.Finish)
        {
            DisableCurrentLevel();
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
            DumpStationBehaviourScript station = objToEnable.GetComponent<DumpStationBehaviourScript>();
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
}
