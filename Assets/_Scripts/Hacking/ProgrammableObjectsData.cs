using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgrammableObjectsData : MonoBehaviour, IMessageReceiver
{
    /*Interface de Hack. Utilisé pour envoyer les infos quand l'objets est hacké.*/
    public GameObject HackInterface;

    /*Variables contenant le graphe de comportement de l'objet*/
    public List<InOutVignette> inputCodes=new List<InOutVignette>();
    public List<InOutVignette> outputCodes=new List<InOutVignette>();
    public List<Arrow> graph= new List<Arrow>();

    /*Variable servant à initié le graphe de comportement et à définir les input et output autorisées*/
    public ProgrammableObjectsScriptable Initiator;
    

    void Start()
    {
        /*Initie le graphe de comportement*/
        inputCodes = new List<InOutVignette>(Initiator.inputCodes);
        outputCodes = new List<InOutVignette>(Initiator.outputCodes);
        graph = new List<Arrow>(Initiator.graph);

        foreach(Arrow a in graph)
        {
            a.timeBeforeTransmit.Clear();
        }
    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
    void OnMouseDown()
    {
        if((this.transform.position - HackInterface.GetComponent<HackInterface>().bonhomme.transform.position).magnitude < 3)
        {
            HackInterface.GetComponent<HackInterface>().bonhomme.SetActive(false);
            ExecuteEvents.Execute<ISelectObject>(HackInterface, null, (x, y) => x.SelectedProgrammableObject(this.gameObject));
            OnInput("OnHack");
        }
        
    }

    /*Quand le mot en parametre apparait dans le chat, active la vignette OnWord correspondant. Potentielement à adapter un petit peu pour le chat.*/
    public void ChatInstruction(string instruction)
    {
        OnInput("OnWord", instruction);
    }

    /*Quand la vignette input désignée en paramêtre est activé, active toute les fléches qui y sont relié*/
    public void OnInput(string codeinput, string parameter = "")
    {
        foreach(Arrow ryan in graph)
        {
            if(inputCodes.Count>ryan.input && inputCodes[ryan.input].code == codeinput && inputCodes[ryan.input].parameter_string == parameter)
            {
                ryan.timeBeforeTransmit.Add(ryan.transmitTime);
            }
        }
    }

    /*Quand la vignette output désigné est activé, fait l'effet correspondant*/
    void OnOutput(string codeoutput, string parameter = "")
    {
        if(codeoutput == "TurnOnLight")
        {
            this.GetComponentInChildren<Light>().intensity = 100;
        }

        if(codeoutput == "TurnOffLight")
        {
            this.GetComponentInChildren<Light>().intensity = 0;
        }

        if(codeoutput == "OpenDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnOpen();

        }

        if(codeoutput == "CloseDoor")
        {
            this.GetComponentInChildren<DoorScript>().OnClose();
        }

        if(codeoutput == "SendMessage")/*A adapter pour le chat*/
        {
            ScriptChatBox.NewChatContent = parameter +"\n";
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
                        OnOutput(outputCodes[graph[i].output].code, outputCodes[graph[i].output].parameter_string);
                    }
                    graph[i].timeBeforeTransmit.RemoveAt(j);
                }
            }
        }
    }
}
