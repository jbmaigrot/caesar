using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour
{
    bool isClosing=false;
    bool isOccupied=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnOpen()
    {
        this.GetComponent<BoxCollider>().isTrigger = true;
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnClose()
    {
        if (isOccupied)
        {
            isClosing = true;
        }
        else
        {
            this.GetComponent<BoxCollider>().isTrigger = false;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other = this.GetComponentInParent<ProgrammableObjectsData>().HackInterface.GetComponent<HackInterface>().bonhomme.GetComponent<Collider>())
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other = this.GetComponentInParent<ProgrammableObjectsData>().HackInterface.GetComponent<HackInterface>().bonhomme.GetComponent<Collider>())
        {
            isOccupied = false;
            if(isClosing)
            {
                this.GetComponent<BoxCollider>().isTrigger = false;
                this.GetComponent<MeshRenderer>().enabled = true;
                isClosing = false;
            }
            this.GetComponentInParent<ProgrammableObjectsData>().OnInput("OnPassInside");
        }
    }

    void OnMouseDown()
    {
        if((this.transform.position - this.GetComponentInParent<ProgrammableObjectsData>().HackInterface.GetComponent<HackInterface>().bonhomme.transform.position).magnitude < 1)
        {
            this.GetComponentInParent<ProgrammableObjectsData>().OnInput("OnInteract");
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
