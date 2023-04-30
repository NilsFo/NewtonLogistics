using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ImpactExpression : MonoBehaviour
{
    [Header("Sound")] public bool playSound;
    public AudioClip impactSound;
    public float volumeMult = 1.0f;

    [Header("Particles")] public bool showParticles;
    public Color particleColor;
    public GameObject impactParticlePrefab;

    private GameStateBehaviourScript _gameStateBehaviourScript;

    // Start is called before the first frame update
    void Start()
    {
        _gameStateBehaviourScript = FindObjectOfType<GameStateBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var contact = other.GetContact(0).point;
        Vector3 contactPosition = transform.position;
        contactPosition.x = contact.x;
        contactPosition.y = contact.y;
        
        if (playSound)
        {
            _gameStateBehaviourScript.musicManager.CreateAudioClip(impactSound, contactPosition, volumeMult);
        }
    }
}