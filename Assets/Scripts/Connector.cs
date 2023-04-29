using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour {
    public Collider2D connectorTrigger;
    public enum ConnectorState {
        Cargo, AttachedOutside, AttachedOutsidePulling, AttachedInside
    }
    public ConnectorState connectorState;

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
            case ConnectorState.AttachedOutsidePulling:
                Gizmos.color = Color.yellow;
                break;
        }
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
