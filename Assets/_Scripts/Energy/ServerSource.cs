using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSource : MonoBehaviour
{
#if SERVER
    public float charge = 0;
    public float maxCharge = 200;
    public AnimationCurve curve = new AnimationCurve();

    private float startingTime = 0;
    private List<ServerCarrier> carriers = new List<ServerCarrier>();
    private float totalChargeSpeed = 0; //sum of all requested charge

    //Start
    private void Start()
    {
        startingTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //get charge
        charge = Mathf.Max(maxCharge, charge + curve.Evaluate(Time.time - startingTime) * Time.deltaTime);

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
        }
    }


    // Add character to list
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ServerCarrier>())
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
    }
#endif
}
