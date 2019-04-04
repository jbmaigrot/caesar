using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour
{
    bool isOpen = false;
    public bool isClosing=false;
    public bool isOccupied=false;
    

    public void OnOpen()
    {
        if (!isOpen)
        {
            isOpen = true;
            this.transform.Translate(new Vector3(2,0,0));
        }
    }

    public void OnClose()
    {
        if (isOpen)
        {
            if (isOccupied)
            {
                isClosing = true;
            }
            else
            {
                isOpen = false;
                this.transform.Translate(new Vector3(-2, 0, 0));
            }
        }
    }

    

    void OnCollisionEnter()
    {
        this.GetComponentInParent<ProgrammableObjectsData>().OnInput("OnInteract");
    }
    
}
