using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DumpableBehaviourScript : MonoBehaviour
{

    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private int dumpStationIndex;

    private void Start()
    {
        textLabel.text = "" + (dumpStationIndex + 1);
    }

    public Vector2 GetPointOfIntrest()
    {
        return boxCollider2D.offset;
    }

    public int DumpStationIndex => dumpStationIndex;

    public BoxCollider2D BoxCollider2D => boxCollider2D;

    public Rigidbody2D Rigidbody2D => rigidbody2D;
    
}
