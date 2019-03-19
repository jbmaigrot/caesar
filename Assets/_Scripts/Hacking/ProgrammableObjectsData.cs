using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Arrow
{
    public int input;
    public int output;
    public float transmitTime=0.2f;
    public List<float> timeBeforeTransmit=new List<float>();
};
public class InputHack
{
    public string inputcode;
    public int parameter_int;
    public string parameter_string;
    public bool is_fixed;
    public InputHack()
    {
        inputcode = "OnHack";
        parameter_int = 0;
        parameter_string = "";
    }
}

public class OutputHack
{
    public string outputcode;
    public int parameter_int;
    public string parameter_string;
    public bool is_fixed;

    public OutputHack()
    {
        outputcode = "TurnLightOff";
        parameter_int = 0;
        parameter_string = "";
    }
}

public class ProgrammableObjectsData : MonoBehaviour, IMessageReceiver
{
    public HackingAssetScriptable HackingAsset;
    public GameObject HackInterface;

    public List<string> accessibleInputCode;
    public List<string> accessibleOutputCode;

    public List<InputHack> inputCodes=new List<InputHack>();
    public List<OutputHack> outputCodes=new List<OutputHack>();
    public List<Arrow> graph= new List<Arrow>();

    public ProgrammableObjectsScriptable Initiator;
    

    void Start()
    {
        accessibleInputCode = Initiator.accessibleInputCode;
        accessibleOutputCode = Initiator.accessibleOutputCode;
        inputCodes = Initiator.inputCodes;
        outputCodes = Initiator.outputCodes;
        graph = Initiator.graph;

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
            if(inputCodes.Count>ryan.input && inputCodes[ryan.input].inputcode == codeinput)
            {
                foreach (InputCode reynolds in HackingAsset.inputCodes)
                {
                    if (reynolds.inputCode == codeinput && (!reynolds.parameter_string || inputCodes[ryan.input].parameter_string == parameter)) ryan.timeBeforeTransmit.Add(ryan.transmitTime);
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
                        string outputcode = outputCodes[graph[i].output].outputcode;
                        foreach(OutputCode ryan in HackingAsset.outputCodes)
                        {
                            if (ryan.outputCode == outputcode)
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
