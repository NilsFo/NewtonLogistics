using UnityEngine;

public class ContainerScoreBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject prefabStationLabel;
    [SerializeField] private GameObject container;
    
    [SerializeField] private GameStateBehaviourScript gameState;

    [SerializeField] private GameObject[] listOfContainerObj;
    [SerializeField] private GameObject[] listOfStationObj;

    private StationPanelBehaviourScript[] _stationPanels;
    
    private void Start()
    {
        _stationPanels = new StationPanelBehaviourScript[0];
        CleanStationPanels();
        CreateStationPanels();
    }

    private void OnEnable()
    {
        gameState.onCurrentLevelListChange.AddListener(OnCurrentLevelListChange);
    }

    private void OnDisable()
    {
        gameState.onCurrentLevelListChange.RemoveListener(OnCurrentLevelListChange);
    }

    
    private void OnCurrentLevelListChange(GameObject[] listOfStations, GameObject[] listOfContainer)
    {
        listOfContainerObj = listOfContainer;
        listOfStationObj = listOfStations;
        
        CleanStationPanels();
        CreateStationPanels();
    }
    
    private void CleanStationPanels()
    {
        if(_stationPanels == null) return;
        for (int i = 0; i < _stationPanels.Length; i++)
        {
            GameObject panel = _stationPanels[i].gameObject;
            Destroy(panel);
        }
        _stationPanels = null;
    }

    private void CreateStationPanels()
    {
        if(listOfStationObj == null) return;
        _stationPanels = new StationPanelBehaviourScript[listOfStationObj.Length];
        for (int i = 0; i < listOfStationObj.Length; i++)
        {
            GameObject station = listOfStationObj[i];
            GameObject obj = Instantiate(prefabStationLabel, container.transform);
            obj.SetActive(true);
            _stationPanels[i] = obj.GetComponent<StationPanelBehaviourScript>();
            _stationPanels[i].Init(station);
        }
    }
}
