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
    private void Start()
    {
        Vector2Int[] stats = gameState.GetStatsArray();
        _stationPanels = new StationPanelBehaviourScript[stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            Vector2Int currrentStats = stats[i];
            GameObject obj = Instantiate(prefabStationLabel, container.transform);
            obj.SetActive(true);
            _stationPanels[i] = obj.GetComponent<StationPanelBehaviourScript>();
            _stationPanels[i].Init(i, currrentStats[0], currrentStats[1]);
        }
    }

    private void OnEnable()
    {
        gameState.onStatsChange.AddListener(OnStateChange);
    }

    private void OnDisable()
    {
        gameState.onStatsChange.RemoveListener(OnStateChange);
    }

    private void OnStateChange(int index, Vector2Int state)
    {
        _stationPanels[index].UpdateState(state[0]);
    }
}
