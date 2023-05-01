using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialRandomRotation : MonoBehaviour
{
    public bool torqueEnabled = true;
    public Rigidbody2D myRB;
    public float magnitude=1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (torqueEnabled)
        {
            myRB.AddTorque(Random.Range(magnitude * -1, magnitude));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}