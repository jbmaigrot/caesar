using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class ProgrammableObjectsData : MonoBehaviour
{
#if SERVER
    /*Server. Seulement coté serveur*/
    public Server server;
    public NavMeshSurface NavMeshSurface;
    
    
    private const float ATTRACTTIME = 10.0f;
    public bool isAttract;
    private float attracttimebeforeend;
    private float attracttimebeforeeffect;
    
    private ServerCarrier serverCarrier;

    public int charactersIndex = -1; //This index correspond to the index in the list of transform of characters the server stores. -1 in case it's not a character

    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;

#endif

    public GameObject RedTrail;
    public GameObject BlueTrail;
#if CLIENT
    /*Client. Seulement coté client*/
    public Client client;
    public HackInterface hackInterface;
    public bool isWaitingHack;
    private RosaceForHacking rosaceForHacking;
    public bool sendingToBlueClient;
    public bool sendingToRedClient;
    public int objectIndexClient;

    public bool isGivingManually = false;
    public bool isTakingManually = false;

    private bool isGiving;
    private float timeIsGiving;
    private bool isTaking;
    private float timeIsTaking;

    private float timeInSoundCurve;
#endif

    public Transform BlueBatterie;
    public Transform RedBatterie;
#if SERVER
    /*Variables contenant le graphe de comportement de l'objet*/
    public List<InOutVignette> inputCodes = new List<InOutVignette>();
    public List<InOutVignette> outputCodes = new List<InOutVignette>();
    public List<Arrow> graph = new List<Arrow>();

    public bool isLightOn = false;
    public bool isDoorOpen = false;


    public int isBeingHackedServer;
    public bool sendingToBlueServer;
    public bool sendingToRedServer;

    public bool shouldBeSendToClientEveryFrame;
    public bool shouldBeSendToClientJustOnceMore;
#endif


    /*Variable servant à initier le graphe de comportement et à définir les inputs et outputs autorisées*/
    public ProgrammableObjectsScriptable Initiator;

    public ProgrammableObjectsContainer objectsContainer;

    private bool isHackable = true;

    public bool startIsOver = false;
    // Start
    void Start()
    {
        objectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();

#if SERVER
		if (GameState.SERVER) // replacement for preprocessor
		{
			NavMeshSurface = FindObjectOfType<NavMeshSurface>();
			server = FindObjectOfType<Server>();

			/*Initie le graphe de comportement*/
			ProgrammableObjectsScriptable InitiatorClone = Instantiate(Initiator);
			inputCodes = new List<InOutVignette>(InitiatorClone.inputCodes);
			outputCodes = new List<InOutVignette>(InitiatorClone.outputCodes);
			graph = new List<Arrow>(InitiatorClone.graph);
			shouldBeSendToClientEveryFrame = Initiator.shouldBeSendToClientEveryFrame;

			foreach (Arrow a in graph)
			{
				a.timeBeforeTransmit.Clear();
			}

			foreach (InOutVignette ryan in Initiator.initialOutputActions)
			{
				OnOutput(ryan.code, ryan.parameter_string, ryan.parameter_int);
			}

			isAttract = false;
			timeBeforeStunReload = 0;

			serverCarrier = this.GetComponent<ServerCarrier>();





			isBeingHackedServer = -1;
		}
#endif
        ServerBattery[] Batteries;
        Batteries = FindObjectsOfType<ServerBattery>();
        if (Batteries[0].team == 0)
        {
            RedBatterie = Batteries[0].transform;
            BlueBatterie = Batteries[1].transform;
        }
        else
        {
            RedBatterie = Batteries[1].transform;
            BlueBatterie = Batteries[0].transform;
        }
        isHackable = Initiator.isHackable;
#if CLIENT
		if (GameState.CLIENT) // replacement for preprocessor
		{
			client = FindObjectOfType<Client>();
			hackInterface = FindObjectOfType<HackInterface>();
			isWaitingHack = false;
			rosaceForHacking = FindObjectOfType<RosaceForHacking>();
			RedTrail.SetActive(true);
			if (RedBatterie == this.transform)
			{
				RedTrail.GetComponent<ParticleSystem>().Play();
			}
			else
			{
				RedTrail.GetComponent<ParticleSystem>().Stop();
			}
			BlueTrail.SetActive(true);
			if (BlueBatterie == this.transform)
			{
				BlueTrail.GetComponent<ParticleSystem>().Play();
			}
			else
			{
				BlueTrail.GetComponent<ParticleSystem>().Stop();
			}
		}
#endif
        startIsOver = true;
    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
#if CLIENT
    void OnMouseDown()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f, 16384)&& isHackable && (hit.collider.ClosestPoint(client.characters[client.playerIndex].transform.position) - client.characters[client.playerIndex].transform.position).magnitude < 15 &&!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts&& !Input.GetKey(KeyCode.LeftControl))
        {
            client.RequestHackState(objectIndexClient);
            hackInterface.ReadyToOpen();
            isWaitingHack = true;
            rosaceForHacking.GetComponent<Animator>().SetTrigger("Activate");
        }

    }

    private void OnMouseUp()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts)
        {
            client.CutSoundOfHackPlease();
            hackInterface.DoNotOpenActually(objectIndexClient);
            isWaitingHack = false;
            rosaceForHacking.GetComponent<Animator>().SetTrigger("Deactivate");
        }
        
    }
#endif

#if SERVER
    

    /*Quand le mot en parametre apparait dans le chat, active la vignette OnWord correspondant. Potentielement à adapter un petit peu pour le chat.*/
    public void ChatInstruction(string instruction)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		OnInput("OnWord", instruction);
    }

    /*Quand la vignette input désignée en paramêtre est activé, active toute les fléches qui y sont relié*/
    public void OnInput(string codeinput, string parameter_string = "", int parameter_int = 0)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		foreach (Arrow ryan in graph)
        {
            if (inputCodes.Count > ryan.input && inputCodes[ryan.input].code == codeinput && inputCodes[ryan.input].parameter_string == parameter_string && inputCodes[ryan.input].parameter_int == parameter_int)
            {
                ryan.timeBeforeTransmit.Add(ryan.transmitTime);
            }
        }
    }

    /*Quand la vignette output désigné est activé, fait l'effet correspondant*/
    public void OnOutput(string codeoutput, string parameter_string = "", int parameter_int = 0)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		if (codeoutput == "TurnOnLight")
        {
            GetComponentInChildren<Light>().enabled = true;
            
            isLightOn = true;
        }

        if (codeoutput == "TurnOffLight")
        {
            GetComponentInChildren<Light>().enabled = false;
            isLightOn = false;
        }
        if (codeoutput == "TurnOnHolo")
        {
            GetComponentInParent<ProgrammableObjectsData>().OnInput("OnTurnOn");
            isLightOn = true;
        }

        if (codeoutput == "TurnOffHolo")
        {
            GetComponentInParent<ProgrammableObjectsData>().OnInput("OnTurnOff");
            isLightOn = false;
        }
        if (codeoutput == "OpenDoor")
        {
            //this.GetComponentInChildren<DoorScript>().OnOpen();
            //this.GetComponentInChildren<NavMeshObstacle>().carving = false;
            this.GetComponentInChildren<DoorScript>().GetComponent<Collider>().enabled = false;
            this.GetComponentInChildren<NavMeshObstacle>().enabled = false;
            OnInput("OnOpen");
            isDoorOpen = true;
        }

        if (codeoutput == "CloseDoor")
        {
            //this.GetComponentInChildren<DoorScript>().OnClose();
            this.GetComponentInChildren<NavMeshObstacle>().enabled = true;
            this.GetComponentInChildren<DoorScript>().GetComponent<Collider>().enabled = true;
            OnInput("OnClose");
            isDoorOpen = false;
        }

        if (codeoutput == "SendMessage")
        {
            server.AddMessage(parameter_string, transform.position);
        }

        if (codeoutput == "TestInt")
        {
            Debug.Log(parameter_int.ToString());
        }

        if (codeoutput == "Ring")
        {
            //Debug.Log("ring-a-ling-a-ling, this is sound");
            //Désolé Sylvain :'(
        }

        if (codeoutput == "Stun")
        {
            if (timeBeforeStunReload <= 0)
            {
                timeBeforeStunReload = TIMEOFSTUNRELOAD;
                for (int i = 0; i < server.characters.Count; i++)
                {
                    if ((Vector3.Distance(server.characters[i].position, this.transform.position)) < InventoryConstants.StunboxRange && i != charactersIndex)
                    {
                        server.characters[i].GetComponent<ServerCharacter>().getStun();
                    }
                }
            }
        }

        if (codeoutput == "Attract")
        {
            isAttract = true;
            attracttimebeforeend = ATTRACTTIME;
            attracttimebeforeeffect = 0.0f;
        }

        if(codeoutput == "PowerPump")
        {
            ServerCarrier targetCarrier;
            for (int i = 0; i < objectsContainer.objectListServer.Count; i++)
            {
                if ((Vector3.Distance(objectsContainer.objectListServer[i].transform.position, this.transform.position)) < InventoryConstants.PowerpumpRange && objectsContainer.objectListServer[i]!=this)
                {
                    if (serverCarrier.charge < serverCarrier.maxCharge)
                    {
                        targetCarrier = objectsContainer.objectListServer[i].GetComponent<ServerCarrier>();
                        if (targetCarrier.charge >= serverCarrier.maxCharge - serverCarrier.charge)
                        {
                            targetCarrier.charge -= serverCarrier.maxCharge - serverCarrier.charge;
                            serverCarrier.charge = serverCarrier.maxCharge;
                        }
                        else
                        {
                            serverCarrier.charge += targetCarrier.charge;
                            targetCarrier.charge = 0;
                        }
                    }
                }
            }
        }

        if (codeoutput == "UseGadget")
        {
            switch (parameter_int)
            {
                case InventoryConstants.Attract:
                    OnOutput("Attract", parameter_string, parameter_int);
                    break;

                case InventoryConstants.Stunbox:
                    OnOutput("Stun", parameter_string, parameter_int);
                    break;

                case InventoryConstants.Powerpump:
                    OnOutput("PowerPump", parameter_string, parameter_int);
                    break;
            }
        }

        if(codeoutput == "GoRed")
        {
            if(this.GetComponent<ServerCharacter>().team != 1)
            {
                this.GetComponent<ServerCharacter>().hasAPriorityDestination = true;
                this.GetComponent<ServerCharacter>().priorityDestination = RedBatterie.position;
            }            
        }

        if(codeoutput == "GoBlue")
        {
            if (this.GetComponent<ServerCharacter>().team != 0)
            {
                this.GetComponent<ServerCharacter>().hasAPriorityDestination = true;
                this.GetComponent<ServerCharacter>().priorityDestination = BlueBatterie.position;
            }                
        }

        if(codeoutput == "IfSourceActiveWrite")
        {
            if(this.GetComponent<ServerSource>() && this.GetComponent<ServerSource>().enabled)
            {
                server.AddMessage(parameter_string, transform.position);
            }
        }
    }

#endif
    /*A chaque frame, le signal se déplace dans les flèches du graphe*/
    void Update()
    {
#if SERVER
		if (GameState.SERVER) // replacement for preprocessor
		{
			for (int i = 0; i < graph.Count; i++)
			{
				for (int j = 0; j < graph[i].timeBeforeTransmit.Count; j++)
				{
					graph[i].timeBeforeTransmit[j] -= Time.deltaTime;
					if (graph[i].timeBeforeTransmit[j] <= 0)
					{
						if (outputCodes.Count > graph[i].output)
						{
							OnOutput(outputCodes[graph[i].output].code, outputCodes[graph[i].output].parameter_string, outputCodes[graph[i].output].parameter_int);
						}
						graph[i].timeBeforeTransmit[j] = 5000;//.RemoveAt(j);
					}
				}
			}

			if (isAttract)
			{
				TheAttractFunction();
			}

			if (timeBeforeStunReload > 0)
			{
				timeBeforeStunReload -= Time.deltaTime;
			}
		}
#endif
#if CLIENT

		if (!GameState.CLIENT) return; // replacement for preprocessor
		{


			if (isGiving)
			{
				if (isTaking)
				{
					if (!isTakingManually)
					{
						StopTakingSound();
					}
					else
					{
						timeIsTaking += Time.deltaTime;
					}
				}
				else
				{
					if (isTakingManually)
					{
						StartTakingSound(!client.characters[client.playerIndex] == this.GetComponent<ClientCharacter>());
					}
				}

				if (!isGivingManually && !sendingToBlueClient && !sendingToRedClient)
				{
					StopGivingSound();
				}
				else
				{
					timeIsGiving += Time.deltaTime;
					timeInSoundCurve = (timeInSoundCurve + Time.deltaTime * objectsContainer.GivingDataSpeedCurve.Evaluate(timeIsGiving)) % 1f;
					RedTrail.GetComponent<AudioSource>().volume = objectsContainer.GivingDataVolumeWindowCurve.Evaluate(timeInSoundCurve);
					RedTrail.GetComponent<AudioSource>().pitch = objectsContainer.GivingDataPitchWindowCurve.Evaluate(timeInSoundCurve);
				}


			}
			else
			{
				if (isGivingManually || sendingToBlueClient || sendingToRedClient)
				{
					StartGivingSound(!client.characters[client.playerIndex] == this.GetComponent<ClientCharacter>());
				}

				if (isTaking)
				{
					if (!isTakingManually)
					{
						StopTakingSound();
					}
					else
					{
						timeIsTaking += Time.deltaTime;
						timeInSoundCurve = (timeInSoundCurve + Time.deltaTime * objectsContainer.TakingDataSpeedCurve.Evaluate(timeIsTaking)) % 1f;
						RedTrail.GetComponent<AudioSource>().volume = objectsContainer.TakingDataVolumeWindowCurve.Evaluate(timeInSoundCurve);
						RedTrail.GetComponent<AudioSource>().pitch = objectsContainer.TakingDataPitchWindowCurve.Evaluate(timeInSoundCurve);
					}
				}
				else
				{
					if (isTakingManually)
					{
						StartTakingSound(!client.characters[client.playerIndex] == this.GetComponent<ClientCharacter>());
					}
				}
			}
		}
#endif
    }
#if SERVER
    void TheAttractFunction()
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		attracttimebeforeend -= Time.deltaTime;
        attracttimebeforeeffect -= Time.deltaTime;
        if (attracttimebeforeeffect <= 0.0f)
        {
            for (int i = 0; i < server.characters.Count; i++)
            {
                if (((int)Vector3.Distance(server.characters[i].position, this.transform.position)) < InventoryConstants.AttractRange && i != charactersIndex && !this.GetComponent<ServerCharacter>().isStunned)
                {
                    server.characters[i].GetComponent<ServerCharacter>().isAttracted = true;
                    server.characters[i].GetComponent<ServerCharacter>().attractDestination = this.transform;
                    server.characters[i].GetComponent<ServerCharacter>().attracttimebeforeend = attracttimebeforeend;
                }
            }
            attracttimebeforeeffect = 0.1f;
        }
        if (attracttimebeforeend <= 0.0f)
        {
            isAttract = false;
        }
    }

    // This function add only new arrows to the arrow graph, to prevent suppressing previous information about transmit time
    public void UpdateArrowGraph(List<Arrow> newGraph)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		for (int i = 0; i < newGraph.Count; i++)
        {
            bool isNew = true;
            for (int j = 0; j < graph.Count; j++)
            {
                if (newGraph[i].input == graph[j].input && newGraph[i].output == graph[j].output)
                {
                    //isNew = false;
                    newGraph[i] = graph[j];
                    break;
                }
            }
        }
        graph = newGraph;
    }
#endif

#if CLIENT
    public void StopGivingSound()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		isGiving = false;
        timeInSoundCurve = 0;
        if (!isTaking)
        {
            RedTrail.GetComponent<AudioSource>().Stop();
        }
    }

    public void StartGivingSound(bool Spatialized)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		Debug.Log("HeyHeyLeSonDuGiving");
        isGiving = true;
        if (!RedTrail.GetComponent<AudioSource>().isPlaying)
        {
            RedTrail.GetComponent<AudioSource>().time = UnityEngine.Random.Range(0f, RedTrail.GetComponent<AudioSource>().clip.length);
            RedTrail.GetComponent<AudioSource>().Play();
            timeInSoundCurve = 0;
        }
        if (Spatialized)
        {
            RedTrail.GetComponent<AudioSource>().spatialBlend = 1.0f;
        }
        else
        {
            RedTrail.GetComponent<AudioSource>().spatialBlend = 0.0f;
        }
        timeIsGiving = 0f;
    }

    public void StopTakingSound()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		isTaking = false;
        timeInSoundCurve = 0;
        if (!isGiving)
        {
            RedTrail.GetComponent<AudioSource>().Stop();
        }
    }

    public void StartTakingSound(bool Spatialized)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		isTaking = true;
        if (!RedTrail.GetComponent<AudioSource>().isPlaying)
        {
            RedTrail.GetComponent<AudioSource>().time = UnityEngine.Random.Range(0f, RedTrail.GetComponent<AudioSource>().clip.length);
            RedTrail.GetComponent<AudioSource>().Play();
            timeInSoundCurve = 0;
        }

        if (Spatialized)
        {
            RedTrail.GetComponent<AudioSource>().spatialBlend = 1.0f;
        }
        else
        {
            RedTrail.GetComponent<AudioSource>().spatialBlend = 0.0f;
        }
        timeIsTaking = 0f;
    }

    public void SetSendingToBatterie(bool RedNotBlue, bool OnNotOff)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (RedNotBlue)
        {
            if (OnNotOff && !sendingToRedClient)
            {
                sendingToRedClient = true;

                RedTrail.GetComponent<ParticleSystem>().Play();
                    
                

            }

            if (!OnNotOff && sendingToRedClient)
            {
                sendingToRedClient = false;
                RedTrail.GetComponent<ParticleSystem>().Stop();
               
            }
        }
        else
        {
            if (OnNotOff && !sendingToBlueClient)
            {
                sendingToBlueClient = true;
                BlueTrail.GetComponent<ParticleSystem>().Play();
                
            }

            if (!OnNotOff && sendingToBlueClient)
            {
                sendingToBlueClient = false;

                BlueTrail.GetComponent<ParticleSystem>().Stop();
                
            }
        }
    }
#endif
}

