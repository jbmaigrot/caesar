using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour, IMessageReceiver
{
    public void ChatInstruction(string instruction)
    {
        foreach (ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>())
        {
            ryan.ChatInstruction(instruction);
        }
    }
}
