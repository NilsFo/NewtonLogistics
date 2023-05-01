using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RadioManager : MonoBehaviour
{
    public Transform radioPos;
    public GameState gameState;
    
    private TextBubbleManager textBubbleManager;

    private float busyTimer = 0;

    private String[] bossQuotes = {
        "Stop slackin', start stackin'!",
        "All breaks for this shift are cancelled.",
        "Is anyone even doin' any work in here?",
        "If you want it done right, do it yourself!",
        "No work ethos with these kids today...",
        "Remember to report any union talks to HR for a bonus!",
        "C'mon, put your back into it!",
        "Those trucks ain't gonna load themselves!",
        "Double time it!",
        "Smoke breaks are deducted from your pay.",
        "You break it, you pay for it!",
        "Back in the day, we'd have loaded that truck in half the time!",
        "Pull those bootstraps, kid!",
        "Do I hear someone takin' a break?",
        "Don't even think about leaving early today. We got a big order coming up.",
        "This delivery is for a big client. Take extra effort, you hear?",
        "Failure to meet company standards will result in disciplinary actions!",
        "I want everyone on their best behaviour when the inspector is here!",
        "Come on, start earning some money!",
        "I can't talk right now, I have an important meeting!",
        "Quitters don't earn!",
        "What's even going on down there?",
        "I've built this place from the ground up you know?",
        "HR wants me to read this: You are a valuable member of this team and we are very proud of you.",
        "When I was your age, I already had a house and a car.",
        "Announcement: We are now owned by International Financing Ltd.",
        "Announcement: We are now owned by the North Bay Fishery Inc.",
        "Announcement: We are now owned by Southeast Airlines.",
        "Look at those incompetent idiots... wait, is this thing on?",
        "Dental care is cancelled for the month.",
        "Get stacking kid! I don't pay you for standing around!"
    };
    
    // Start is called before the first frame update
    void Start() {
        busyTimer = 0.9f;
        textBubbleManager = GetComponent<TextBubbleManager>();
        //Invoke("DialogueLine1", 1);
        //Invoke("DialogueLine2", 3+5);
        //Invoke("DialogueLine3", 3+5+7);
    }

    public void RadioMessage(string message, float duration) {
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(transform, message, duration);
    }
    
    // Update is called once per frame
    void Update()
    {
        busyTimer = busyTimer - Time.deltaTime;

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
            busyTimer = time;
            RadioMessage(text, time);
        }
    }

    public void DialogueLine1() {
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(radioPos, "Get stacking kid! I don't pay you for standing around!", 5);
        busyTimer = 8;
    }

    public void ReadRandomQuote() {
        if (busyTimer > 0)
            return;
        var i = Random.Range(0, bossQuotes.Length);
        var quote = bossQuotes [i];
        textBubbleManager.ClearDialogueBoxes();
        textBubbleManager.Say(radioPos, quote);
    }
    
}
