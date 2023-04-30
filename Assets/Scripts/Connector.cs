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
    public LineRenderer connectionLine;

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

    public void Update() {
        if (connectorState is ConnectorState.AttachedOutsideCouldPull or ConnectorState.AttachedOutsidePulling) {
            connectionLine.enabled = true;
            var myPos = transform.position;
            var otherPos = couldConnect.transform.position;
            connectionLine.SetPositions(new []{myPos, otherPos});
            if (connectorState == ConnectorState.AttachedOutsideCouldPull) {
                connectionLine.startColor = connectionLine.endColor = Color.yellow;
            } else {
                connectionLine.startColor = connectionLine.endColor = Color.white;
            }
        } else {
            connectionLine.enabled = false;
        }
    }
}
