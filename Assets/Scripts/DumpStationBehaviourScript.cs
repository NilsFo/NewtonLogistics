using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DumpStationBehaviourScript : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Collider2D myCollider2D;
    [SerializeField] private Transform generalAttractorPoint = null;
    [SerializeField] private Transform generalPushPoint = null;

    [Header("Config")]
    [SerializeField] private int stationIndex;
    [SerializeField] private string stationName;
    [SerializeField] private float pushPower = 5f;
    [SerializeField] private float suckPower = 5f;
    [SerializeField] private int maxContainerCount = 0;
    [SerializeField] private int currentLevelSuck = 0;
    
    [Header("Debug")]
    [SerializeField] private List<DumpableBehaviourScript> listOfInTrigger;
    [SerializeField] private List<DumpableBehaviourScript> listOfInTriggerWrongDump;
    [SerializeField] private List<DumpableBehaviourScript> listOfOverlapCenterMass;
    [SerializeField] private List<DumpableBehaviourScript> listOfSuck;

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
        if (listOfSuck == null)
        {
            listOfSuck = new List<DumpableBehaviourScript>();
        }
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
                if (myCollider2D.OverlapPoint(dump.GetPointOfIntrest()) 
                    && dump.DumpStationIndex == stationIndex 
                    && dump.Cargo.cargoState == Cargo.CargoState.Free)
                {
                    listOfOverlapCenterMass.Add(dump);
                }
            }
        }

        if (listOfInTriggerWrongDump.Count > 0)
        {
            for (int i = 0; i < listOfInTriggerWrongDump.Count; i++)
            {
                DumpableBehaviourScript dumpForPush = listOfInTriggerWrongDump[i];
                if (dumpForPush.Cargo.cargoState == Cargo.CargoState.Free)
                {
                    Transform transformDump = dumpForPush.transform;
                    //dumpForPush.Rigidbody2D.velocity = Vector2.zero;
                    /*transformDump.position = Vector3.MoveTowards(transformDump.position, generalPushPoint.position,
                        (pushPower * Time.deltaTime));*/
                    dumpForPush.Rigidbody2D.AddForce((generalPushPoint.position - transformDump.position).normalized * (500f * Time.deltaTime));
                }
            }
        }
        
        for (int i = 0; i < listOfOverlapCenterMass.Count; i++)
        {
             DumpableBehaviourScript dump = listOfOverlapCenterMass[i];
             if (dump.Cargo.cargoState == Cargo.CargoState.Free)
             {
                 if(!listOfSuck.Contains(dump)) listOfSuck.Add(dump);
             }
        }
        
        //SuckList
        for (int i = listOfSuck.Count-1; i > -1; i--)
        {
            DumpableBehaviourScript dump = listOfSuck[i];
            
            dump.Rigidbody2D.simulated = false;
            //TODO CargoScript NONO
             
            Transform transformDump = dump.transform;
            transformDump.position = Vector3.MoveTowards(transformDump.position, generalAttractorPoint.position,
                (suckPower * Time.deltaTime));
            
            //Its gone
            if (Vector3.Distance(transformDump.position, generalAttractorPoint.position) < 0.2f)
            {
                currentLevelSuck++;
                listOfSuck.RemoveAt(i);
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
    }

    public int StationIndex => stationIndex;

    public int GetContainerCount()
    {
        return currentLevelSuck;
    }

    public string GetStationName()
    {
        return stationName;
    }

    public void SetMaxCountContainer(int i)
    {
        maxContainerCount = i;
        currentLevelSuck = 0; //Reset Suck
    }

    public int MAXContainerCount => maxContainerCount;
}
