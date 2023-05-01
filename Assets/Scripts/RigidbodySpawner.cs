using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySpawner : MonoBehaviour {
    public Rigidbody2D bodyToSpawn;
    public float force = 100f;
    public float interval = 0f;

    private float _timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (interval <= 0) {
            Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (interval > 0) {
            _timer += Time.deltaTime;
            if (_timer > interval) {
                _timer -= interval;
                Spawn();
            }
        }
    }

    public void Spawn() {
        var body = Instantiate(bodyToSpawn, transform.position, transform.rotation);
        body.AddForce(transform.right * force, ForceMode2D.Impulse);
        body.AddTorque(Random.Range(-10f, 10f));
    }
}
