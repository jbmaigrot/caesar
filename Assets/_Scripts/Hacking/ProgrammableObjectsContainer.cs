using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour
{
#if CLIENT
    public Client client;
    public List<ProgrammableObjectsData> objectListClient;
#endif

#if SERVER
    public List<ProgrammableObjectsData> objectListServer;
#endif

    public void Start()
    {
#if CLIENT
        client = FindObjectOfType<Client>();
#endif

        foreach(ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>())
        {
#if CLIENT
            ryan.client = client;
            objectListClient.Add(ryan);
#endif

#if SERVER
            objectListServer.Add(ryan);
#endif
        }
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
