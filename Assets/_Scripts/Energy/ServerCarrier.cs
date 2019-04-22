using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCarrier : MonoBehaviour
{
#if CLIENT
    public float clientCharge = 0; //ratio between 0 and 1
    public Client client;
    public ProgrammableObjectsContainer programmableObjectsContainer;
#endif

#if SERVER
    public float charge = 0;
    public float maxCharge = 10;
    public float chargeSpeed = 1;
    public bool charging = false;
    public ServerCarrier givingTo = null;
    public ServerCarrier takingFrom = null;
#endif

    private void Start()
    {
#if CLIENT
        client = FindObjectOfType<Client>();
        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if CLIENT
        //Display charge
#endif

#if SERVER
        if (charge > maxCharge)
        {
            charge = maxCharge;
            StopTaking();
        }

        if (givingTo != null && charge > 0) // The carrier is giving energy
        {
            if (givingTo.charge >= givingTo.maxCharge || Vector3.Distance(transform.position, givingTo.transform.position) > 5)
            {
                StopGiving();
            }
            else
            {
                float energyToTransfer = Mathf.Min(charge, chargeSpeed * Time.deltaTime);
                givingTo.charge += energyToTransfer;
                charge -= energyToTransfer;
            }
        }

        if (takingFrom != null && charge < maxCharge) // The carrier is taking/stealing energy
        {
            if (takingFrom.charge <= 0 || Vector3.Distance(transform.position, takingFrom.transform.position) > 5)
            {
                StopTaking();
            }
            else
            {
                float energyToTransfer = Mathf.Min(takingFrom.charge, chargeSpeed * Time.deltaTime);
                takingFrom.charge -= energyToTransfer;
                charge += energyToTransfer;
            }
        }
#endif
    }

#if CLIENT
    // Take and Give
    public void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            if (GetComponent<ProgrammableObjectsData>() != null)
                Debug.Log(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
                client.StartTaking(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
        {
            if (GetComponent<ProgrammableObjectsData>() != null)
                client.StartGiving(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
        }
    }
#endif

#if SERVER
    //functions
    public void StartTaking(ServerCarrier other)
    {
        if (other != this/* && Vector3.Distance(transform.position, other.transform.position) < 5*/) //max distance
        {
            takingFrom = other;
            charging = true;
        }
    }

    public void StopTaking()
    {
        takingFrom = null;
        charging = false;
    }

    public void StartGiving(ServerCarrier other)
    {
        if (other != this/* && Vector3.Distance(transform.position, other.transform.position) < 5*/) //max distance
            givingTo = other;
    }
    
    public void StopGiving()
    {
        givingTo = null;
    }
#endif
}
