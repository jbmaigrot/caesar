using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;





public class ProgrammableObjectsData : MonoBehaviour, IMessageReceiver
{
    public HackingAssetScriptable HackingAsset;
    public GameObject HackInterface;

    public List<string> accessibleInputCode;
    public List<string> accessibleOutputCode;

    public List<InOutVignette> inputCodes=new List<InOutVignette>();
    public List<InOutVignette> outputCodes=new List<InOutVignette>();
    public List<Arrow> graph= new List<Arrow>();

    public ProgrammableObjectsScriptable Initiator;
    

    void Start()
    {
        accessibleInputCode = new List<string>(Initiator.accessibleInputCode);
        accessibleOutputCode = new List<string>(Initiator.accessibleOutputCode);
        inputCodes = new List<InOutVignette>(Initiator.inputCodes);
        outputCodes = new List<InOutVignette>(Initiator.outputCodes);
        graph = new List<Arrow>(Initiator.graph);

        foreach(Arrow a in graph)
        {
            a.timeBeforeTransmit.Clear();
        }
    }

    void OnMouseDown()
    {
        if((this.transform.position - HackInterface.GetComponent<HackInterface>().bonhomme.transform.position).magnitude < 3)
        {
            HackInterface.GetComponent<HackInterface>().bonhomme.SetActive(false);
            ExecuteEvents.Execute<ISelectObject>(HackInterface, null, (x, y) => x.SelectedProgrammableObject(this.gameObject));
            OnInput("OnHack");
        }
        
    }

    public void ChatInstruction(string instruction)
    {
        OnInput("OnWord", instruction);
    }

    public void OnInput(string codeinput, string parameter = "")
    {
        foreach(Arrow ryan in graph)
        {
            if(inputCodes.Count>ryan.input && inputCodes[ryan.input].code == codeinput)
            {
                foreach (InOutCode reynolds in HackingAsset.inputCodes)
                {
                    if (reynolds.code == codeinput && (!reynolds.parameter_string || inputCodes[ryan.input].parameter_string == parameter)) ryan.timeBeforeTransmit.Add(ryan.transmitTime);
                }
            }
        }
    }

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

        if(codeoutput == "SendMessage")
        {
            ScriptChatBox.NewChatContent = parameter +"\n";
        }
    }

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
                        string outputcode = outputCodes[graph[i].output].code;
                        foreach(InOutCode ryan in HackingAsset.outputCodes)
                        {
                            if (ryan.code == outputcode)
                            {
                                if (ryan.parameter_string)
                                {
                                    OnOutput(outputcode, outputCodes[graph[i].output].parameter_string);
                                }
                                else
                                {
                                    OnOutput(outputcode);
                                }
                            }
                        }
                    }
                    graph[i].timeBeforeTransmit.RemoveAt(j);
                }
            }
        }
    }
}
