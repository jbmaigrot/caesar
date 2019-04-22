using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBattery : MonoBehaviour
{
#if SERVER
    public int team = 0;
    private ServerCarrier carrier;
    private Server server;

    // Start
    private void Start()
    {
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
    }

    // Update is called once per frame
    void Update()
    {
        if (carrier.charge >= carrier.maxCharge)
        {
            server.Win(team);
        }
    }

#endif
}
