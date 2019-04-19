using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerScript : MonoBehaviour
{
    public GameObject DoorScript;
#if SERVER
    private int nbrObstacle = -2;
    private void OnTriggerEnter(Collider other)
    {
        nbrObstacle += 1;
        if (nbrObstacle > 0)
        {
            DoorScript.GetComponent<DoorScript>().isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nbrObstacle -= 1;
        if(nbrObstacle <= 0)
        {
            DoorScript.GetComponent<DoorScript>().isOccupied = false;
            if (DoorScript.GetComponent<DoorScript>().isClosing)
            {
                DoorScript.GetComponent<DoorScript>().isClosing = false;
                DoorScript.GetComponent<DoorScript>().OnClose();
            }
        }
        this.GetComponentInParent<ProgrammableObjectsData>().OnInput("OnPassInside");
        
    }
#endif
}
