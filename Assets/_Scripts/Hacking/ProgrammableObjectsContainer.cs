using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour
{
    public ProgrammableObjectsScriptable PhilosopherChair;
#if CLIENT
    public Client client;
    public List<ProgrammableObjectsData> objectListClient;
#endif

#if SERVER
    public List<ProgrammableObjectsData> objectListServer;
    public List<ProgrammableObjectsData> chairListServer;
#endif

    public void Start()
    {
#if CLIENT
        client = FindObjectOfType<Client>();
#endif

        foreach(ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>(true))
        {
#if CLIENT
            ryan.client = client;
            objectListClient.Add(ryan);
            ryan.objectIndexClient = objectListClient.Count - 1;
#endif

#if SERVER
            objectListServer.Add(ryan);
            if (ryan.gameObject.name.Contains("Armchair"))
            {
                chairListServer.Add(ryan);
            }
#endif
        }
#if SERVER
        float bestScore=0f;
        ProgrammableObjectsData bestchair = new ProgrammableObjectsData();
        float randScore;
        foreach (ProgrammableObjectsData ryan in chairListServer)
        {
            randScore = Random.Range(0f, 1f);
            if (randScore > bestScore)
            {
                
                bestchair = ryan;
                bestScore = randScore;
            }
        }
        bestchair.Initiator = Instantiate(PhilosopherChair);
        bestchair.inputCodes = new List<InOutVignette>(bestchair.Initiator.inputCodes);
        bestchair.outputCodes = new List<InOutVignette>(bestchair.Initiator.outputCodes);
        bestchair.graph = new List<Arrow>(bestchair.Initiator.graph);
        foreach (Arrow a in bestchair.graph)
        {
            a.timeBeforeTransmit.Clear();
        }

        foreach (InOutVignette ryan in bestchair.Initiator.initialOutputActions)
        {
            bestchair.OnOutput(ryan.code, ryan.parameter_string, ryan.parameter_int);
        }
#endif
    }

#if SERVER
    public void ChatInstruction(string instruction)
    {
        for(int i = 0; i < objectListServer.Count; i++)
        {
            objectListServer[i].ChatInstruction(instruction);
        }
    }
#endif
#if CLIENT
    public int GetObjectIndexClient(ProgrammableObjectsData objectData)
    {
        Debug.Log(objectData);
        return objectListClient.IndexOf(objectData);

    }
#endif
#if SERVER
    public int GetObjectIndexServer(ProgrammableObjectsData objectData)
    {
        return objectListServer.IndexOf(objectData);
    }        
#endif
}
