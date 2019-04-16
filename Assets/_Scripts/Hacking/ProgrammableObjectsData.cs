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
    private const float STUNBOXRADIUS = 10.0f;
    private const float ATTRACTRADIUS = 20.0f;
    private const float ATTRACTTIME = 10.0f;
    private bool isAttract;
    private float attracttimebeforeend;
    private float attracttimebeforeeffect;

#endif

#if CLIENT
    /*Network manager. Seulement coté client*/
    public Client client;
#endif

#if SERVER
    /*Variables contenant le graphe de comportement de l'objet*/
    public List<InOutVignette> inputCodes=new List<InOutVignette>();
    public List<InOutVignette> outputCodes=new List<InOutVignette>();
    public List<Arrow> graph= new List<Arrow>();

    public bool isLightOn = false;
    public bool isDoorOpen = false;
#endif

    /*Variable servant à initier le graphe de comportement et à définir les inputs et outputs autorisées*/
    public ProgrammableObjectsScriptable Initiator;

    private ProgrammableObjectsContainer objectsContainer;

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
        
        foreach(Arrow a in graph)
        {
            a.timeBeforeTransmit.Clear();
        }

        foreach(InOutVignette ryan in Initiator.initialOutputActions)
        {
            OnOutput(ryan.code, ryan.parameter_string, ryan.parameter_int);
        }

        isAttract = false;
#endif
#if CLIENT
        client = FindObjectOfType<Client>();
#endif

    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
#if CLIENT
    void OnMouseDown()
    {
        //if((this.transform.position - HackInterface.GetComponent<HackInterface>().bonhomme.transform.position).magnitude < 3)
        if (true)
        {
            client.RequestHackState(objectsContainer.GetObjectIndexClient(this));
            
        }
        
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
        foreach(Arrow ryan in graph)
        {
            if(inputCodes.Count>ryan.input && inputCodes[ryan.input].code == codeinput && inputCodes[ryan.input].parameter_string == parameter_string && inputCodes[ryan.input].parameter_int == parameter_int)
            {
                ryan.timeBeforeTransmit.Add(ryan.transmitTime);
            }
        }
    }

    /*Quand la vignette output désigné est activé, fait l'effet correspondant*/
    public void OnOutput(string codeoutput, string parameter_string = "", int parameter_int = 0)
    {
        if(codeoutput == "TurnOnLight")
        {
            GetComponentInChildren<Light>().enabled = true;
            isLightOn = true;
        }

        if(codeoutput == "TurnOffLight")
        {
            GetComponentInChildren<Light>().enabled = false;
            isLightOn = false;
        }

        if(codeoutput == "OpenDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnOpen();
            NavMeshSurface.GetComponent<NavMeshSurfaceScript>().hasToBeRebake = true;
            isDoorOpen = true;
        }

        if(codeoutput == "CloseDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnClose();
            NavMeshSurface.GetComponent<NavMeshSurfaceScript>().hasToBeRebake = true;
            isDoorOpen = false;
        }

        if(codeoutput == "SendMessage")
        {
            server.AddMessage(parameter_string, new Vector3(0,0,0)); //TO UPDATE: temporary position
        }

        if(codeoutput == "TestInt")
        {
            Debug.Log(parameter_int.ToString());
        }

        if(codeoutput == "Ring")
        {
            Debug.Log("ring-a-ling-a-ling, this is sound");
        }

        if(codeoutput == "Stun")
        {
            foreach (Transform ryan in server.characters)
            {
                if (((int) Vector3.Distance(ryan.position, this.transform.position)) < STUNBOXRADIUS)
                {
                    ryan.GetComponent<ServerCharacter>().getStun();
                }
            }
        }

        if(codeoutput == "Attract")
        {
            isAttract = true;
            attracttimebeforeend = ATTRACTTIME;
            attracttimebeforeeffect = 0.0f;
        }

        if(codeoutput == "UseGadget")
        {
            switch (parameter_int)
            {
                case InventoryConstants.Attract:
                    OnOutput("Attract", parameter_string,parameter_int);
                    break;

                case InventoryConstants.Stunbox:
                    OnOutput("Stun", parameter_string, parameter_int);
                    break;

                case InventoryConstants.Powerpump:
                    OnOutput("PowerPump", parameter_string, parameter_int);
                    break;
            }
        }
    }


    /*A chaque frame, le signal se déplace dans les flèches du graphe*/
    void Update()
    {
        for(int i = 0; i < graph.Count; i++)
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
    }

    void TheAttractFunction()
    {
        attracttimebeforeend -= Time.deltaTime;
        attracttimebeforeeffect -= Time.deltaTime;
        if (attracttimebeforeeffect <= 0.0f)
        {
            foreach (Transform ryan in server.characters)
            {
                if (((int)Vector3.Distance(ryan.position, this.transform.position)) < ATTRACTRADIUS)
                {
                    ryan.GetComponent<NavMeshAgent>().ResetPath();
                    ryan.GetComponent<NavMeshAgent>().destination = this.transform.position;
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

