using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    private DialogueManager _dialogueManager;
    public String message;
    public float duration = 0f;
    // Start is called before the first frame update
    void Start() {
        _dialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ship")) {
            _dialogueManager.RequestRadioMessage(message, duration);
            Destroy(gameObject);
        }
    }
}
