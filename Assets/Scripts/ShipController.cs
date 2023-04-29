using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class ShipController : MonoBehaviour {

    private bool _fThrustOn, _lbThrustOn, _lfThrustOn, _rbThrustOn, _rfThrustOn, _bThrustOn, _magnetOn;
    public float forwardThrust = 1f, sideThrust = 1f, backwardThrust = 1f;

    public float pullForce = 100f, pullDistance = 2f;

    public Rigidbody2D rb;
    public Thruster thrusterF, thrusterB, thrusterLB, thrusterLF, thrusterRB, thrusterRF;

    public List<Connector> outsideConnectors;
    private static readonly int MAX_CONNECTORS = 10; 
    private (Connector, Connector)[] _insideConnectors = new (Connector, Connector)[MAX_CONNECTORS];
    private List<(Connector, Cargo)> _nearbyConnectors = new List<(Connector, Cargo)>();
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
        if (kb.digit1Key.wasPressedThisFrame) {
            Disconnect(0);
        }
        if (kb.digit2Key.wasPressedThisFrame) {
            Disconnect(1);
        }
        if (kb.digit3Key.wasPressedThisFrame) {
            Disconnect(2);
        }
        if (kb.digit4Key.wasPressedThisFrame) {
            Disconnect(3);
        }
        if (kb.digit5Key.wasPressedThisFrame) {
            Disconnect(4);
        }
        if (kb.digit6Key.wasPressedThisFrame) {
            Disconnect(5);
        }
        if (kb.digit7Key.wasPressedThisFrame) {
            Disconnect(6);
        }
        if (kb.digit8Key.wasPressedThisFrame) {
            Disconnect(7);
        }
        if (kb.digit9Key.wasPressedThisFrame) {
            Disconnect(8);
        }
        if (kb.digit0Key.wasPressedThisFrame) {
            Disconnect(9);
        }
    }

    public void UpdateNearbyConnectors() {
        _nearbyConnectors.Clear();
        var contactFilter = new ContactFilter2D() {
            layerMask = LayerMask.GetMask("Cargo")
        };
        List<Collider2D> results = new List<Collider2D>();
        foreach (var outsideCon in outsideConnectors) {
            outsideCon.connectorState = Connector.ConnectorState.AttachedOutside;
            results.Clear();
            outsideCon.connectorTrigger.OverlapCollider(contactFilter, results);
            if (results.Count > 0) {
                foreach (var col in results) {
                    var cargo = col.GetComponent<Cargo>();
                    if(cargo != null && cargo.cargoState == Cargo.CargoState.Free)
                        _nearbyConnectors.Add((outsideCon, cargo));
                }
            }
        }
    }

    public void PullConnectors() {
        _nearbyConnectors = _nearbyConnectors.OrderBy(tuple => (tuple.Item1.transform.position - tuple.Item2.transform.position).magnitude).ToList();
        List<Cargo> cargoBeingPulled = new List<Cargo>();
        foreach (var (shipConnector, cargo) in _nearbyConnectors) {
            if(shipConnector.connectorState != Connector.ConnectorState.AttachedOutside || cargoBeingPulled.Contains(cargo))
                continue;

            var minDistance = float.MaxValue;
            Vector2 delta = new Vector2();
            Connector cargoConnector = null;
            foreach (var cargoConnectorT in cargo.connectors) {
                Vector2 deltaT = shipConnector.transform.position - cargoConnectorT.transform.position;
                var distance = deltaT.magnitude;
                if (distance < minDistance) {
                    minDistance = distance;
                    delta = deltaT;
                    cargoConnector = cargoConnectorT;
                }
            }
            if (minDistance < pullDistance) {
                var modPullForce = pullForce * (1 - minDistance / pullDistance) * (1 - minDistance / pullDistance);
                cargo.rb.AddForceAtPosition(Time.fixedDeltaTime * modPullForce * delta, cargoConnector.transform.position);
                rb.AddForceAtPosition(Time.fixedDeltaTime * modPullForce * -delta, shipConnector.transform.position);
                Debug.DrawLine(cargoConnector.transform.position, shipConnector.transform.position, Color.yellow, Time.fixedDeltaTime);
                shipConnector.connectorState = Connector.ConnectorState.AttachedOutsidePulling;
                cargoBeingPulled.Add(cargo);
            }
            if (minDistance < 0.1f) {
                Connect(shipConnector, cargo, cargoConnector);
            }
        }
    }
    
    private void Connect(Connector shipConnector, Cargo cargo, Connector cargoConnector) {
        if (cargo.cargoState == Cargo.CargoState.Attached) {
            // No need to attach something already attached
            return;
        }

        cargoConnector.connectorState = Connector.ConnectorState.AttachedInside;
        shipConnector.connectorState = Connector.ConnectorState.AttachedInside;

        cargo.cargoState = Cargo.CargoState.Attached;
        
        cargo.gameObject.layer = LayerMask.NameToLayer("Ship");
        cargo.gameObject.transform.parent = transform;
        
        // Snap to grid
        /*var relPos = transform.worldToLocalMatrix.MultiplyPoint3x4(cargo.transform.position);
        var x = relPos.x;
        var y = relPos.y;
        x = Mathf.Round(x / 2) * 2;
        y = Mathf.Round(y / 2) * 2;
        cargo.transform.position = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector2(x, y));
        var angle = cargo.transform.rotation.eulerAngles.z - transform.rotation.eulerAngles.z;
        angle = Mathf.Round(angle / 90f) * 90f;
        cargo.transform.rotation = Quaternion.Euler(0,0, transform.rotation.eulerAngles.z + angle);*/

        for (int i = 0; i < _insideConnectors.Length; i++) {
            if (_insideConnectors [i] == (null, null)) {
                _insideConnectors [i] = (shipConnector, cargoConnector);
                break;
            }
            Debug.LogWarning("Could not connect cargo, is the maximum number of connections reached?");
        }
        outsideConnectors.Remove(shipConnector);
        foreach (var cargoCon in cargo.connectors) {
            cargoCon.gameObject.layer = LayerMask.NameToLayer("Ship");
            if (cargoCon != cargoConnector) {
                outsideConnectors.Add(cargoCon);
            }
        }
        
        var joint = cargo.AddComponent<FixedJoint2D>();
        joint.connectedBody = rb;
        joint.anchor = cargoConnector.transform.localPosition;
    }

    public void Disconnect(int index) {
        // Find other links connected to this one
        var (stayConnector, leaveConnector) = _insideConnectors [index];
        if (stayConnector == null) {
            Debug.Log("Can't disconnect connector " + index + ", no such connector on record");
            return;
        }
        var cargo = leaveConnector.GetComponentInParent<Cargo>();
        foreach (var cargoConnector in cargo.connectors) {
            if (cargoConnector.connectorState == Connector.ConnectorState.AttachedInside) {
                for (int i = 0; i < MAX_CONNECTORS; i++) {
                    if (_insideConnectors [i].Item1 == cargoConnector) {
                        // Disconnect down the line first
                        Disconnect(i);
                    }
                }
            }
        }
        cargo.transform.parent = null;
        cargo.cargoState = Cargo.CargoState.Free;
        Destroy(cargo.GetComponent<FixedJoint2D>());
        foreach (var cargoConnector in cargo.connectors) {
            cargoConnector.connectorState = Connector.ConnectorState.Cargo;
            outsideConnectors.Remove(cargoConnector);
        }
        stayConnector.connectorState = Connector.ConnectorState.AttachedOutside;
        outsideConnectors.Add(stayConnector);
        _insideConnectors [index] = (null, null);
        cargo.rb.AddForce((cargo.transform.position - stayConnector.transform.position).normalized * 1f, ForceMode2D.Impulse);
        cargo.rb.AddTorque(Random.Range(-0.5f, 0.5f), ForceMode2D.Impulse);
    }
}
