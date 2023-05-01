using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour {
    public Collider2D connectorTrigger;
    public enum ConnectorState {
        Cargo, AttachedOutside, AttachedOutsideCouldPull, AttachedOutsidePulling, AttachedInside
    }
    public ConnectorState connectorState;

    public Connector couldConnect;
    public SpriteRenderer connectionLine, connectionInsideBtn;
    public Cargo cargo;

    void Start() {
        cargo = GetComponentInParent<Cargo>();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        switch(connectorState) {
            case ConnectorState.Cargo:
                Gizmos.color = Color.grey;
                break;
            case ConnectorState.AttachedInside:
                Gizmos.color = Color.blue;
                break;
            case ConnectorState.AttachedOutside:
                Gizmos.color = Color.green;
                break;
            case ConnectorState.AttachedOutsideCouldPull:
                Gizmos.color = Color.yellow;
                break;
            case ConnectorState.AttachedOutsidePulling:
                Gizmos.color = Color.white;
                break;
        }
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endif
    public void Update() {
        if (connectorState is ConnectorState.AttachedOutsideCouldPull or ConnectorState.AttachedOutsidePulling) {
            connectionInsideBtn.enabled = false;
            connectionLine.enabled = true;
            var myPos = transform.position;
            var otherPos = couldConnect.transform.position;
            connectionLine.transform.position = (myPos + otherPos)/2;
            connectionLine.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, myPos - otherPos));
            connectionLine.size = new Vector2(0.5f, Vector2.Distance(myPos, otherPos));
            if (connectorState == ConnectorState.AttachedOutsideCouldPull) {
                connectionLine.color = Color.yellow;
            } else {
                connectionLine.color = Color.white;
            }
        } else if (connectorState is ConnectorState.AttachedInside) {
            connectionInsideBtn.enabled = true;
        } else {
            connectionLine.enabled = false;
            connectionInsideBtn.enabled = false;
        }
    }
}
