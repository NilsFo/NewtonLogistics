using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour {

    private bool _fThrustOn, _lbThrustOn, _lfThrustOn, _rbThrustOn, _rfThrustOn, _bThrustOn, _magnetOn;
    public float forwardThrust = 1f, sideThrust = 1f, backwardThrust = 1f;

    public float pullForce = 100f;

    public Rigidbody2D rb;
    public Thruster thrusterF, thrusterB, thrusterLB, thrusterLF, thrusterRB, thrusterRF;

    public List<Connector> outsideConnectors;
    public List<Connector> insideConnectors;
    private Dictionary<Connector, Cargo> _nearbyConnectors = new Dictionary<Connector, Cargo>();
    public List<Cargo> allCargo;
    
    // Start is called before the first frame update
    void Start() {
        allCargo = new List<Cargo>(FindObjectsOfType<Cargo>());
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        UpdateViz();
    }

    private void FixedUpdate() {
        var tf = transform.localToWorldMatrix;
        if (_bThrustOn) {
            var pos = thrusterB.transform.position;
            var dir = tf.rotation * (Vector2.right * forwardThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_fThrustOn) {
            var pos = thrusterF.transform.position;
            var dir = tf.rotation * (Vector2.left * backwardThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_lbThrustOn) {
            var pos = thrusterLB.transform.position;
            var dir = tf.rotation * (Vector2.down * sideThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_lfThrustOn) {
            var pos = thrusterLF.transform.position;
            var dir = tf.rotation * (Vector2.down * sideThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_rbThrustOn) {
            var pos = thrusterRB.transform.position;
            var dir = tf.rotation * (Vector2.up * sideThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_rfThrustOn) {
            var pos = thrusterRF.transform.position;
            var dir = tf.rotation * (Vector2.up * sideThrust);
            rb.AddForceAtPosition(dir, pos);
            Debug.DrawLine(pos, pos + dir, Color.red, Time.fixedDeltaTime);
        }
        if (_magnetOn) {
            UpdateNearbyConnectors();
            PullConnectors();
        }
    }

    private void UpdateViz() {
        thrusterB.SetViz(_bThrustOn);
        thrusterF.SetViz(_fThrustOn);
        thrusterLF.SetViz(_lfThrustOn);
        thrusterLB.SetViz(_lbThrustOn);
        thrusterRB.SetViz(_rbThrustOn);
        thrusterRF.SetViz(_rfThrustOn);
    }

    void HandleInput() {
        var kb = Keyboard.current;
        _bThrustOn = _fThrustOn = _lbThrustOn = _lfThrustOn = _rbThrustOn = _rfThrustOn = _magnetOn = false;
        if (kb.wKey.isPressed) {
            _bThrustOn = true;
        }
        if (kb.sKey.isPressed) {
            _fThrustOn = true;
        }
        if (kb.qKey.isPressed) {
            _rbThrustOn = _rfThrustOn = true;
        }
        if (kb.eKey.isPressed) {
            _lbThrustOn = _lfThrustOn = true;
        }
        if (kb.aKey.isPressed) {
            _rfThrustOn = _lbThrustOn = true;
        }
        if (kb.dKey.isPressed) {
            _rbThrustOn = _lfThrustOn = true;
        }
        if (kb.fKey.isPressed) {
            _magnetOn = true;
        }
    }

    public void UpdateNearbyConnectors() {
        _nearbyConnectors.Clear();
        var contactFilter = new ContactFilter2D() {
            layerMask = LayerMask.GetMask("Cargo")
        };
        List<Collider2D> results = new List<Collider2D>();
        foreach (var outsideCon in outsideConnectors) {
            results.Clear();
            outsideCon.connectorTrigger.OverlapCollider(contactFilter, results);
            if (results.Count > 0) {
                foreach (var col in results) {
                    var cargo = col.GetComponent<Cargo>();
                    if(cargo != null)
                        _nearbyConnectors.Add(outsideCon, cargo);
                }
            }
        }
    }

    public void PullConnectors() {
        foreach (var kv in _nearbyConnectors) {
            var shipConnector = kv.Key;
            var cargo = kv.Value;
            foreach (var cargoConnector in cargo.connectors) {
                Vector2 delta = shipConnector.transform.position - cargoConnector.transform.position;
                var distance = delta.magnitude;
                if (distance < 2) {
                    cargo.rb.AddForceAtPosition(Time.fixedDeltaTime * pullForce * delta, cargoConnector.transform.position);
                    rb.AddForceAtPosition(Time.fixedDeltaTime * pullForce * -delta, shipConnector.transform.position);
                    Debug.DrawLine(cargoConnector.transform.position, shipConnector.transform.position, Color.yellow, Time.fixedDeltaTime);
                }
                if (distance < 0.1f) {
                    Connect(shipConnector, cargo);
                }
            }
        }
    }
    private void Connect(Connector connector, Cargo cargo) {
        var joint = cargo.AddComponent<FixedJoint2D>();
        cargo.gameObject.layer = LayerMask.NameToLayer("Ship");
        joint.connectedBody = rb;
        joint.anchor = connector.transform.position;
    }
}
