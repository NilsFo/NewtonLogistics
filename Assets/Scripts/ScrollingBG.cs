using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBG : MonoBehaviour
{
    public float speed = 0.69f;
    private float dt;
    private float x, y, z;
    public bool resetable = true;

    public float resetDist = 5;

    // Start is called before the first frame update
    void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        dt += Time.deltaTime * speed;
        if (transform.position.x > resetDist && resetable)
        {
            dt = 0;
        }
        y = transform.position.y;

        Vector3 pos = new Vector3(x + dt, y, z);
        transform.position = pos;
    }
}