using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour, IMessageReceiver
{

    public Client client;
    public HackInterface hackInterface;

    public List<ProgrammableObjectsData> objectList;

    public void Start()
    {
        client = FindObjectOfType<Client>();
        hackInterface = FindObjectOfType<HackInterface>();

        foreach(ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>())
        {
            ryan.client = client;
            ryan.hackInterface = hackInterface;
            objectList.Add(ryan);
        }
    }

    public void ChatInstruction(string instruction)
    {
        foreach (ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>())
        {
            ryan.ChatInstruction(instruction);
        }
    }

    public int GetObjectIndex(ProgrammableObjectsData objectData)
    {
        return objectList.IndexOf(objectData);
    }
}
