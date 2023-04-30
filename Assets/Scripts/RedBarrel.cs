using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBarrel : MonoBehaviour
{
    
    [Header("Fass Settings")]
    public bool explodeable;
    public float minHitStrength = 1;
    private GameStateBehaviourScript _gameStateBehaviourScript;

    [Header("Explosion Settings")]
    public Explosion Explosion; 

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
        float mag = other.relativeVelocity.magnitude;

        if (mag>=minHitStrength)
        {
            Explode();
        }
        else
        {
        }
    }

    public void Explode()
    {
        Explosion.Explode();
        Explosion.gameObject.transform.parent = null;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
    }
}