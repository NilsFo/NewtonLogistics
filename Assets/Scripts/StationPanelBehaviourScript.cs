using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StationPanelBehaviourScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLable;
    
    [SerializeField] private GameObject prefabLabel;
    [SerializeField] private GameObject container;

    [SerializeField] private Image[] images;
    
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    [SerializeField] private GameObject stationObj;
    [SerializeField] private DumpStationBehaviourScript stationBehaviourScript;
        
    public void Init(GameObject station)
    {
        stationObj = station;
        stationBehaviourScript = stationObj.GetComponentInChildren<DumpStationBehaviourScript>();
        if (stationBehaviourScript != null)
        {
            nameLable.text = stationBehaviourScript.GetStationName();
            InitLabels();
        }
    }

    private void Update()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        if(stationBehaviourScript == null) return;
        int currentState = stationBehaviourScript.GetContainerCount();
        if(0 > currentState || currentState >= stationBehaviourScript.MAXContainerCount) return;
        
        for (int i = 0; i < stationBehaviourScript.MAXContainerCount; i++)
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
        images = new Image[stationBehaviourScript.MAXContainerCount];
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
