using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PparallaxScrolling : MonoBehaviour
{
    public bool lockX = false;
    public bool lockY = false;
    public float magnitude=1.0f;

    private GameStateBehaviourScript _gameStateBehaviourScript;
    private MainCameraController _cameraController;

    private Vector3 initialPos;
    private Vector3 initialCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        _gameStateBehaviourScript = FindObjectOfType<GameStateBehaviourScript>();
        _cameraController = FindObjectOfType<MainCameraController>();

        initialPos = transform.position;
        initialCameraPos = _cameraController.CameraPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        var camDist = initialCameraPos - _cameraController.CameraPos.position;
        float distX = camDist.x;
        float distY = camDist.y;


        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.x = newPos.x + distX * magnitude;
        newPos.y = newPos.y + distY * magnitude;

        if (lockX)
        {
            newPos.x = initialPos.x;
        }
        if (lockY)
        {
            newPos.y = initialPos.y;
        }

        transform.position = newPos;
    }
}