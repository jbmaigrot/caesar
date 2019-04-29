using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerSource : MonoBehaviour
{
#if SERVER
    public AnimationCurve curve = new AnimationCurve();
    public float startingTime = 0;
    private ServerCarrier carrier;
    private Server server;
    /*private List<ServerCarrier> carriers = new List<ServerCarrier>();
    private float totalChargeSpeed = 0; //sum of all requested charge */

    //Start
    private void Start()
    {
        startingTime = Time.time;
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time - startingTime > 240)
            gameObject.SetActive(false);

        //get charge
        carrier.charge = Mathf.Min(carrier.maxCharge, carrier.charge + curve.Evaluate(Time.time - startingTime) * Time.deltaTime);
        foreach (Transform ryan in server.characters)
        {
            if (ryan.GetComponent<ServerCarrier>().charge < ryan.GetComponent<ServerCarrier>().maxCharge && Vector3.Distance(ryan.position, this.transform.position) < this.GetComponent<ServerCarrier>().charge * 30 / this.GetComponent<ServerCarrier>().maxCharge && !server.players.Contains(ryan))
            {
                ryan.GetComponent<ServerCharacter>().isAttractedByData = 1;
                ryan.GetComponent<ServerCharacter>().attractByDataDestination = this.transform.position;
                ryan.GetComponent<ServerCarrier>().StartTaking(this.GetComponent<ServerCarrier>());
            }
        }
        /* TEMP DISABLED
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


    /* TEMP DISABLED
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
