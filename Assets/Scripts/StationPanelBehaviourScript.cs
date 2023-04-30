using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StationPanelBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameStateBehaviourScript gameState;
    [SerializeField] private TextMeshProUGUI nameLable;
    
    [SerializeField] private GameObject prefabLabel;
    [SerializeField] private GameObject container;

    [SerializeField] private Image[] images;
    
    [SerializeField] private int stationIndex = 0;
    [SerializeField] private int currentState = 0;
    [SerializeField] private int maxState = 0;

    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    private void Awake()
    {
        gameState = FindObjectOfType<GameStateBehaviourScript>();
    }

    private void OnEnable()
    {
        gameState.onStatsChange.AddListener(OnStatsChange);
    }

    private void OnDisable()
    {
        gameState.onStatsChange.RemoveListener(OnStatsChange);
    }

    private void OnStatsChange(int index, Vector2Int values)
    {
        if (index == stationIndex)
        {
            maxState = values[1];
            currentState = values[0];
            InitLabels();
            UpdateState();
        }
    }

    public void Init(int index)
    {
        stationIndex = index;
        nameLable.text = "Station " + (index + 1);
        maxState = 0;
        currentState = 0;
        InitLabels();
        UpdateState();
    }

    public void UpdateState()
    {
        if(0 > currentState || currentState >= maxState) return;
        
        for (int i = 0; i < maxState; i++)
        {
            if (i < currentState)
            {
                images[i].sprite = onSprite;
            }
            else
            {
                images[i].sprite = offSprite; 
            }
        }
    }

    private void InitLabels()
    {
        CleanLabels();
        images = new Image[maxState];
        for (int i = 0; i < images.Length; i++)
        {
            GameObject obj = Instantiate(prefabLabel, container.transform);
            obj.SetActive(true);
            images[i] = obj.GetComponentInChildren<Image>();
        }
    }

    private void CleanLabels()
    {
        if (images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                Destroy(images[i].gameObject);
            }
        }
    }
}
