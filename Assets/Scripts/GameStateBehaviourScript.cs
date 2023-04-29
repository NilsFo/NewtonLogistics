using System;
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
    [Header("Unity Events")]
    [SerializeField] private UnityEvent onInit;
    [SerializeField] private UnityEvent onIntro;
    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onFinish;
    
    [SerializeField] private UnityEvent<int,Vector2Int> onStatsChange;
    
    //Stats
    [Header("Stats")]
    [SerializeField] private GameState gameState = GameState.Init;

    [SerializeField] private int[] levelStats;
    [SerializeField] private Vector2Int[] currentStatsArray;
    
    // misc
    public ShipController player;

    private void Awake()
    {
        player = FindObjectOfType<ShipController>();
    }

    void Start()
    {
        currentStatsArray = new Vector2Int[levelStats.Length];
        
        for (int i = 0; i < levelStats.Length; i++) 
        {
            currentStatsArray[i] = new Vector2Int(0, levelStats[i] );
        }
        
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
        
        ChangeToInit();
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
    
    public bool AddPoints(int index)
    {
        if (currentStatsArray[index][0] >= currentStatsArray[index][1])
        {
            //Max Points reached! try to add more Points than possible!
            Debug.LogError("Trying to add more Points than possible!");
            return false;
        }
        currentStatsArray[index][0]++;
        onStatsChange.Invoke(index, currentStatsArray[index]);
        return true;
    }
    
    public bool RemovePoints(int index)
    {
        if (currentStatsArray[index][0] <= 0)
        {
            //Min Points reached! try to remove more Points than possible!
            Debug.LogError("Trying to remove more Points than possible!");
            return false;
        }
        currentStatsArray[index][0]--;
        onStatsChange.Invoke(index, currentStatsArray[index]);
        return true;
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
