using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionIntroBehaviourScript : MonoBehaviour
{

    [SerializeField] private GameStateBehaviourScript gameState;

    [SerializeField] private float introTime = 3f;
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool inPlaying = false;
    
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlaying)
        {
            if (currentTime < introTime)
            {
                //TODO Trigger Phase form Stack
                introTime += Time.deltaTime;
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
        introTime = 15f;
        //TODO Phase 1..X based on Time
    }
}
