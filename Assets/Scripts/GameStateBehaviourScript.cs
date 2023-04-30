using System;
using System.Collections.Generic;
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

public class GameStateBehaviourScript : MonoBehaviour
{
    //UnityEvents
    public UnityEvent onInit;
    public UnityEvent onIntro;
    public UnityEvent onPause;
    public UnityEvent onStart;
    public UnityEvent onFinish;
    
    public UnityEvent<int,Vector2Int> onStatsChange;
    
    public UnityEvent<DumpableBehaviourScript[]> onDumpListChange;
    public UnityEvent<DumpStationBehaviourScript[]> onDumpStationChange;
    
    //Stats
    [Header("Stats")]
    [SerializeField] private GameState gameState = GameState.Init;
    [SerializeField] private Vector2Int[] currentStatsArray;
    
    // misc
    public ShipController player;
    
    //privates
    private DumpableBehaviourScript[] listOfDump;
    private DumpStationBehaviourScript[] listOfDumpStations;

    private void Awake()
    {
        player = FindObjectOfType<ShipController>();

        if (onInit == null)
            onInit = new UnityEvent();
        if (onIntro == null)
            onIntro = new UnityEvent();
        if (onPause == null)
            onPause = new UnityEvent();
        if (onStart == null)
            onStart = new UnityEvent();
        if (onFinish == null)
            onFinish = new UnityEvent();
        
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
        ChangeToStart();
    }

    public GameState CurrentGameState => gameState;

    public int GetCurrentPoints(int index)
    {
        return currentStatsArray[index][0];
    }
    
    public int GetMaxPoints(int index)
    {
        return currentStatsArray[index][1];
    }
    
    public Vector2Int GetPointsForIndex(int index)
    {
        return currentStatsArray[index];
    }

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

        if (isComplet)
        {
            this.ChangeToFinish();
        }
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
        
        currentStatsArray = new Vector2Int[dict.Keys.Count];
        for (int i = 0; i < dict.Keys.Count; i++) 
        {
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
    public void ChangeToInit()
    {
        gameState = GameState.Init;
        onInit.Invoke();
    }
    
    public void ChangeToIntro()
    {
        gameState = GameState.Intro;
        onInit.Invoke();
    }
    
    public void ChangeToPause()
    {
        gameState = GameState.Pause;
        onPause.Invoke();
    }
    
    public void ChangeToStart()
    {
        gameState = GameState.Start;
        onStart.Invoke();
    }
    
    public void ChangeToFinish()
    {
        gameState = GameState.Finish;
        onFinish.Invoke();
    }
    
    #endregion 
}
