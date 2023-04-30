using UnityEngine;

public class KompassBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject prefabStation;
    [SerializeField] private GameObject prefabContainer;
    
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private GameStateBehaviourScript gameState;

    [Header("Debug")]
    [SerializeField] private DumpableBehaviourScript[] listOfDumps;
    [SerializeField] private GameObject[] listOfDumpableObj;
    
    [SerializeField] private DumpStationBehaviourScript[] listOfDumpStations;
    [SerializeField] private GameObject[] listOfDumpStationObj;
    
    private void OnEnable()
    {
        if(gameState == null) gameState = FindObjectOfType<GameStateBehaviourScript>();
        gameState.onDumpListChange.AddListener(UpdateDumps);
        gameState.onDumpStationChange.AddListener(UpdateDumpStations);
    }

    private void OnDisable()
    {
        gameState.onDumpListChange.RemoveListener(UpdateDumps);
        gameState.onDumpStationChange.RemoveListener(UpdateDumpStations);
    }

    private void UpdateDumps(DumpableBehaviourScript[] list)
    {
        listOfDumps = list;
        CleanDumpableObj();
        CreateDumpableObj();
        UpdateDumpableObj();
    }

    private void CleanDumpableObj()
    {
        if(listOfDumpableObj == null)
            return;
        for (int i = 0; i < listOfDumpableObj.Length; i++)
        {
            GameObject obj = listOfDumpableObj[i];
            Destroy(obj);
        }
        listOfDumpableObj = new GameObject[0];
    }

    private void CreateDumpableObj()
    {
        if(listOfDumps == null) return;
        listOfDumpableObj = new GameObject[listOfDumps.Length];
        for (int i = 0; i < listOfDumps.Length; i++)
        {
            DumpableBehaviourScript dumb = listOfDumps[i];
            listOfDumpableObj[i] = Instantiate(prefabContainer, transform);
            listOfDumpableObj[i].SetActive(true);
        }
    }

    private void UpdateDumpableObj()
    {
        if(listOfDumps.Length != listOfDumpableObj.Length) return;
        for (int i = 0; i < listOfDumps.Length; i++)
        {
            DumpableBehaviourScript dumb = listOfDumps[i];
            Vector2 pos = circleCollider2D.ClosestPoint(dumb.Rigidbody2D.worldCenterOfMass);
            listOfDumpableObj[i].transform.position = pos;
        }
    }

    private void UpdateDumpStations(DumpStationBehaviourScript[] list)
    {
        listOfDumpStations = list;
        CleanDumpStationObj();
        CreateDumpStationObj();
        UpdateDumpStationObj();
    }
    
    private void CleanDumpStationObj()
    {
        if(listOfDumpStationObj == null)
            return;
        for (int i = 0; i < listOfDumpStationObj.Length; i++)
        {
            GameObject obj = listOfDumpStationObj[i];
            Destroy(obj);
        }
        listOfDumpStationObj = new GameObject[0];
    }

    private void CreateDumpStationObj()
    {
        if(listOfDumpStations == null) return;
        listOfDumpStationObj = new GameObject[listOfDumpStations.Length];
        for (int i = 0; i < listOfDumpStations.Length; i++)
        {
            DumpStationBehaviourScript dumb = listOfDumpStations[i];
            listOfDumpStationObj[i] = Instantiate(prefabStation, transform);
            listOfDumpStationObj[i].SetActive(true);
        }
    }

    private void UpdateDumpStationObj()
    {
        if(listOfDumpStations.Length != listOfDumpStationObj.Length) return;
        for (int i = 0; i < listOfDumpStations.Length; i++)
        {
            DumpStationBehaviourScript dumb = listOfDumpStations[i];
            Vector2 pos = circleCollider2D.ClosestPoint(dumb.transform.position);
            listOfDumpStationObj[i].transform.position = pos;
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        UpdateDumpableObj();
        UpdateDumpStationObj();
    }
}
 