using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public UnityEvent<int,Vector2Int> onStatsChange;
    public UnityEvent<DumpableBehaviourScript[]> onDumpListChange;
    public UnityEvent<DumpStationBehaviourScript[]> onDumpStationChange;
    
    //Stats
    [Header("Stats")]
    [SerializeField] private GameState gameState = GameState.Init;
    [SerializeField] private GameLevel gameLevel = GameLevel.None;
    
    [SerializeField] private Vector2Int[] currentStatsArray;
    
    // misc
    public ShipController player;
    public MainCameraController cameraController;
    
    //privates
    private DumpableBehaviourScript[] listOfDump;
    private DumpStationBehaviourScript[] listOfDumpStations;

    private void Awake()
    {
        player = FindObjectOfType<ShipController>();
        cameraController = FindObjectOfType<MainCameraController>();
        
        if (onGameStateChange == null)
            onGameStateChange = new UnityEvent<GameLevel, GameState>();

        if (onStatsChange == null)
            onStatsChange = new UnityEvent<int, Vector2Int>();
        if (onDumpListChange == null)
            onDumpListChange = new UnityEvent<DumpableBehaviourScript[]>();
        if (onDumpStationChange == null)
            onDumpStationChange = new UnityEvent<DumpStationBehaviourScript[]>();
    }

    void Start()
    {
        FindAllDumpStations();
        FindAllDumpable();
        ChangeGameLevelAndGameState(GameLevel.One, GameState.Init);
    }

    public GameState CurrentGameState => gameState;
    public GameLevel CurrentGameLevel => gameLevel;

    public Vector2Int[] GetStatsArray()
    {
        return currentStatsArray;
    }

    public bool SetPoints(int index, int points)
    {
        int currentPoints = points;
        if (currentPoints >= currentStatsArray[index][1])
        {
            //Max Points reached! try to add more Points than possible!
            currentPoints = currentStatsArray[index][1];
        }
        if (currentPoints <= 0)
        {
            //Min Points reached! try to remove more Points than possible!
            currentPoints = 0;
        }
        currentStatsArray[index][0] = currentPoints;
        onStatsChange.Invoke(index, currentStatsArray[index]);
        CheckPointCompleteness();
        return true;
    }

    private void CheckPointCompleteness()
    {
        bool isComplet = true;
        for (int i = 0; i < currentStatsArray.Length; i++)
        {
            Vector2Int stats = currentStatsArray[i];
            if (stats[0] != stats[1])
            {
                isComplet = false;
                break;
            }
        }

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
        if (gameLevel != GameLevel.None)
        {
            return GameLevel.One;
        }
        
        if (gameLevel != GameLevel.One)
        {
            return GameLevel.Two;
        }
        
        if (gameLevel != GameLevel.Two)
        {
            return GameLevel.Three;
        }
        
        if (gameLevel != GameLevel.Three)
        {
            return GameLevel.Four;
        }
        
        if (gameLevel != GameLevel.Four)
        {
            return GameLevel.Done;
        }

        return gameLevel;
    }
    
    private void FindAllDumpable()
    {
        listOfDump = FindObjectsByType<DumpableBehaviourScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Dictionary<int, int> dict = new Dictionary<int, int>();
            
        for (int i = 0; i < listOfDump.Length; i++)
        {
            DumpableBehaviourScript dumb = listOfDump[i];
            if (!dict.ContainsKey(dumb.DumpStationIndex))
            {
                dict[dumb.DumpStationIndex] = 0;
            }
            dict[dumb.DumpStationIndex] = dict[dumb.DumpStationIndex] + 1;
        }
        
        currentStatsArray = new Vector2Int[dict.Keys.Max()+1];
        foreach (var i in dict.Keys) {
            currentStatsArray[i] = new Vector2Int(0,  dict[i]);
        }
        onDumpListChange.Invoke(listOfDump);
        
        //onStatsChange after onDumpListChange for Stations UI
        for (int i = 0; i < dict.Keys.Count; i++) 
        {
            onStatsChange.Invoke(i, currentStatsArray[i]);
        }
    }

    public void FindAllDumpStations()
    {
        listOfDumpStations = FindObjectsByType<DumpStationBehaviourScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        onDumpStationChange.Invoke(listOfDumpStations);
    }
    
    #region EventFunctions

    public void ChangeGameLevel(GameLevel level)
    {
        gameLevel = level;
        gameState = GameState.Init;
        onGameStateChange.Invoke(gameLevel, gameState);
    }
    
    public void ChangeGameState(GameState state)
    {
        gameState = state;
        onGameStateChange.Invoke(gameLevel, gameState);
    }

    public void ChangeGameLevelAndGameState(GameLevel level, GameState state)
    {
        gameLevel = level;
        gameState = state;
        onGameStateChange.Invoke(gameLevel, gameState);
    }

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
