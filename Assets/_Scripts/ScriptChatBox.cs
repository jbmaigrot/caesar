using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ScriptChatBox : MonoBehaviour
{
    public GameObject target;
    static public string NewChatContent;
    private string ChatContent;
    private string justOneWord;
    // Start is called before the first frame update
    void Start()
    {
        NewChatContent = "";
        ChatContent = "\n";

    }

    // Update is called once per frame
    void Update()
    {
        if (NewChatContent != "")
        {
            justOneWord = "";
            foreach(char c in NewChatContent)
            {
                if ((c == '\n') || (c == '\r') || (c == ' '))
                {
                    ExecuteEvents.Execute<IMessageReceiver>(target, null, (x, y) => x.ChatInstruction(justOneWord));
                                         
                    
                    justOneWord = "";
                }
                else
                {
                    justOneWord += c;
                }
            }
            ChatContent += NewChatContent;
            NewChatContent = "";
        }
        this.GetComponent<Text>().text = ChatContent;
    }
}
