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
}
