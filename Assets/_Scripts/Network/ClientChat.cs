using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClientChat : MonoBehaviour
{
    public RectTransform chatBox;
    public GameObject messagePrefab;
#if CLIENT
    private InputField inputField;
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponentInChildren<InputField>();
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            client.Message(inputField.text);
            inputField.text = "";
        }
    }

    //
    public void AddMessage(string message, Vector3 pos) 
    {
        chatBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chatBox.rect.height + 50);
        GameObject newMessage = Instantiate(messagePrefab);
        newMessage.transform.SetParent(chatBox);
        newMessage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        newMessage.GetComponent<RectTransform>().localPosition = new Vector3(10, 50, 0);
        newMessage.GetComponentInChildren<Text>().text = message;
        newMessage.GetComponent<ClientMessage>().sourcePosition = pos;
    }
#endif
}



