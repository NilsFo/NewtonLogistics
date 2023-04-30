using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShower : MonoBehaviour
{

    public PolygonCollider2D col;
    public GameObject showerTemplate;
    private GameStateBehaviourScript _gameStateBehaviourScript;
    private List<GameObject> myPoints;

    // Start is called before the first frame update
    void Start()
    {
        myPoints = new List<GameObject>();
        _gameStateBehaviourScript = FindObjectOfType<GameStateBehaviourScript>();

        foreach (var colPoint in col.points)
        {
            var newPos = Instantiate(showerTemplate, gameObject.transform);
            newPos.transform.localPosition = colPoint;
            myPoints.Add(newPos.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("enter: "+other.gameObject.name);
        var player = other.GetComponentInParent<ShipController>();
        if (player != null)
        {
            Show();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var player = other.GetComponentInParent<ShipController>();
        if (player != null)
        {
            Hide();
        }
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        print("hide");
        foreach (GameObject myPoint in myPoints)
        {
            _gameStateBehaviourScript.cameraController.RemoveFollowTarget(myPoint);
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        print("show");
        foreach (GameObject myPoint in myPoints)
        {
            _gameStateBehaviourScript.cameraController.AddFollowTarget(myPoint);
        }
    }

    private void OnDisable()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
