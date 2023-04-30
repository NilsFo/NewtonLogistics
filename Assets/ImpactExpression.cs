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
    public bool respectBinning = true;

    [Header("Particles")] public bool showParticles;
    public Color particleColor;
    public GameObject impactParticlePrefab;
    private float particleCooldown = 0f;

    private GameStateBehaviourScript _gameStateBehaviourScript;

    // Start is called before the first frame update
    void Start()
    {
        _gameStateBehaviourScript = FindObjectOfType<GameStateBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        particleCooldown -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Impact(other);
    }

    [ContextMenu("Impact!")]
    public void Impact(Collision2D other)
    {
        var contact = other.GetContact(0).point;
        Vector3 contactPosition = transform.position;
        contactPosition.x = contact.x;
        contactPosition.y = contact.y;
        contactPosition.z = contactPosition.z - 1f;

        if (playSound)
        {
            _gameStateBehaviourScript.musicManager.CreateAudioClip(impactSound, contactPosition, volumeMult,
                respectBinning);
        }

        if (showParticles && particleCooldown < 0)
        {
            print("showing particles from: " + gameObject.name);
            var particles = Instantiate(impactParticlePrefab);
            particles.transform.position = contactPosition;
            ParticleSystem system = particles.GetComponent<ParticleSystem>();
            var main = system.main;
            main.startColor = particleColor;

            particleCooldown = Time.fixedDeltaTime;
        }
    }
}