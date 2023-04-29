using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private GameStateBehaviourScript _gameState;
    private CinemachineBrain cmBrain;
    public GameObject cameraFollow;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameStateBehaviourScript>();
        cmBrain = FindObjectOfType<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        cameraFollow.transform.position = _gameState.player.transform.position;
    }
}