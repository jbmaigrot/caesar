using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCheckTrigger : MonoBehaviour
{
    private int nbrObstacle = -2;
    private ProgrammableObjectsData parent;

    private void Start()
    {
        if (!GameState.SERVER) return; // replacement for preprocessor

        parent = this.GetComponentInParent<ProgrammableObjectsData>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameState.SERVER) return; // replacement for preprocessor

        nbrObstacle += 1;
        
        if (parent.startIsOver)
        {
            parent.OnInput("OnPress");
            if (other.transform == parent.server.PositionBlueRelay || other.transform == parent.server.PositionRedRelay)
            {
                parent.OnInput("OnRelayMet");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!GameState.SERVER) return; // replacement for preprocessor

        nbrObstacle -= 1;        
    }
}
