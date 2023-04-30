using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public GameStateBehaviourScript gameState;
    
    // Start is called before the first frame update
    void Start()
    {
        gameState = FindObjectOfType<GameStateBehaviourScript>();
    }

    public void RunLevel(int level)
    {
        if (level == 0)
        {
            gameState.ChangeToLevelNone();
        }
        else if (level == 1)
        {
            gameState.ChangeToLevelOne();
        }
        else if (level == 2)
        {
            gameState.ChangeToLevelTwo();
        }
        else if (level == 3)
        {
            gameState.ChangeToLevelThree();
        }
        else if (level == 4)
        {
            gameState.ChangeToLeveFour();
        } 
        else if (level == 5)
        {
            gameState.ChangeToLevelDone();
        } 
    }
}
