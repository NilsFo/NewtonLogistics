using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrainBehaviourScript : MonoBehaviour
{
    [SerializeField] private float pushPower = 5f;
    [SerializeField] private BoxCollider2D collider;
    
    [SerializeField] private List<Rigidbody2D> listOfInTrigger;

    private void Awake()
    {
        if (listOfInTrigger == null)
        {
            listOfInTrigger = new List<Rigidbody2D>();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (listOfInTrigger.Count > 0)
        {
            for (int i = 0; i < listOfInTrigger.Count; i++)
            {
                Rigidbody2D tragetRigidbody2D = listOfInTrigger[i];

                Vector2 direction = tragetRigidbody2D.position - (Vector2) collider.bounds.center;
                tragetRigidbody2D.AddForce(direction.normalized * (pushPower * Time.deltaTime), ForceMode2D.Impulse);
            }
            
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Rigidbody2D currentRigidbody2D = col.gameObject.GetComponent<Rigidbody2D>();
        if (currentRigidbody2D == null)
        {
            return;
        }
        listOfInTrigger.Add(currentRigidbody2D);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        Rigidbody2D currentRigidbody2D = col.gameObject.GetComponent<Rigidbody2D>();
        if (currentRigidbody2D == null)
        {
            return;
        }
        listOfInTrigger.Remove(currentRigidbody2D);
    }
    
}
