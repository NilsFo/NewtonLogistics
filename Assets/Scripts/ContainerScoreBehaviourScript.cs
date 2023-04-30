using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContainerScoreBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject prefabStationLabel;
    [SerializeField] private GameObject container;
    
    [SerializeField] private GameStateBehaviourScript gameState;

    private StationPanelBehaviourScript[] _stationPanels;
    private DumpStationBehaviourScript[] listOfStations;
    
    private void Start()
    {
        CleanStationPanels();
        CreateStationPanels();
    }

    private void CleanStationPanels()
    {
        if(_stationPanels == null) return;
        for (int i = 0; i < _stationPanels.Length; i++)
        {
            StationPanelBehaviourScript panel = _stationPanels[i];
            Destroy(panel.gameObject);
        }
        _stationPanels = null;
    }

    private void CreateStationPanels()
    {
        if(listOfStations == null) return;
        _stationPanels = new StationPanelBehaviourScript[listOfStations.Length];
        for (int i = 0; i < listOfStations.Length; i++)
        {
            DumpStationBehaviourScript station = listOfStations[i];
            GameObject obj = Instantiate(prefabStationLabel, container.transform);
            obj.SetActive(true);
            _stationPanels[i] = obj.GetComponent<StationPanelBehaviourScript>();
            _stationPanels[i].Init(i);
        }
    }
    
    private void OnEnable()
    {
        gameState.onDumpStationChange.AddListener(OnStationListChange);
    }

    private void OnDisable()
    {
        gameState.onDumpStationChange.RemoveListener(OnStationListChange);
    }

    private void OnStationListChange(DumpStationBehaviourScript[] list)
    {
        listOfStations = list;
        CleanStationPanels();
        CreateStationPanels();
    }
}
