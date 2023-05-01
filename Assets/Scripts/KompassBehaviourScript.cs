using UnityEngine;
using UnityEngine.InputSystem;

public class KompassBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform pointerContainer;
    
    [SerializeField] private GameObject prefabStation;
    [SerializeField] private GameObject prefabContainer;
    
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private GameStateBehaviourScript gameState;

    [Header("Debug")]
    [SerializeField] private GameObject[] listOfContainerObj;
    [SerializeField] private GameObject[] listOfContainerPointer;
    
    [SerializeField] private GameObject[] listOfStationObj;
    [SerializeField] private GameObject[] listOfStationPointer;
    
    private void OnEnable()
    {
        if(gameState == null) gameState = FindObjectOfType<GameStateBehaviourScript>();
        gameState.onCurrentLevelListChange.AddListener(UpdateList);
    }

    private void OnDisable()
    {
        gameState.onCurrentLevelListChange.RemoveListener(UpdateList);
    }

    private void UpdateList(GameObject[] listOfStations, GameObject[] listOfContainer)
    {
        listOfContainerObj = listOfContainer;
        listOfStationObj = listOfStations;
        
        CleanPointer();
        CreatePointer();
    }

    private void CleanPointer()
    {
        if (listOfStationPointer != null)
        {
            for (int i = 0; i < listOfStationPointer.Length; i++)
            {
                GameObject obj = listOfStationPointer[i];
                Destroy(obj);
            }
            listOfStationPointer = new GameObject[0];
        }
        if (listOfContainerPointer != null)
        {
            for (int i = 0; i < listOfContainerPointer.Length; i++)
            {
                GameObject obj = listOfContainerPointer[i];
                Destroy(obj);
            }
            listOfContainerPointer = new GameObject[0];
        }
    }

    private void CreatePointer()
    {
        listOfContainerPointer = new GameObject[listOfContainerObj.Length];
        for (int i = 0; i < listOfContainerObj.Length; i++)
        {
            DumpableBehaviourScript dumb = listOfContainerObj[i].GetComponent<DumpableBehaviourScript>();
            listOfContainerPointer[i] = Instantiate(prefabContainer, pointerContainer);
            listOfContainerPointer[i].SetActive(true);
            
            if (dumb != null)
            {
                Vector2 pos = circleCollider2D.ClosestPoint(dumb.Rigidbody2D.worldCenterOfMass);
                listOfContainerPointer[i].transform.position = pos;
            }
        }
        
        listOfStationPointer = new GameObject[listOfStationObj.Length];
        for (int i = 0; i < listOfStationObj.Length; i++)
        {
            DumpStationBehaviourScript dumb = listOfStationObj[i].GetComponentInChildren<DumpStationBehaviourScript>();
            listOfStationPointer[i] = Instantiate(prefabStation, pointerContainer);
            listOfStationPointer[i].SetActive(true);
            
            if (dumb != null)
            {
                Vector2 pos = circleCollider2D.ClosestPoint(dumb.transform.position);
                listOfStationPointer[i].transform.position = pos;
            }
        }
    }

    private void UpdatePointer()
    {
        for (int i = 0; i < listOfContainerObj.Length; i++)
        {
            DumpableBehaviourScript dumb = listOfContainerObj[i].GetComponent<DumpableBehaviourScript>();
            if (dumb != null)
            {
                Vector3 pos = circleCollider2D.ClosestPoint(dumb.Rigidbody2D.worldCenterOfMass);
                pos.z = -8;
                listOfContainerPointer[i].transform.position = pos;
                listOfContainerPointer [i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(dumb.transform.position - pos));
            }
        }

        for (int i = 0; i < listOfStationObj.Length; i++)
        {
            DumpStationBehaviourScript dumb = listOfStationObj[i].GetComponentInChildren<DumpStationBehaviourScript>();
            if (dumb != null)
            {
                Vector3 pos = circleCollider2D.ClosestPoint(dumb.transform.position);
                pos.z = -8;
                listOfStationPointer[i].transform.position = pos;
                listOfStationPointer [i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(dumb.transform.position - pos));
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdatePointer();
        bool playerRequestsZoomOut = Keyboard.current.tabKey.isPressed;
        pointerContainer.gameObject.SetActive(playerRequestsZoomOut);
    }
}
 