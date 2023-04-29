using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour {
    public Collider2D possibleConnection;
    public Rigidbody2D shipRB; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
    int cargoLayer = LayerMask.NameToLayer("Cargo"); 
        if(other.gameObject.layer == cargoLayer)
            possibleConnection = other;
        
    }

    public bool TryConnect() {
        if(possibleConnection == null)
            return false;
        var conRB = possibleConnection.GetComponent<Rigidbody2D>();
        if (conRB == null)
            return false;
        Vector2 myPos = transform.position;
        var pullPoint = conRB.ClosestPoint(myPos);
        var force = (myPos - pullPoint).normalized * 1f;
        conRB.AddForceAtPosition(force, pullPoint, ForceMode2D.Impulse);
        shipRB.AddForceAtPosition(-force, myPos, ForceMode2D.Impulse);
        return true;
    }
}
