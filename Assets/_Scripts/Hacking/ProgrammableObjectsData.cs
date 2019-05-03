using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class ProgrammableObjectsData : MonoBehaviour
{
#if SERVER
    /*Server. Seulement coté serveur*/
    private Server server;
    public NavMeshSurface NavMeshSurface;
    

    private const float ATTRACTRADIUS = 20.0f;
    private const float ATTRACTTIME = 10.0f;
    public bool isAttract;
    private float attracttimebeforeend;
    private float attracttimebeforeeffect;

    private const float POWERPUMPRADIUS = 25.0f;
    private ServerCarrier serverCarrier;

    public int charactersIndex = -1; //This index correspond to the index in the list of transform of characters the server stores. -1 in case it's not a character

    private float timeBeforeStunReload;
    private const float TIMEOFSTUNRELOAD = 20.0f;
    private const float STUNBOXRADIUS = 10.0f;
#endif

#if CLIENT
    /*Client. Seulement coté client*/
    public Client client;
    public HackInterface hackInterface;
    public bool isWaitingHack;
    private RosaceForHacking rosaceForHacking;
#endif

#if SERVER
    /*Variables contenant le graphe de comportement de l'objet*/
    public List<InOutVignette> inputCodes = new List<InOutVignette>();
    public List<InOutVignette> outputCodes = new List<InOutVignette>();
    public List<Arrow> graph = new List<Arrow>();

    public bool isLightOn = false;
    public bool isDoorOpen = false;

    public Transform BlueBatterie;
    public Transform RedBatterie;

    public bool sendingToBlue;
    public bool sendingToRed;
#endif


    /*Variable servant à initier le graphe de comportement et à définir les inputs et outputs autorisées*/
    public ProgrammableObjectsScriptable Initiator;

    public ProgrammableObjectsContainer objectsContainer;

    private bool isHackable = true;

    // Start
    void Start()
    {
        objectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();

#if SERVER
        NavMeshSurface = FindObjectOfType<NavMeshSurface>();
        server = FindObjectOfType<Server>();

        /*Initie le graphe de comportement*/
        ProgrammableObjectsScriptable InitiatorClone = Instantiate(Initiator);
        inputCodes = new List<InOutVignette>(InitiatorClone.inputCodes);
        outputCodes = new List<InOutVignette>(InitiatorClone.outputCodes);
        graph = new List<Arrow>(InitiatorClone.graph);

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
#endif
        isHackable = Initiator.isHackable;
#if CLIENT
        client = FindObjectOfType<Client>();
        hackInterface = FindObjectOfType<HackInterface>();
        isWaitingHack = false;
        rosaceForHacking = FindObjectOfType<RosaceForHacking>();
#endif

    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
#if CLIENT
    void OnMouseDown()
    {
        if (isHackable && (this.GetComponent<Collider>().ClosestPoint(client.characters[client.playerIndex].transform.position) - client.characters[client.playerIndex].transform.position).magnitude < 5 &&!hackInterface.GetComponent<CanvasGroup>().blocksRaycasts&& !Input.GetKey(KeyCode.LeftControl))
        {
            client.RequestHackState(objectsContainer.GetObjectIndexClient(this));
            hackInterface.ReadyToOpen();
            isWaitingHack = true;
            rosaceForHacking.GetComponent<Animator>().SetTrigger("Activate");
        }

    }

    private void OnMouseUp()
    {
        hackInterface.DoNotOpenActually();
        isWaitingHack = false;
        rosaceForHacking.GetComponent<Animator>().SetTrigger("Deactivate");
    }
#endif

#if SERVER
    public void OnTriggerEnter(Collider other)
    {
        OnInput("OnPress");
    }

    /*Quand le mot en parametre apparait dans le chat, active la vignette OnWord correspondant. Potentielement à adapter un petit peu pour le chat.*/
    public void ChatInstruction(string instruction)
    {
        OnInput("OnWord", instruction);
    }

    /*Quand la vignette input désignée en paramêtre est activé, active toute les fléches qui y sont relié*/
    public void OnInput(string codeinput, string parameter_string = "", int parameter_int = 0)
    {
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

        if (codeoutput == "OpenDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnOpen();
            //this.GetComponentInChildren<NavMeshObstacle>().carving = false;
            this.GetComponentInChildren<NavMeshObstacle>().enabled = false;
            isDoorOpen = true;
        }

        if (codeoutput == "CloseDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnClose();
            this.GetComponentInChildren<NavMeshObstacle>().enabled = true;
            isDoorOpen = false;
        }

        if (codeoutput == "SendMessage")
        {
            server.AddMessage(parameter_string, new Vector3(0, 0, 0)); //TO UPDATE: temporary position
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
                    if (((int)Vector3.Distance(server.characters[i].position, this.transform.position)) < STUNBOXRADIUS && i != charactersIndex)
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
                if (((int)Vector3.Distance(objectsContainer.objectListServer[i].transform.position, this.transform.position)) < POWERPUMPRADIUS && objectsContainer.objectListServer[i]!=this)
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
    }

#endif
    /*A chaque frame, le signal se déplace dans les flèches du graphe*/
    void Update()
    {
#if SERVER
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
#endif
#if CLIENT
        if (isWaitingHack)
        {
            /*this.GetComponentInChildren<MeshRenderer>().material.color = Color.green;*/
        }
        else
        {
            /*this.GetComponentInChildren<MeshRenderer>().material.color = Color.white;*/
        }
#endif
    }
#if SERVER
    void TheAttractFunction()
    {
        attracttimebeforeend -= Time.deltaTime;
        attracttimebeforeeffect -= Time.deltaTime;
        if (attracttimebeforeeffect <= 0.0f)
        {
            for (int i = 0; i < server.characters.Count; i++)
            {
                if (((int)Vector3.Distance(server.characters[i].position, this.transform.position)) < ATTRACTRADIUS && i != charactersIndex)
                {
                    server.characters[i].GetComponent<ServerCharacter>().isAttracted = true;
                    server.characters[i].GetComponent<ServerCharacter>().attractDestination = this.transform.position;
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
#endif
}

