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
    
    [SerializeField] private int currentState = 0;
    [SerializeField] private int maxState = 0;
    
    public void Init(int index, int currentState, int maxState)
    {
        nameLable.text = "Station " + (index + 1);
        this.maxState = maxState;
        this.currentState = currentState;
        InitLabels();
        UpdateState(currentState);
    }

    public void UpdateState(int currentState)
    {
        if(0 < currentState && currentState <= maxState)
        this.currentState = currentState;
        for (int i = 0; i < maxState; i++)
        {
            if (i < currentState)
            {
               images[i].color = Color.green; 
            }
            else
            {
                images[i].color = Color.red; 
            }
        }
    }

    private void InitLabels()
    {
        if (images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                Destroy(images[i].gameObject);
            }
        }
        images = new Image[maxState];
        for (int i = 0; i < images.Length; i++)
        {
            GameObject obj = Instantiate(prefabLabel, container.transform);
            obj.SetActive(true);
            images[i] = obj.GetComponentInChildren<Image>();
        }
    }
}
