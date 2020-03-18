using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class ProgrammableObjectsData : MonoBehaviour
{

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



    public string uniqueName;
    public int uniqueNumber;

    public GameObject RedTrail;
    public GameObject BlueTrail;

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


    public Transform BlueBatterie;
    public Transform RedBatterie;

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



    /*Variable servant à initier le graphe de comportement et à définir les inputs et outputs autorisées*/
    public ProgrammableObjectsScriptable Initiator;

    public ProgrammableObjectsContainer objectsContainer;

    private bool isHackable = true;

    public bool startIsOver = false;
    // Start
    void Start()
    {
        objectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();


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

            GenerateUniqueName();



			isBeingHackedServer = -1;
		}

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

        startIsOver = true;
    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
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
    
    

    /*Quand le mot en parametre apparait dans le chat, active la vignette OnWord correspondant. Potentielement à adapter un petit peu pour le chat.*/
    public void ChatInstruction(string instruction, string messageCopy)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		OnInput("OnWord", instruction,0,messageCopy);
    }

    /*Quand la vignette input désignée en paramêtre est activé, active toute les fléches qui y sont relié*/
    public void OnInput(string codeinput, string parameter_string = "", int parameter_int = 0, string message = "")
	{
		if (!GameState.SERVER) return; // replacement for preprocessor

		foreach (Arrow ryan in graph)
        {
            if (inputCodes.Count > ryan.input && inputCodes[ryan.input].code == codeinput && inputCodes[ryan.input].parameter_string == parameter_string && inputCodes[ryan.input].parameter_int == parameter_int)
            {
                ryan.timeBeforeTransmit.Add(ryan.transmitTime);
                ryan.messageToTransmit.Add(message);
            }
        }
    }

    /*Quand la vignette output désigné est activé, fait l'effet correspondant*/
    public void OnOutput(string codeoutput, string parameter_string = "", int parameter_int = 0, string message ="")
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
            server.AddMessage(parameter_string, transform.position, this.GetComponent<ProgrammableObjectsData>());
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

        if(codeoutput == "GoTo")
        {
            if(objectsContainer.objectNameServer.ContainsKey(parameter_string))
            {
                this.GetComponent<ServerCharacter>().hasAPriorityDestination = true;
                this.GetComponent<ServerCharacter>().priorityDestination = objectsContainer.objectNameServer[parameter_string].GetComponent<Transform>().position;
            }
            else if(parameter_string == "me")
            {
                this.GetComponent<ServerCharacter>().hasAPriorityDestination = true;
                this.GetComponent<ServerCharacter>().priorityDestination = this.GetComponent<Transform>().position;
            }
        }

        if(codeoutput == "IfSourceActiveWrite")
        {
            if(this.GetComponent<ServerSource>() && this.GetComponent<ServerSource>().enabled)
            {
                server.AddMessage(parameter_string, transform.position,this.GetComponent<ProgrammableObjectsData>());
            }
        }

        if(codeoutput == "RepeatTo")
        {
            if (message != "" && parameter_string !="")
            {
                bool startOfWord = true;
                message += ' ';
                foreach (char c in parameter_string)
                {
                    if (startOfWord && (c != '@') && (c != '\n') && (c != '\r') && (c != ' '))
                    {
                        message += '@';
                    }
                    message += c;
                    startOfWord = (c == '\n') || (c == '\r') || (c == ' ');
                }

            }
            server.AddMessage(message, transform.position, this.GetComponent<ProgrammableObjectsData>());
        }
    }

    /*A chaque frame, le signal se déplace dans les flèches du graphe*/
    void Update()
    {

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
							OnOutput(outputCodes[graph[i].output].code, outputCodes[graph[i].output].parameter_string, outputCodes[graph[i].output].parameter_int, graph[i].messageToTransmit[j]);
						}
						graph[i].timeBeforeTransmit[j] = 50000;//.RemoveAt(j);
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
    }

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

    public void GenerateUniqueName()
    {
        if (!GameState.SERVER) return;

        bool namedOk = false;
        int nameNumber;
        string numberName;
        string name ="";

        while (!namedOk)
        {
            if(Initiator.baseName == "drone")
            {
                nameNumber = UnityEngine.Random.Range(0, 1000);
                if (nameNumber > 99)
                {
                    numberName = nameNumber.ToString();
                }
                else if (nameNumber > 9)
                {
                    numberName = string.Concat("0", nameNumber.ToString());
                }
                else
                {
                    numberName = string.Concat("00", nameNumber.ToString());
                }
            }
            else
            {
                nameNumber = UnityEngine.Random.Range(0, 100);
                if (nameNumber > 9)
                {
                    numberName = nameNumber.ToString();
                }
                else
                {
                    numberName = string.Concat("0", nameNumber.ToString());
                }
            }
            name = string.Concat(Initiator.baseName, numberName);
            if (!objectsContainer.objectNameServer.ContainsKey(name))
            {
                objectsContainer.objectNameServer.Add(name,this.GetComponent<ProgrammableObjectsData>());
                namedOk = true;
                uniqueName = name;
                uniqueNumber = nameNumber;
            }
        }
        if (server.players.Contains(this.GetComponent<Transform>()))
        {
            server.SendRegards(name, this.GetComponent<ServerCharacter>().team);
        }
    }

    public void SetUniqueName(int uniqueNumber)
    {
        if (!GameState.CLIENT) return;
        
        string numberName;
        string name;

        if (Initiator.baseName == "drone")
        {
            if (uniqueNumber > 99)
            {
                numberName = uniqueNumber.ToString();
            }
            else if (uniqueNumber > 9)
            {
                numberName = string.Concat("0", uniqueNumber.ToString());
            }
            else
            {
                numberName = string.Concat("00", uniqueNumber.ToString());
            }
        }
        else
        {
            if (uniqueNumber > 9)
            {
                numberName = uniqueNumber.ToString();
            }
            else
            {
                numberName = string.Concat("0", uniqueNumber.ToString());
            }
        }   
        uniqueName = string.Concat(Initiator.baseName, numberName);
    }

    public void BatteryNewPlayerInTeam(string name)
    {
        if (!GameState.SERVER) return;
        int inputVign = -1;
        string inputParam = string.Concat("@", uniqueName);
        for (int i =0; i< inputCodes.Count;i++) 
        {
            if (inputCodes[i].code == "OnWord" && inputCodes[i].parameter_string == inputParam)
            {
                inputVign = i;
            }
        }
        if (inputVign == -1)
        {
            InOutVignette reynolds = new InOutVignette();
            reynolds.code = "OnWord";
            reynolds.parameter_string = inputParam;
            inputVign = inputCodes.Count;
            inputCodes.Add(reynolds);
        }

        int outputVign = outputCodes.Count;
        InOutVignette ryan = new InOutVignette();
        ryan.code = "RepeatTo";
        ryan.parameter_string = name;
        outputCodes.Add(ryan);

        Arrow arrow = new Arrow();
        arrow.input = inputVign;
        arrow.output = outputVign;
        graph.Add(arrow);
    }
}
