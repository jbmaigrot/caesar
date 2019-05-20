using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour
{
    private const float timeForDisappearing = 0.15f;
    private const float timeForAppearing = 0.15f;
    bool isOpen = false;
    public bool isClosing=false;
    public bool isOccupied=false;
    public AudioClip holoOn;
    public AudioClip holoOff;

    private float timeBeforeDisappearing;
    private float timeBeforeAppearing;

    public void Update()
    {
        if (isOpen&& GetComponent<MeshRenderer>().enabled)
        {
            timeBeforeDisappearing -= Time.deltaTime;
            if (timeBeforeDisappearing <= 0)
            {
                this.GetComponent<MeshRenderer>().enabled = false;
                this.GetComponent<Collider>().enabled = false;
            }
        }

        if (!isOpen && !GetComponent<MeshRenderer>().enabled)
        {
            timeBeforeAppearing -= Time.deltaTime;
            if (timeBeforeAppearing <= 0)
            {
                this.GetComponent<MeshRenderer>().enabled = true;
                this.GetComponent<Collider>().enabled = true;
            }
        }


    }

    public void OnOpen()
    {
        if (!isOpen)
        {
            isOpen = true;
            timeBeforeDisappearing = timeForDisappearing;
            this.GetComponent<AudioSource>().PlayOneShot(holoOff);
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
                timeBeforeAppearing = timeForAppearing;
                this.GetComponent<AudioSource>().PlayOneShot(holoOn);

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
