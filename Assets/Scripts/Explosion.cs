using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Explosion : MonoBehaviour
{
    [Header("Explosion Settings")] public float explosionMagnitude;
    public float explosionRadius;

    [FormerlySerializedAs("explosionDistanceMult")]
    public AnimationCurve explosionDistanceMultCurve;

    public CircleCollider2D pushCollider;

    private GameStateBehaviourScript _gameStateBehaviourScript;

    [Header("Expansion Settings")] private List<GameObject> alreadyExplodedObjects;
    public bool explosionInProgress;
    public bool stopExplosionNextFrame;
    public float explosionSpeed = 1;

    private void Awake()
    {
        pushCollider.radius = 0.01f;
        pushCollider.enabled = false;
        stopExplosionNextFrame = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        explosionInProgress = false;
        alreadyExplodedObjects = new List<GameObject>();
        _gameStateBehaviourScript = FindObjectOfType<GameStateBehaviourScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopExplosionNextFrame)
        {
            explosionInProgress = false;
            stopExplosionNextFrame = false;
            _gameStateBehaviourScript.cameraController.RemoveFollowTarget(gameObject);
            Destroy(gameObject);
        }

        if (explosionInProgress)
        {
            float newRad = pushCollider.radius + Time.deltaTime * explosionSpeed;
            newRad = Mathf.Min(newRad, explosionRadius);
            pushCollider.radius = newRad;
            if (pushCollider.radius >= explosionRadius)
            {
                stopExplosionNextFrame = true;
            }
        }
        else
        {
            pushCollider.radius = 0.01f;
            stopExplosionNextFrame = false;
        }
    }

    [ContextMenu("Explode now")]
    public void Explode()
    {
        explosionInProgress = true;
        pushCollider.enabled = true;
        alreadyExplodedObjects = new List<GameObject>();

        _gameStateBehaviourScript.cameraController.AddFollowTarget(gameObject);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Debug.DrawLine(transform.position, roamingOrigin);
        Vector3 wireOrigin = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        UnityEditor.Handles.DrawWireDisc(wireOrigin, Vector3.forward, explosionRadius);

        UnityEditor.Handles.DrawWireDisc(wireOrigin, Vector3.forward,
            explosionRadius * explosionDistanceMultCurve.Evaluate(0.25f));
        UnityEditor.Handles.DrawWireDisc(wireOrigin, Vector3.forward,
            explosionRadius * explosionDistanceMultCurve.Evaluate(0.5f));
        UnityEditor.Handles.DrawWireDisc(wireOrigin, Vector3.forward,
            explosionRadius * explosionDistanceMultCurve.Evaluate(0.75f));
    }
#endif
    public float GetExplosionRangeProgressPercent()
    {
        return pushCollider.radius / explosionRadius;
    }

    public void PushObject(GameObject other)
    {
        var rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 impulse = rb.worldCenterOfMass - (Vector2)transform.position;
            impulse = impulse.normalized;
            impulse = impulse * (explosionMagnitude *
                                 (1.0f - explosionDistanceMultCurve.Evaluate(GetExplosionRangeProgressPercent())));
            rb.AddForce(impulse, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gm = other.gameObject;
        if (!alreadyExplodedObjects.Contains(gm))
        {
            alreadyExplodedObjects.Add(gm);
            PushObject(gm);
        }
    }
}