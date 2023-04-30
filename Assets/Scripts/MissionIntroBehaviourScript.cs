using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionIntroBehaviourScript : MonoBehaviour
{

    [SerializeField] private GameStateBehaviourScript gameState;

    [SerializeField] private float introTime = 3f;
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool inPlaying = false;

    private Stack<Tuple<float, Action>> phaseStack;
    
    private void OnEnable()
    {
        gameState.onGameStateChange.AddListener(OnGameStateChange);
    }

    private void OnDisable()
    {
        gameState.onGameStateChange.RemoveListener(OnGameStateChange);
    }

    private void OnGameStateChange(GameLevel level, GameState state)
    {
        if (state == GameState.Intro)
        {
            //Play Intro for level
            if(level == GameLevel.One){
                PlayIntroLevelOne();
            }
            else
            {
                gameState.ChangeGameState(GameState.Start);
                inPlaying = false;
                introTime = 0f;
                currentTime = 0f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlaying)
        {
            if (currentTime < introTime)
            {
                //Trigger Phase form Stack when time reached!
                if (phaseStack.Count > 0)
                {
                    Tuple<float, Action> trup = phaseStack.Peek();
                    if (trup.Item1 < currentTime)
                    {
                        phaseStack.Pop();
                        trup.Item2.Invoke();
                    }
                }
                currentTime += Time.deltaTime;
            }
            else if(currentTime >= introTime)
            {
                gameState.ChangeGameState(GameState.Start);
                inPlaying = false;
            }
        }
    }

    private void PlayIntroLevelOne()
    {
        inPlaying = true;
        introTime = 5f;
        currentTime = 0f;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(4f, IntroLevelOnePhaseFour));
        phaseStack.Push(new Tuple<float, Action>(3f, IntroLevelOnePhaseThree));
        phaseStack.Push(new Tuple<float, Action>(2f, IntroLevelOnePhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(1f, IntroLevelOnePhaseOne));
    }

    #region IntroLevelOne

    void IntroLevelOnePhaseOne()
    {
        Debug.Log("First Line of Intro Bla Bla");
    }
    void IntroLevelOnePhaseTwo()
    {
        Debug.Log("Two Line of Intro Bla Bla");
    }
    void IntroLevelOnePhaseThree()
    {
        Debug.Log("Three Line of Intro Bla Bla");
    }
    void IntroLevelOnePhaseFour()
    {
        Debug.Log("Four Line of Intro Bla Bla");
    }

    #endregion
}
