using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCarrier : MonoBehaviour
{
#if CLIENT
    public float clientCharge = 0;
    private Client client;
#endif

#if SERVER
    public float charge = 0;
    public float maxCharge = 10;
    public float chargeSpeed = 1;
    public bool charging = false;
    public ServerCarrier givingTo = null;
    public ServerCarrier takingFrom = null;
#endif

#if CLIENT
    private void Start()
    {
        client = FindObjectOfType<Client>();
    }
#endif

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
            if (Vector3.Distance(transform.position, givingTo.transform.position) > 5) //max distance
            {
                StopGiving();
            }
            else
            {
                givingTo.charge += chargeSpeed * Time.deltaTime;
                charge -= chargeSpeed * Time.deltaTime;
            }
        }

        if (takingFrom != null && charge < maxCharge) // The carrier is taking/stealing energy
        {
            if (Vector3.Distance(transform.position, givingTo.transform.position) > 5) //max distance
            {
                StopTaking();
            }
            else
            {
                takingFrom.charge -= chargeSpeed * Time.deltaTime;
                charge += chargeSpeed * Time.deltaTime;
            }
        }
#endif
    }

#if CLIENT
    // Take and Give
    public void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1) /*&& number != client.playerIndex*/) //Control+click is for energy-related actions
        {
            //TO DO
































        }
    }
#endif

#if SERVER
    //functions
    public void StartTaking(ServerCarrier other)
    {
        if (other != this && Vector3.Distance(transform.position, other.transform.position) < 5) //max distance
            takingFrom = other;
    }

    public void StopTaking()
    {
        takingFrom = null;
    }

    public void StartGiving(ServerCarrier other)
    {
        if (other != this && Vector3.Distance(transform.position, other.transform.position) < 5) //max distance
            givingTo = other;
    }
    
    public void StopGiving()
    {
        givingTo = null;
    }
#endif
}
