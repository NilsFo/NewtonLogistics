using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Dialogue;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionIntroBehaviourScript : MonoBehaviour
{

    [SerializeField] private GameStateBehaviourScript gameState;

    [SerializeField] private DialogueManager dialogueManager;

    [SerializeField] private float msgTimer = 6f;
    [SerializeField] private float msgOffset = 2f;
    
    [SerializeField] private float introTime = 3f;
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool inPlaying = false;

    private Stack<Tuple<float, Action>> phaseStack;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

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
            if(level == GameLevel.One)
            {
                PlayIntroLevelOne();
            }
            else if(level == GameLevel.Two)
            {
                PlayIntroLevelTwo();
            }
            else if(level == GameLevel.Three)
            {
                PlayIntroLevelThree();
            }
            else if(level == GameLevel.Four)
            {
                PlayIntroLevelFour();
            }
            else if(level == GameLevel.Five)
            {
                PlayIntroLevelFive();
            }
            else if(level == GameLevel.Six)
            {
                PlayIntroLevelSix();
            }
            else if(level == GameLevel.Done)
            {
                PlayIntroLevelFin();
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
        currentTime = 0f;
        introTime = msgTimer * 4 + msgOffset * 3;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 3 + msgOffset * 3, IntroLevelOnePhaseFour));
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 2 + msgOffset * 2, IntroLevelOnePhaseThree));
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelOnePhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelOnePhaseOne));
    }

    private void PlayIntroLevelTwo()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 4 + msgOffset * 3;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 3 + msgOffset * 3, IntroLevelTwoPhaseFour));
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 2 + msgOffset * 2, IntroLevelTwoPhaseThree));
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelTwoPhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelTwoPhaseOne));
    }
    
    private void PlayIntroLevelThree()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 1 + msgOffset * 0;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelThreePhaseOne));
    }

    private void PlayIntroLevelFour()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 2 + msgOffset * 1;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelFourPhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelFourPhaseOne));
    }
    
    private void PlayIntroLevelFive()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 2 + msgOffset * 1;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelFivePhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelFivePhaseOne));
    }
    
    private void PlayIntroLevelSix()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 2 + msgOffset * 1;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelSixPhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelSixPhaseOne));
    }
    
    private void PlayIntroLevelFin()
    {
        inPlaying = true;
        currentTime = 0f;
        introTime = msgTimer * 2 + msgOffset * 1;
        phaseStack = new Stack<Tuple<float, Action>>();
        phaseStack.Push(new Tuple<float, Action>(msgTimer * 1 + msgOffset * 1, IntroLevelFinPhaseTwo));
        phaseStack.Push(new Tuple<float, Action>(0f, IntroLevelFinPhaseOne));
    }
    
    #region IntroLevelOne
    
    void IntroLevelOnePhaseOne()
    {
        dialogueManager.RequestRadioMessage("Hey new guy, welcome to Newton Logistics. We have a space station to build, so don't wait around.", msgTimer);
    }
    void IntroLevelOnePhaseTwo()
    {
        dialogueManager.RequestRadioMessage("Move your ship around with WASD. Get to the cargo hall and I'll fill you in.", msgTimer);
    }
    void IntroLevelOnePhaseThree()
    {
        dialogueManager.RequestRadioMessage("The pros also use Q and E to move the ship sideways.", msgTimer);
    }
    void IntroLevelOnePhaseFour()
    {
        dialogueManager.RequestRadioMessage("Great. Now grab these containers with your magnet. Hold F to pull them all the way in and release them with R.", msgTimer*5);
    }

    #endregion

    #region IntroLevelTwo
    
    void IntroLevelTwoPhaseOne()
    {
        dialogueManager.RequestRadioMessage("Good job on those containers. They are pretty sturdy, so don't be afraid to eject them with force using SPACEBAR.", msgTimer);
    }
    void IntroLevelTwoPhaseTwo()
    {
        dialogueManager.RequestRadioMessage("We have a few more flying around outside. Grab them and bring them back to Cargo Bay 3.", msgTimer);
    }
    void IntroLevelTwoPhaseThree()
    {
        dialogueManager.RequestRadioMessage("You can use the TAB key to get a better overview. Cargo and cargo bays are also marked on your radar.", msgTimer);
    }
    void IntroLevelTwoPhaseFour()
    {
        dialogueManager.RequestRadioMessage("If you haven't already figured it out, you can connect cargo together. Keep holding F until they are connected.", msgTimer);
    }

    #endregion
    
    #region IntroLevelThree
    
    void IntroLevelThreePhaseOne()
    {
        dialogueManager.RequestRadioMessage("We have a hull breach in the western corridor. Grab what's left and bring it back.", msgTimer);
    }
    

    #endregion
    
    #region IntroLevelFour
    
    void IntroLevelFourPhaseOne()
    {
        dialogueManager.RequestRadioMessage("New delivery from the train station. Get your ship over to the east and bring the lot back.", msgTimer);
    }
    void IntroLevelFourPhaseTwo()
    {
        dialogueManager.RequestRadioMessage("Remember that heavy cargo displaces your center of mass. Use Q and E for better control.", msgTimer);
    }

    #endregion
    
    #region IntroLevelFive
    
    void IntroLevelFivePhaseOne()
    {
        dialogueManager.RequestRadioMessage("You should apply to interstellar shipping with these moves!", msgTimer);
    }
    void IntroLevelFivePhaseTwo()
    {
        dialogueManager.RequestRadioMessage("We need some cargo from the northern warehouse. Let's see you work in tight corners.", msgTimer);
    }

    #endregion
    
    #region IntroLevelSix
    
    void IntroLevelSixPhaseOne()
    {
        dialogueManager.RequestRadioMessage("Time flies when you're hard at work. This station is almost complete!", msgTimer);
    }
    void IntroLevelSixPhaseTwo()
    {
        dialogueManager.RequestRadioMessage("Only thing left is the sign up front. Grab the letters and don't misspell. The station is called KAIROS.", msgTimer);
    }

    #endregion
    
    #region IntroLevelFin
    
    void IntroLevelFinPhaseOne()
    {
        dialogueManager.RequestRadioMessage("That's it! KAIROS STATION is now legally completed. Good job new guy!", msgTimer);
    }
    void IntroLevelFinPhaseTwo()
    {
        GameStateBehaviourScript.LoadLevel = -1;
        SceneManager.LoadScene("EndingScene");
    }

    #endregion
}
