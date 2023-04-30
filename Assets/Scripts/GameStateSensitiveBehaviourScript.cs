using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateSensitiveBehaviourScript : MonoBehaviour
{
    public GameLevel myLevel = GameLevel.One;
    public GameState[] myStates;
    
    public UnityEvent onInit;
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    
    public GameStateBehaviourScript gameState;

    public bool isInit = false;
    public bool isActivate = false;

    private void Awake()
    {
        if (onInit == null)
            onInit = new UnityEvent();
        if (onActivate == null)
            onActivate = new UnityEvent();
        if (onDeactivate == null)
            onDeactivate = new UnityEvent();

        gameState = FindObjectOfType<GameStateBehaviourScript>();
        isActivate = false;
        isInit = false;
    }

    private void OnEnable()
    {
        gameState.onGameStateChange.AddListener(OnGameStateChange);
    }

    private void OnDisable()
    {
        gameState.onGameStateChange.RemoveListener(OnGameStateChange);
    }

    private void Start()
    {
        if (isActivate)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
        
    }

    private void OnGameStateChange(GameLevel leve, GameState state)
    {
        if (leve != myLevel)
        {
            if(isActivate) Deactivate();
            return;
        }

        bool matchState = false;
        for (int i = 0; i < myStates.Length; i++)
        {
            GameState currentState = myStates[i];
            if (currentState == state)
            {
                matchState = true;
            }
        }

        if (!matchState && isActivate)
        {
            Deactivate();
            return;
        }

        if (matchState && !isActivate)
        {
            Activate();
            return;
        }
    }

    private void Init()
    {
        isInit = true;
        onInit.Invoke();
    }
    
    private void Activate()
    {
        if (!isInit) Init();
        isActivate = true;
        onActivate.Invoke();
    }
    
    private void Deactivate()
    {
        isActivate = false;
        onDeactivate.Invoke();
    }
}
