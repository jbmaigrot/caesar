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


#if CLIENT
    void OnMouseDown()
    {
        if ((this.GetComponentInParent<Collider>().ClosestPoint(GetComponentInParent<ProgrammableObjectsData>().client.characters[GetComponentInParent<ProgrammableObjectsData>().client.playerIndex].transform.position) - GetComponentInParent<ProgrammableObjectsData>().client.characters[GetComponentInParent<ProgrammableObjectsData>().client.playerIndex].transform.position).magnitude < 2 && !Input.GetKey(KeyCode.LeftControl))
        {
            GetComponentInParent<ProgrammableObjectsData>().client.DoorInteract(GetComponentInParent<ProgrammableObjectsData>().objectsContainer.GetObjectIndexClient(GetComponentInParent<ProgrammableObjectsData>()));
        }
    }
#endif
    
}
