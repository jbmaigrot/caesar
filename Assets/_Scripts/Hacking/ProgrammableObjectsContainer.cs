using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour
{
#if CLIENT
    public Client client;
#endif

    public List<ProgrammableObjectsData> objectList;

    public void Start()
    {
#if CLIENT
        client = FindObjectOfType<Client>();
#endif

        foreach(ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>())
        {
#if CLIENT
            ryan.client = client;
#endif
            objectList.Add(ryan);
        }
    }

#if SERVER
    public void ChatInstruction(string instruction)
    {
        for(int i = 0; i < objectList.Count; i++)
        {
            objectList[i].ChatInstruction(instruction);
        }
    }
#endif

    public int GetObjectIndex(ProgrammableObjectsData objectData)
    {
        return objectList.IndexOf(objectData);
    }
}
