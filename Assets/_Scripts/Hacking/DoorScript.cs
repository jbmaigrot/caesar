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
            this.GetComponent<MeshRenderer>().enabled = false;
            this.GetComponent<Collider>().enabled = false;
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
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<Collider>().enabled = true;
                
            }
        }
    }


#if CLIENT
    void OnMouseDown()
    {
        if (GetComponentInParent<ProgrammableObjectsData>().client.hackInterface.GetComponent<CanvasGroup>().blocksRaycasts)
        {
            GetComponentInParent<ProgrammableObjectsData>().client.hackInterface.OnClose();
        }
        else
        {
            if ((this.GetComponentInParent<Collider>().ClosestPoint(GetComponentInParent<ProgrammableObjectsData>().client.characters[GetComponentInParent<ProgrammableObjectsData>().client.playerIndex].transform.position) - GetComponentInParent<ProgrammableObjectsData>().client.characters[GetComponentInParent<ProgrammableObjectsData>().client.playerIndex].transform.position).magnitude < 15 && !Input.GetKey(KeyCode.LeftControl))
            {
                GetComponentInParent<ProgrammableObjectsData>().client.DoorInteract(GetComponentInParent<ProgrammableObjectsData>().objectsContainer.GetObjectIndexClient(GetComponentInParent<ProgrammableObjectsData>()));
            }
        }
       
    }
#endif
    
}
