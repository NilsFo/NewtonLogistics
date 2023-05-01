using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject panel;
    public bool showing;
    private GameStateBehaviourScript _gameState;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = FindObjectOfType<GameStateBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            showing = !showing;
        }

        panel.gameObject.SetActive(showing);

        if (showing)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("MainMenuScene");
            }

            if (Keyboard.current.backspaceKey.wasPressedThisFrame)
            {
                _gameState.RestartLevel();
            }
        }
    }
}