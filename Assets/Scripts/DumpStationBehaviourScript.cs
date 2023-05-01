using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DumpStationBehaviourScript : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Collider2D myCollider2D;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private Transform generalAttractorPoint = null;
    [SerializeField] private Transform[] specificAttractorPoint = new Transform[0];
        
    [Header("Config")]
    [SerializeField] private int stationIndex;
    [SerializeField] private string stationName;
    [SerializeField] private float pushPower = 5f;
    [SerializeField] private int maxContainerCount = 0;
    
    [Header("Debug")]
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
                if (myCollider2D.OverlapPoint(dump.GetPointOfIntrest()) && dump.DumpStationIndex == stationIndex)
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
                Rigidbody2D rigidbody2D = dumpForPush.Rigidbody2D;
                
                Vector2 direction = rigidbody2D.position - (Vector2) myCollider2D.bounds.center;
                rigidbody2D.AddForce(direction.normalized * (pushPower * Time.deltaTime), ForceMode2D.Impulse);
            }
        }

        if (listOfOverlapCenterMass.Count > 0)
        {
            if (generalAttractorPoint != null)
            {
                 for (int i = 0; i < listOfOverlapCenterMass.Count; i++)
                 {
                     DumpableBehaviourScript dump = listOfOverlapCenterMass[i];
                     if (dump.Cargo.cargoState == Cargo.CargoState.Free)
                     {
                         Vector2 direction = dump.Rigidbody2D.position - (Vector2) generalAttractorPoint.position;
                         if (direction.magnitude < 0.2f)
                         { 
                             dump.Rigidbody2D.AddForce(direction.normalized * (pushPower * Time.deltaTime), ForceMode2D.Impulse);  
                         }
                     }
                 }   
            }
            else if (specificAttractorPoint.Length > 0)
            {
                for (int i = 0; i < listOfOverlapCenterMass.Count; i++)
                {
                    if (i < specificAttractorPoint.Length)
                    {
                        DumpableBehaviourScript dump = listOfOverlapCenterMass[i];
                        if (dump.Cargo.cargoState == Cargo.CargoState.Free)
                        {
                            Vector2 direction = dump.Rigidbody2D.position - (Vector2) specificAttractorPoint[i].position;
                            if (direction.magnitude < 0.2f)
                            { 
                                dump.Rigidbody2D.AddForce(direction.normalized * (pushPower * Time.deltaTime), ForceMode2D.Impulse);  
                            }
                        }
                    }
                } 
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
        return listOfOverlapCenterMass.Count;
    }

    public string GetStationName()
    {
        return stationName;
    }

    public void SetMaxCountContainer(int i)
    {
        maxContainerCount = i;
    }

    public int MAXContainerCount => maxContainerCount;
}
