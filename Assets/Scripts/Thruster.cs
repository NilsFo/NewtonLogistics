using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {
    private bool _vizActive;
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetViz(bool active) {
        if (active != _vizActive) {
            _vizActive = active;
            _spriteRenderer.color = active ? Color.yellow : Color.white;
        }
    }
    
}
