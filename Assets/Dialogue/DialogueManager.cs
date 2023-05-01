using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue {
    public class DialogueManager : MonoBehaviour
    {
        public Transform radioPos;
        public GameStateBehaviourScript gameState;
    
        public TextBubbleManager textBubbleManager;
    
        // Start is called before the first frame update
        void Start()
        {
            textBubbleManager = GetComponent<TextBubbleManager>();
            gameState = FindObjectOfType<GameStateBehaviourScript>();
            
            RequestRadioMessage("Hallo Welt", 5f);
        }

        public void RadioMessage(string message, float duration) {
            textBubbleManager.ClearDialogueBoxes();
            textBubbleManager.Say(radioPos, message, duration);
        }
    
        // Update is called once per frame
        void Update()
        {
        }

        public bool AcceptsAmbientMessages()
        {
            //return gameState.PlayState == GameState.GameplayState.Playing && busyTimer < 0f;
            return true;
        }

        public void RequestRadioMessage(string text, float time)
        {
            if (AcceptsAmbientMessages())
            {
                RadioMessage(text, time);
            }
        }

    }
}
