using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnOpen()
    {
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnClose()
    {
        this.GetComponent<BoxCollider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
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
