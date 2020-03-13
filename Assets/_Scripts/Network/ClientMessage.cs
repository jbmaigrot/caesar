using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientMessage : MonoBehaviour
{
    public List<Vector3> sourcePosition = new List<Vector3>();
    public bool isPrivate;
    public bool isPriority;
    private bool isHighlighted;
    public string message;
    private Client client;
    private ProgrammableObjectsData player;
    private Text textBox;
    private CameraController cam;
    private Minimap minimap;
    private bool hasInit = false;
    private int numberOfMessage = 0;

    //Start
    public void Start()
	{

		if (!GameState.CLIENT) return; // replacement for preprocessor

        cam = Camera.main.GetComponent<CameraController>();
        minimap = FindObjectOfType<Minimap>();

        
    }

    //Click
    public void ShowMessageOnMap()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

        foreach(Vector3 ryan in sourcePosition)
        {
            minimap.ShowMessage(ryan);
        }

    }

    public void NouveauMessage()
    {
        if (!hasInit)
        {
            textBox = GetComponentInChildren<Text>();
            client = FindObjectOfType<Client>();
            player = client.characters[client.playerIndex].GetComponent<ProgrammableObjectsData>();
            hasInit = true;
        }
        
        if (isPrivate)
        {
            bool hasFoundArobase = false;
            string justOneWord = "";
            isHighlighted = false;
            for(int ryan =0; ryan < message.Length; ryan++)
            {
                int reynolds = ryan + 1;
                if (message[ryan] == '@')
                {
                    
                    while(reynolds<message.Length && message[reynolds]!=' ' && message[reynolds] != '\n' && message[reynolds] != '\r')
                    {
                        justOneWord += message[reynolds];
                        reynolds++;
                    }
                    
                }
                if(justOneWord == player.uniqueName)
                {
                    message = message.Insert(reynolds, "</b>");
                    message = message.Insert(ryan, "<b>");
                    hasFoundArobase = true;
                    isHighlighted = true;
                }
                justOneWord = "";
            }
            if (!hasFoundArobase)
            {
                for (int ryan = 0; ryan < message.Length; ryan++)
                {
                    int reynolds = ryan + 1;
                    if (message[ryan] == '@')
                    {

                        while (reynolds < message.Length && message[reynolds] != ' ' && message[reynolds] != '\n' && message[reynolds] != '\r')
                        {
                            justOneWord += message[reynolds];
                            reynolds++;
                        }

                    }
                    if (justOneWord == "me")
                    {
                        message = message.Insert(reynolds, "</b>");
                        message = message.Insert(ryan, "<b>");
                        hasFoundArobase = true;
                        isHighlighted = true;
                    }
                    justOneWord = "";
                }
            }
        }
        else
        {
            isHighlighted = isPriority;
        }
        if (isHighlighted)
        {
            textBox.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            textBox.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
        AfficheMessage();
    }

    public void AfficheMessage()
    {
        numberOfMessage++;
        if (numberOfMessage > 1)
        {
            textBox.text = message + "(" + numberOfMessage.ToString() + ")";
        }
        else
        {
            textBox.text = message;
        }
    }
	/*
    //Click
    public void MoveCamera()
    {
		if (!GameState.CLIENT) return; // replacement for preprocessor

        cam.cameraMode = 1;
        cam.transform.parent.position = sourcePosition;
    }*/
}
