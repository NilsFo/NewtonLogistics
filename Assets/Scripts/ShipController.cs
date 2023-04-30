using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<(Connector, Connector)> _nearbyConnectors = new List<(Connector, Connector)>();
    public List<Cargo> connectedCargo = new List<Cargo>();

    private GameStateBehaviourScript gameState;
    
    // Start is called before the first frame update
    void Start() {
        gameState = FindObjectOfType<GameStateBehaviourScript>();
        InvokeRepeating(nameof(UpdateNearbyConnectors), 0.1f, 0.1f);
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
        if (kb.eKey.isPressed) {
            _rbThrustOn = _rfThrustOn = true;
        }
        if (kb.qKey.isPressed) {
            _lbThrustOn = _lfThrustOn = true;
        }
        if (kb.aKey.isPressed) {
            _rfThrustOn = _lbThrustOn = true;
        }
        if (kb.dKey.isPressed) {
            _rbThrustOn = _lfThrustOn = true;
        }
        if (kb.fKey.isPressed || kb.spaceKey.isPressed) {
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
        var contactFilter = new ContactFilter2D() {
            layerMask = LayerMask.GetMask("Cargo")
        };
        _nearbyConnectors.Clear();
        List<Collider2D> results = new List<Collider2D>();
        foreach (var outsideCon in outsideConnectors) {
            if(!_magnetOn)
                outsideCon.connectorState = Connector.ConnectorState.AttachedOutside;
            results.Clear();
            outsideCon.connectorTrigger.OverlapCollider(contactFilter, results);
            if (results.Count > 0) {
                foreach (var col in results) {
                    var cargo = col.GetComponent<Cargo>();
                    if (cargo != null && cargo.cargoState == Cargo.CargoState.Free) {
                        foreach (var cargoConnector in cargo.connectors) {
                            if((cargoConnector.transform.position - outsideCon.transform.position).magnitude <= pullDistance)
                                _nearbyConnectors.Add((outsideCon, cargoConnector));
                        }
                    }
                }
            }
        }
        
        _nearbyConnectors = _nearbyConnectors.OrderBy(tuple => (tuple.Item1.transform.position - tuple.Item2.transform.position).magnitude).ToList();
        List<Connector> connectorsOccupied = new List<Connector>();
        List<Cargo> cargoOccupied = new List<Cargo>();
        List<(Connector, Connector)> tempConnectors = new List<(Connector, Connector)>();
        foreach (var (outsideCon, cargoCon) in _nearbyConnectors) {
            var cargo = cargoCon.GetComponentInParent<Cargo>();
            if (connectorsOccupied.Contains(outsideCon) || cargoOccupied.Contains(cargo)) {
                continue;
            }
            if(outsideCon.connectorState != Connector.ConnectorState.AttachedOutsidePulling)
                outsideCon.connectorState = Connector.ConnectorState.AttachedOutsideCouldPull;
            outsideCon.couldConnect = cargoCon;
            connectorsOccupied.Add(outsideCon);
            cargoOccupied.Add(cargo);
            tempConnectors.Add((outsideCon, cargoCon));
        }
        _nearbyConnectors = tempConnectors;
    }

    public void PullConnectors() {
        List<Cargo> cargoBeingPulled = new List<Cargo>();
        foreach (var (shipConnector, cargoConnector) in _nearbyConnectors) {
            var cargo = cargoConnector.GetComponentInParent<Cargo>();

            Vector2 delta = shipConnector.transform.position - cargoConnector.transform.position;
            var distance = delta.magnitude;

            if (distance < pullDistance) {
                var modPullForce = pullForce * (1 - (-1 + distance) / pullDistance) * (1 - (-1 + distance) / pullDistance);
                cargo.rb.AddForceAtPosition(Time.fixedDeltaTime * modPullForce * delta, cargoConnector.transform.position);
                rb.AddForceAtPosition(Time.fixedDeltaTime * modPullForce * -delta, shipConnector.transform.position);
                Debug.DrawLine(cargoConnector.transform.position, shipConnector.transform.position, Color.yellow, Time.fixedDeltaTime);
                shipConnector.connectorState = Connector.ConnectorState.AttachedOutsidePulling;
                cargoBeingPulled.Add(cargo);
            }
            if (distance < 0.15f) {
                Connect(shipConnector, cargo, cargoConnector);
            }
        }
    }

    private int AddInsideConnection(Connector shipConnector, Connector cargoConnector) {
        
        for (int i = 0; i < _insideConnectors.Length; i++) {
            if (_insideConnectors [i] == (null, null)) {
                _insideConnectors [i] = (shipConnector, cargoConnector);
                return i;
            }
        }
        Debug.LogWarning("Could not connect cargo, is the maximum number of connections reached?");
        return -1;
    }
    
    private void Connect(Connector shipConnector, Cargo cargo, Connector cargoConnector) {
        if (cargo.cargoState == Cargo.CargoState.Attached) {
            // No need to attach something already attached
            return;
        }

        int insideConnectionIndex = AddInsideConnection(shipConnector, cargoConnector);
        if (insideConnectionIndex == -1) {
            return;
        }
        
        cargoConnector.connectorState = Connector.ConnectorState.AttachedInside;
        shipConnector.connectorState = Connector.ConnectorState.AttachedInside;

        cargo.cargoState = Cargo.CargoState.Attached;
        
        cargo.gameObject.layer = LayerMask.NameToLayer("Ship");
        cargo.transform.parent = shipConnector.transform.parent;
        
        // Snap to grid
        /*var relPos = transform.worldToLocalMatrix.MultiplyPoint3x4(cargo.transform.position);
        var x = relPos.x;
        var y = relPos.y;
        x = Mathf.Round(x / 2) * 2;
        y = Mathf.Round(y / 2) * 2;
        //cargo.transform.position = transform.localToWorldMatrix.MultiplyPoint3x4(new Vector2(x, y));
        var angle = cargo.transform.rotation.eulerAngles.z - transform.rotation.eulerAngles.z;
        angle = Mathf.Round(angle / 90f) * 90f;
        //cargo.transform.rotation = Quaternion.Euler(0,0, transform.rotation.eulerAngles.z + angle);
        cargo.rb.SetRotation(angle);
        cargo.rb.MovePosition(transform.localToWorldMatrix.MultiplyPoint3x4(new Vector2(x, y)));*/
        
        outsideConnectors.Remove(shipConnector);
        foreach (var cargoCon in cargo.connectors) {
            cargoCon.gameObject.layer = LayerMask.NameToLayer("Ship");
            if (cargoCon != cargoConnector) {
                outsideConnectors.Add(cargoCon);
            }
        }
       
        var shipRB = shipConnector.GetComponentInParent<Rigidbody2D>();
        var joint = cargo.AddComponent<HingeJoint2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = shipRB;
        joint.anchor = cargoConnector.transform.localPosition;
        joint.connectedAnchor = shipConnector.transform.localPosition;
        float conZ = shipConnector.transform.localRotation.z;
        joint.limits = new JointAngleLimits2D() {
            min = -2f+conZ, max = 2f+conZ
        };
        joint.useLimits = true;

        connectedCargo.Add(cargo);
        gameState.cameraController.AddFollowTarget(cargo.gameObject);
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
        cargo.gameObject.layer = LayerMask.NameToLayer("Cargo");
        cargo.cargoState = Cargo.CargoState.Free;
        Destroy(cargo.GetComponent<HingeJoint2D>());
        foreach (var cargoConnector in cargo.connectors) {
            cargoConnector.connectorState = Connector.ConnectorState.Cargo;
            cargoConnector.gameObject.layer = LayerMask.NameToLayer("Cargo");
            outsideConnectors.Remove(cargoConnector);
        }
        stayConnector.connectorState = Connector.ConnectorState.AttachedOutside;
        outsideConnectors.Add(stayConnector);
        _insideConnectors [index] = (null, null);
        cargo.rb.AddForce((cargo.transform.position - stayConnector.transform.position).normalized * 1f, ForceMode2D.Impulse);
        cargo.rb.AddTorque(Random.Range(-0.5f, 0.5f), ForceMode2D.Impulse);

        connectedCargo.Remove(cargo);
        gameState.cameraController.RemoveFollowTarget(cargo.gameObject);
    }

}
