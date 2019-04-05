using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour, IMessageReceiver
{
    private List<ProgrammableObjectsData> objectsData;

    private void Start()
    {
        objectsData = new List<ProgrammableObjectsData>(GetComponentsInChildren<ProgrammableObjectsData>());
    }

    public void ChatInstruction(string instruction)
    {
        for(int i = 0; i < objectsData.Count; i++)
        {
            objectsData[i].ChatInstruction(instruction);
        }
    }
}
