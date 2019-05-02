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
    private ProgrammableObjectsData objectData;

    // Start
    private void Start()
    {
        carrier = GetComponent<ServerCarrier>();
        server = FindObjectOfType<Server>();
        objectData = GetComponent<ProgrammableObjectsData>();
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
            if (ryan.GetComponent<ServerCarrier>().charge > 0 && Vector3.Distance(ryan.position,this.transform.position) < 30 && !server.players.Contains(ryan))
            {
                ryan.GetComponent<ServerCharacter>().isAttractedByData = -1;
                ryan.GetComponent<ServerCharacter>().attractByDataDestination = this.transform.position;
                ryan.GetComponent<ServerCarrier>().StartGiving(this.GetComponent<ServerCarrier>());
            }
        }
    }

    public void RelayWin()
    {
        if (team == 0)
        {
            InOutVignette ryan=null;
            foreach (InOutVignette gosling in objectData.outputCodes)
            {
                if (gosling.code == "UseGadget" && gosling.parameter_int == InventoryConstants.BlueRelay)
                {
                    ryan = gosling;
                }
            }
            objectData.outputCodes.Remove(ryan);
            float toTransfer = Mathf.Min(objectData.BlueBatterie.GetComponent<ServerCarrier>().charge,carrier.maxCharge*0.1f);
            objectData.BlueBatterie.GetComponent<ServerCarrier>().charge -= toTransfer;
            InOutVignette reynolds = new InOutVignette();
            reynolds.code = "UseGadget";
            reynolds.parameter_int = InventoryConstants.BlueRelay;
            reynolds.is_fixed = true;
            objectData.BlueBatterie.GetComponent<ProgrammableObjectsData>().outputCodes.Add(reynolds);
            carrier.charge += toTransfer;
        }
        if (team == 1)
        {

            InOutVignette ryan = null;
            foreach (InOutVignette reynolds in objectData.outputCodes)
            {
                if (reynolds.code == "UseGadget" && reynolds.parameter_int == InventoryConstants.OrangeRelay)
                {
                    ryan = reynolds;
                }
            }
            objectData.outputCodes.Remove(ryan);
            float toTransfer = Mathf.Min(objectData.RedBatterie.GetComponent<ServerCarrier>().charge, carrier.maxCharge * 0.1f);
            objectData.RedBatterie.GetComponent<ServerCarrier>().charge -= toTransfer;
            InOutVignette gosling = new InOutVignette();
            gosling.code = "UseGadget";
            gosling.parameter_int = InventoryConstants.OrangeRelay;
            gosling.is_fixed = true;
            objectData.RedBatterie.GetComponent<ProgrammableObjectsData>().outputCodes.Add(gosling);
            carrier.charge += toTransfer;
        }
    }
#endif
}
