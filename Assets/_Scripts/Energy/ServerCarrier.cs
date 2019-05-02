using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCarrier : MonoBehaviour
{
#if CLIENT
    public float clientCharge = 0; //ratio between 0 and 1
    public Client client;
    //public GameObject scoreDisplay;
    public ProgrammableObjectsContainer programmableObjectsContainer;
    private HackInterface hackInterface;

    //to display charge
    public Vector2 pos = new Vector2(0,0);
    public Vector2 size = new Vector2(0,0);
    private float zoom = 1;
    public Texture2D emptyTex;
    public Texture2D fullTex;
    public bool draw = false;
    private GUIStyle style = new GUIStyle();
    private Camera cam;
#endif

#if SERVER
    private const float RELAYTRANSFERRATE = 1.0f;
    public float charge = 0;
    public float maxCharge = 10;
    public float chargeSpeed = 1;
    //public bool charging = false;
    public ServerCarrier givingTo = null;
    public ServerCarrier takingFrom = null;
    private bool isFull;
    private bool isEmpty;
    private ProgrammableObjectsData objectData;
#endif

    private void Start()
    {
#if SERVER
        objectData = this.gameObject.GetComponent<ProgrammableObjectsData>();
#endif
#if CLIENT
        client = FindObjectOfType<Client>();
        programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
        hackInterface = FindObjectOfType<HackInterface>();
        cam = Camera.main;
#endif
    }

#if CLIENT
    // Display charge
    void OnGUI()
    {
        if (draw)
        {
            zoom = Mathf.Sqrt(cam.GetComponent<CameraController>().zoomFactor);
            size = new Vector2(0.05f * Screen.width / zoom, 0.01f * Screen.height / zoom);
            pos = new Vector2(cam.WorldToScreenPoint(transform.position).x - size.x /2, Screen.height - cam.WorldToScreenPoint(transform.position).y);
            //draw the background:
            GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
                GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex, style);

                //draw the filled-in part:
                GUI.BeginGroup(new Rect(0, 0, size.x * clientCharge, size.y));
                    GUI.Box(new Rect(0, 0, size.x, size.y), fullTex, style);

                GUI.EndGroup();
            GUI.EndGroup();
        }
    }
	/*
        //Display charge
        //todo changer le test degueu de la batterie + charger batterie par étape
        if (scoreDisplay != null)
        {
            scoreDisplay.GetComponentInChildren<Text>().text = clientCharge.ToString() + " %";
            if(clientCharge > 0.2f && clientCharge <= 0.4f)
            {
                //scoreDisplay
                
            }
        }*/
#endif

    // Update is called once per frame
    void Update()
    {
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
        }
        else
        {
            isEmpty = false;
        }

        if (givingTo != null && charge > 0) // The carrier is giving energy
        {
            if (givingTo.charge >= givingTo.maxCharge || Vector3.Distance(transform.position, givingTo.transform.position) > 5)
            {
                StopGiving();
            }
            else
            {
                float energyToTransfer = Mathf.Min(charge, chargeSpeed * Time.deltaTime);
                givingTo.charge += energyToTransfer;
                charge -= energyToTransfer;
            }
        }

        if (takingFrom != null && charge < maxCharge) // The carrier is taking/stealing energy
        {
            if (takingFrom.charge <= 0 || Vector3.Distance(transform.position, takingFrom.transform.position) > 5)
            {
                StopTaking();
            }
            else
            {
                float energyToTransfer = Mathf.Min(takingFrom.charge, chargeSpeed * Time.deltaTime);
                takingFrom.charge -= energyToTransfer;
                charge += energyToTransfer;
            }
        }

        if (objectData.sendingToBlue)
        {
            float energyToTransfer = Mathf.Min(charge, RELAYTRANSFERRATE * Time.deltaTime);
            charge -= energyToTransfer;
            objectData.BlueBatterie.gameObject.GetComponent<ServerCarrier>().charge += energyToTransfer;
        }
        if (objectData.sendingToRed)
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
        if(!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts)
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
        {
            if (GetComponent<ProgrammableObjectsData>() != null)
                Debug.Log(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
                client.StartTaking(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1))
        {
            if (GetComponent<ProgrammableObjectsData>() != null)
                client.StartGiving(programmableObjectsContainer.objectListClient.IndexOf(GetComponent<ProgrammableObjectsData>()));
        }
    }
#endif

#if SERVER
    //functions
    public void StartTaking(ServerCarrier other)
    {
        if (other != this/* && Vector3.Distance(transform.position, other.transform.position) < 5*/) //max distance
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
        if (other != this/* && Vector3.Distance(transform.position, other.transform.position) < 5*/) //max distance
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
