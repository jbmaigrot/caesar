using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMessageReceiver : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void ChatInstruction(string instruction);
    
}

public interface ISelectObject : IEventSystemHandler
{
    void SelectedProgrammableObject(GameObject SelectedObject);
}

