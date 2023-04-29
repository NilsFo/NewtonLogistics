using System;
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
    }

    void Start()
    {
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
        if (currentStatsArray[index][0] >= currentStatsArray[index][1])
        {
            //Max Points reached! try to add more Points than possible!
            currentPoints = currentStatsArray[index][1];
        }
        if (currentStatsArray[index][0] <= 0)
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
