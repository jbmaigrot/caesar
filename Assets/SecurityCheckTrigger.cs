using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCheckTrigger : MonoBehaviour
{
#if SERVER
    private int nbrObstacle = -2;
    private ProgrammableObjectsData parent;

    private void Start()
    {
        parent = this.GetComponentInParent<ProgrammableObjectsData>();
    }

    private void OnTriggerEnter(Collider other)
    {
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
        nbrObstacle -= 1;
        


    }
#endif
}
