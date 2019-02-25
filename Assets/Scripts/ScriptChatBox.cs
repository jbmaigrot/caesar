using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ScriptChatBox : MonoBehaviour
{
    static public Hashtable WordsToLookFor = new Hashtable();
    public GameObject[] target;
    static public string NewChatContent;
    private string ChatContent;
    private string justOneWord;
    // Start is called before the first frame update
    void Start()
    {
        NewChatContent = "";
        ChatContent = "\n";
        WordsToLookFor.Add("light", 1);
        WordsToLookFor.Add("dark", 1);

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
                    if (WordsToLookFor.ContainsKey(justOneWord))
                    {
                        for(int i = 0;i< target.Length; i++)
                        {
                            ExecuteEvents.Execute<IMessageReceiver>(target[i], null, (x, y) => x.ChatInstruction(justOneWord));
                        }
                        
                    }
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
