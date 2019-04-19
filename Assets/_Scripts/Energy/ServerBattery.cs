using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBattery : ServerCarrier
{
#if SERVER
    // Update is called once per frame
    void Update()
    {
        if (charge >= maxCharge)
        {
            Time.timeScale = 0;
            Debug.Log("GG!");
        }
    }

#endif
}
