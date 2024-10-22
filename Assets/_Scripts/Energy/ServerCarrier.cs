﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerCarrier : MonoBehaviour
{
//Client only
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

//Client and server
    public GameObject dataBar;
    public Image filled;
    public bool draw = false;
    public Image pastille;
    public Material pastilleMaterialBleu;
    public Material pastilleMaterialOrange;

    public float maxCharge;
    public float chargeSpeed;

//Server only
    private const float RELAYTRANSFERRATE = 2.0f;
    public float charge = 0;
    
    //public bool charging = false;
    public ServerCarrier givingTo = null;
    public ServerCarrier takingFrom = null;
    private bool isFull;
    private bool isEmpty;
    private ProgrammableObjectsData objectData;


    private void Start()
    {
		if (GameState.CLIENT) // replacement for preprocessor
		{
			client = FindObjectOfType<Client>();
			programmableObjectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
			hackInterface = FindObjectOfType<HackInterface>();
			cam = Camera.main;
		}
		if (GameState.SERVER) // replacement for preprocessor
		{
			objectData = this.gameObject.GetComponent<ProgrammableObjectsData>();
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (GameState.CLIENT) // replacement for preprocessor
		{
			if (draw)
			{
				filled.fillAmount = clientCharge;
			}
		}
		if (GameState.SERVER) // replacement for preprocessor
		{
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
		}
    }


    // Take and Give
    public void OnMouseOver()
	{
		if (GameState.CLIENT) // replacement for preprocessor
		{
			if (!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts && !client.characters[client.playerIndex].isTacle)
			{
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
		}
    }
    
    //functions
    public void StartTaking(ServerCarrier other)
	{
		if (GameState.SERVER) // replacement for preprocessor
		{
			if (other != this && Vector3.Distance(transform.position, other.transform.position) < 15)
			{
				takingFrom = other;
				//charging = true;
				StopGiving();
			}
		}
    }

    public void StopTaking()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		takingFrom = null;
        //charging = false;
    }

    public void StartGiving(ServerCarrier other)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		if (other != this && Vector3.Distance(transform.position, other.transform.position) < 15 )
        {
            givingTo = other;
            StopTaking();
        }
    }

    public void StopGiving()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		givingTo = null;
    }
}
