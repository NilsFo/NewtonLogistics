using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DumpableBehaviourScript : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [SerializeField] private int dumpStationIndex;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite[] sprites;

    private void Start()
    {
        Sprite currentSprite = defaultSprite;
        if (sprites.Length < dumpStationIndex)
            currentSprite = sprites[dumpStationIndex];
        spriteRenderer.sprite = currentSprite;
    }

    public Vector2 GetPointOfIntrest()
    {
        return rigidbody2D.worldCenterOfMass;
    }

    public int DumpStationIndex => dumpStationIndex;

    public BoxCollider2D BoxCollider2D => boxCollider2D;

    public Rigidbody2D Rigidbody2D => rigidbody2D;
    
    
    
}
