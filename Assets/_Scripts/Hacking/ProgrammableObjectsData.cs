﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgrammableObjectsData : MonoBehaviour
{
    /*Interface de Hack. Utilisé pour envoyer les infos quand l'objets est hacké. Seulement coté client.*/
    public GameObject HackInterface;

    /*Server. Seulement coté serveur*/
    public GameObject Server;

    /*Network manager. Seulement coté client*/
    public NetworkManager networkManager;

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

        foreach(InOutVignette ryan in Initiator.initialOutputActions)
        {
            OnOutput(ryan.code, ryan.parameter_string, ryan.parameter_int);
        }
    }

    /*Si l'objet est cliqué à distance suffisament courte, ouvre l'interface de hack. Cette fonction doit être adapté pour le réseau.*/
    void OnMouseDown()
    {
        //if((this.transform.position - HackInterface.GetComponent<HackInterface>().bonhomme.transform.position).magnitude < 3)
        if (true)
        {
            Debug.Log("requesting hack");
            networkManager.RequestHackState(transform.GetSiblingIndex());
            //ExecuteEvents.Execute<ISelectObject>(HackInterface, null, (x, y) => x.SelectedProgrammableObject(this.gameObject));
            //OnInput("OnHack");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == HackInterface.GetComponent<HackInterface>().bonhomme.GetComponent<Collider>())
        {
            OnInput("OnPress");
        }
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
    void OnOutput(string codeoutput, string parameter_string = "", int parameter_int = 0)
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
            ScriptChatBox.NewChatContent = parameter_string +"\n";
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
            foreach (Transform ryan in Server.GetComponent<Server>().characters)
            {
                if (((int) Vector3.Distance(ryan.position, this.transform.position)) < parameter_int)
                {
                    ryan.GetComponent<ServerCharacter>().getStun();
                }
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
                    graph[i].timeBeforeTransmit.RemoveAt(j);
                }
            }
        }
    }
}
