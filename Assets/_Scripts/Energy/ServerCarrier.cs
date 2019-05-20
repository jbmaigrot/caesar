using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerCarrier : MonoBehaviour
{
#if CLIENT
    public float clientCharge = 0; //ratio between 0 and 1
    public Client client;
    //public GameObject scoreDisplay;
    public ProgrammableObjectsContainer programmableObjectsContainer;
    private HackInterface hackInterface;

    //to display charge
    public Vector2 pos = new Vector2(0, 0);
    public Vector2 size = new Vector2(0, 0);
    private float zoom = 1;
    private GUIStyle style = new GUIStyle();
    private Camera cam;
#endif
    public GameObject dataBar;
    public Image filled;
    public bool draw = false;
    public Image pastille;
    public Material pastilleMaterialBleu;
    public Material pastilleMaterialOrange;

    public float maxCharge;
    public float chargeSpeed;
#if SERVER
    private const float RELAYTRANSFERRATE = 2.0f;
    public float charge = 0;
    
    //public bool charging = false;
    public ServerCarrier givingTo = null;
    public ServerCarrier takingFrom = null;
    private bool isFull;
    private bool isEmpty;
    private ProgrammableObjectsData objectData;
#endif

    private void Start()
    {
#if CLIENT
        client = FindObjectOfType<Client>();
        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
        hackInterface = FindObjectOfType<HackInterface>();
        cam = Camera.main;
#endif
#if SERVER
        objectData = this.gameObject.GetComponent<ProgrammableObjectsData>();
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if CLIENT
        if (draw)
        {
            filled.fillAmount = clientCharge;
        }
#endif
#if SERVER
        if (charge >= maxCharge)
        {
            charge = maxCharge;
            if (!isFull)
            {
                objectData.OnInput("OnFull");
                isFull = true;
            }
            StopTaking();
        }
        else
        {
            isFull = false;
        }

        if (charge <= 0)
        {
            charge = 0;
            if (!isEmpty)
            {
                objectData.OnInput("OnEmpty");
                isEmpty = true;
            }
            StopGiving();
        }
        else
        {
            isEmpty = false;
        }

        if (givingTo != null && charge > 0) // The carrier is giving energy
        {
            if (givingTo.charge >= givingTo.maxCharge || Vector3.Distance(transform.position, givingTo.transform.position) > 15)
            {
                StopGiving();
            }
            else
            {
                float energyToTransfer = Mathf.Min(charge, chargeSpeed * Time.deltaTime);
                givingTo.charge += energyToTransfer;
                charge -= energyToTransfer;

                if (givingTo.GetComponent<ServerBattery>())
                {
                    givingTo.GetComponent<ServerBattery>().receiving = true;
                    givingTo.GetComponent<ServerBattery>().doNotResetReceiving = true;
                }
            }
        }

        if (takingFrom != null && charge < maxCharge) // The carrier is taking/stealing energy
        {
            if (takingFrom.charge <= 0 || Vector3.Distance(transform.position, takingFrom.transform.position) > 15)
            {
                StopTaking();
            }
            else
            {
                float energyToTransfer = Mathf.Min(takingFrom.charge, chargeSpeed * Time.deltaTime);
                takingFrom.charge -= energyToTransfer;
                charge += energyToTransfer;

                if (takingFrom.GetComponent<ServerSource>())
                {
                    takingFrom.GetComponent<ServerSource>().takenFrom = true;
                    takingFrom.GetComponent<ServerSource>().doNotResetTakenFrom = true;
                }
            }
        }

        if (objectData.sendingToBlueServer)
        {
            float energyToTransfer = Mathf.Min(charge, RELAYTRANSFERRATE * Time.deltaTime);
            charge -= energyToTransfer;
            objectData.BlueBatterie.gameObject.GetComponent<ServerCarrier>().charge += energyToTransfer;
        }
        if (objectData.sendingToRedServer)
        {
            float energyToTransfer = Mathf.Min(charge, RELAYTRANSFERRATE * Time.deltaTime);
            charge -= energyToTransfer;
            objectData.RedBatterie.gameObject.GetComponent<ServerCarrier>().charge += energyToTransfer;
        }
#endif
    }

#if CLIENT
    // Take and Give
    public void OnMouseOver()
    {
        if (!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts && !client.characters[client.playerIndex].isTacle)
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
            {
                if (GetComponent<ProgrammableObjectsData>() != null)
                {
                    client.StartTaking(GetComponent<ProgrammableObjectsData>().objectIndexClient);
                }

            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
            {
                if (GetComponent<ProgrammableObjectsData>() != null)
                {
                    client.StartGiving(GetComponent<ProgrammableObjectsData>().objectIndexClient);
                }
            }
    }
#endif

#if SERVER
    //functions
    public void StartTaking(ServerCarrier other)
    {
        if (other != this && Vector3.Distance(transform.position, other.transform.position) < 15)
        {
            takingFrom = other;
            //charging = true;
            StopGiving();
        }
    }

    public void StopTaking()
    {
        takingFrom = null;
        //charging = false;
    }

    public void StartGiving(ServerCarrier other)
    {
        if (other != this && Vector3.Distance(transform.position, other.transform.position) < 15 )
        {
            givingTo = other;
            StopTaking();
        }
    }

    public void StopGiving()
    {
        givingTo = null;
    }
#endif
}
