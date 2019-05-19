using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerSource : MonoBehaviour
{

    public AnimationCurve curve = new AnimationCurve();
#if SERVER
    public float startingTime = 0;
    public bool isActivated;
    public bool takenFrom = false;
    public bool doNotResetTakenFrom = false;
    public ServerCarrier carrier;
    private Server server;
    /*private List<ServerCarrier> carriers = new List<ServerCarrier>();
    private float totalChargeSpeed = 0; //sum of all requested charge */

    //Start
    private void Start()
    {
        startingTime = Time.time;
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
        isActivated = false;
    }

    // reset takenFrom (used for animation)
    private void LateUpdate()
    {
        if (doNotResetTakenFrom)
        {
            doNotResetTakenFrom = false;
        }
        else
        {
            takenFrom = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            if (Time.time - startingTime > 240)
            {
                foreach (Transform ryan in server.characters)
                {
                    if (Vector3.Distance(ryan.position, this.transform.position) < 35)
                    {
                        ryan.GetComponent<ServerCharacter>().isAttractedByData = 0;
                    }
                }

                isActivated = false;
                carrier.charge = 0;
                enabled = false;
            }
        }
        else
        {
            if (carrier.charge > 0)
            {
                isActivated = true;
                server.AddMessage("THE NEW DATA POOL HAS BEGUN TO FILL.", Vector3.zero);
                server.NewAnnoncement(2);
            }
            takenFrom = false;
        }
        

        //get charge
        carrier.charge = Mathf.Min(carrier.maxCharge, carrier.charge + curve.Evaluate(Time.time - startingTime) * Time.deltaTime);
        foreach (Transform ryan in server.characters)
        {
            if (ryan.GetComponent<ServerCarrier>().charge < ryan.GetComponent<ServerCarrier>().maxCharge && Vector3.Distance(ryan.position, this.transform.position) < this.GetComponent<ServerCarrier>().charge * 30 / this.GetComponent<ServerCarrier>().maxCharge && !server.players.Contains(ryan))
            {
                ryan.GetComponent<ServerCharacter>().isAttractedByData = 1;
                ryan.GetComponent<ServerCharacter>().attractByDataDestination = this.transform.position;

                if (!ryan.GetComponent<ServerCharacter>().isStunned)
                    ryan.GetComponent<ServerCarrier>().StartTaking(this.GetComponent<ServerCarrier>());
            }
        }


        /* DISABLED: split charge equally
        //give charge
        totalChargeSpeed = 0;
        foreach (ServerCarrier james in carriers)
        {
            if (james.charging && (james.charge < james.maxCharge))
                totalChargeSpeed += james.chargeSpeed;
        }

        if (totalChargeSpeed * Time.deltaTime < charge) //if there is enough energy
        {
            foreach (ServerCarrier james in carriers)
            {
                james.charge += james.chargeSpeed * Time.deltaTime;
            }
            charge -= totalChargeSpeed * Time.deltaTime;
        }
        else //if there is not enough energy, split "equally" (pro rata)
        {
            foreach (ServerCarrier james in carriers)
            {
                james.charge += james.chargeSpeed / totalChargeSpeed * charge;
            }
            charge = 0;
        }*/
    }


    /* DISABLED: split charge equally
    // Add character to list
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ServerCarrier>() && other.GetComponent<ServerCarrier>() != GetComponent<ServerCarrier>())
            carriers.Add(other.GetComponent<ServerCarrier>());
    }

    // Remove character from list
    private void OnTriggerExit(Collider other)
    {
        for(int i = 0; i < carriers.Count; i++)
        {
            if (carriers[i].GetComponent<Collider>() == other)
            {
                carriers.RemoveAt(i);
                i--;
            }
        }
    }*/
#endif
}
