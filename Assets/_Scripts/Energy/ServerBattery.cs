using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        foreach (Transform ryan in server.characters)
        {
            if (ryan.GetComponent<ServerCarrier>().charge > 0 && (ryan.position - this.transform.position).magnitude < 30 && !server.players.Contains(ryan))
            {
                ryan.GetComponent<NavMeshAgent>().destination = this.transform.position;
                ryan.GetComponent<ServerCarrier>().StartGiving(this.GetComponent<ServerCarrier>());
            }
        }
    }

#endif
}
