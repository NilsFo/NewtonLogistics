using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour {
    public Rigidbody2D rb;
    public Connector[] connectors;
    public enum CargoState {
        Free, Attached
    }
    public CargoState cargoState;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
