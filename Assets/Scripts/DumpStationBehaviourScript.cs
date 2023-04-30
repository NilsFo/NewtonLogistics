using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DumpStationBehaviourScript : MonoBehaviour
{
    [SerializeField] private Collider2D myCollider2D;
    [SerializeField] private GameStateBehaviourScript gameState;
    [SerializeField] private TextMeshProUGUI textLabel;
    
    [SerializeField] private int stationIndex;
    [SerializeField] private float pushPower = 5f;
    
    [SerializeField] private List<DumpableBehaviourScript> listOfInTrigger;
    [SerializeField] private List<DumpableBehaviourScript> listOfInTriggerWrongDump;
    [SerializeField] private List<DumpableBehaviourScript> listOfOverlapCenterMass;

    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (listOfInTrigger == null)
        {
            listOfInTrigger = new List<DumpableBehaviourScript>();
        }
        if (listOfOverlapCenterMass == null)
        {
            listOfOverlapCenterMass = new List<DumpableBehaviourScript>();
        }
        if (listOfInTriggerWrongDump == null)
        {
            listOfInTriggerWrongDump = new List<DumpableBehaviourScript>();
        }
    }

    private void Start()
    {
        textLabel.text = "" + (stationIndex + 1);
        gameState = FindObjectOfType<GameStateBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (listOfOverlapCenterMass.Count != listOfInTrigger.Count)
        {
            listOfOverlapCenterMass = new List<DumpableBehaviourScript>();
            for (int i = 0; i < listOfInTrigger.Count; i++)
            {
                DumpableBehaviourScript dump = listOfInTrigger[i];
                if (myCollider2D.OverlapPoint(dump.GetPointOfIntrest()))
                {
                    listOfOverlapCenterMass.Add(dump);
                }
            }
            gameState.SetPoints(stationIndex, listOfOverlapCenterMass.Count);
        }

        if (listOfInTriggerWrongDump.Count > 0)
        {
            for (int i = 0; i < listOfInTriggerWrongDump.Count; i++)
            {
                DumpableBehaviourScript dumpForPush = listOfInTriggerWrongDump[i];
                Rigidbody2D rigidbody2D = dumpForPush.Rigidbody2D;
                
                Vector2 direction = rigidbody2D.position - (Vector2) myCollider2D.bounds.center;
                rigidbody2D.AddForce(direction.normalized * (pushPower * Time.deltaTime), ForceMode2D.Impulse);
            }
            
        }
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        DumpableBehaviourScript dumpableBehaviourScript = col.gameObject.GetComponent<DumpableBehaviourScript>();
        if (dumpableBehaviourScript == null)
        {
            return;
        }

        if (dumpableBehaviourScript.DumpStationIndex == stationIndex)
        {
            listOfInTrigger.Add(dumpableBehaviourScript);
        }
        else
        {
            listOfInTriggerWrongDump.Add(dumpableBehaviourScript);
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        DumpableBehaviourScript dumpableBehaviourScript = col.gameObject.GetComponent<DumpableBehaviourScript>();
        if (dumpableBehaviourScript == null)
        {
            return;
        }
        listOfInTrigger.Remove(dumpableBehaviourScript);
        listOfOverlapCenterMass.Remove(dumpableBehaviourScript);
        listOfInTriggerWrongDump.Remove(dumpableBehaviourScript);
        gameState.SetPoints(stationIndex, listOfOverlapCenterMass.Count);
    }
}
