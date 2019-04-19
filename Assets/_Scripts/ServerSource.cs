using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSource : MonoBehaviour
{/*
    public float charge = 0;
    public float maxCharge = 200;
    public float startingTime = 0;
    public AnimationCurve curve = new AnimationCurve();

    private List<ServerCharacter> charactersInside = new List<ServerCharacter>();
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
        foreach (ServerCharacter james in charactersInside)
        {
            if (james.charge < james.maxCharge)
                totalChargeSpeed += james.chargeSpeed;
        }

        if (totalChargeSpeed * Time.deltaTime < charge) //if there is enough energy
        {
            foreach (ServerCharacter james in charactersInside)
            {
                james.charge += james.chargeSpeed * Time.deltaTime;
            }
            charge -= totalChargeSpeed * Time.deltaTime;
        }
        else //if there is not enough energy, split "equally" (pro rata)
        {
            foreach (ServerCharacter james in charactersInside)
            {
                james.charge += james.chargeSpeed / totalChargeSpeed * charge;
            }
            charge = 0;
        }
    }


    // Add character
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ServerCharacter>())
            charactersInside.Add(other.GetComponent<ServerCharacter>());
    }

    // Remove character
    private void OnTriggerExit(Collider other)
    {
        for(int i = 0; i < charactersInside.Count; i++)
        {
            if (charactersInside[i].GetComponent<Collider>() == other)
            {
                charactersInside.RemoveAt(i);
                i--;
            }
        }
    }*/
}
