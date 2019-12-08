using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClientChat : MonoBehaviour
{
    public RectTransform chatBox;
    public GameObject messagePrefab;
#if CLIENT
    public InputField inputField;
    private Client client;
    private AudioSource audioSource;
    private float saturation;

    // Start is called before the first frame update
    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		inputField = GetComponentInChildren<InputField>();
        client = FindObjectOfType<Client>();
        audioSource = GetComponent<AudioSource>();
        saturation = 0;
    }

    // Update is called once per frame
    void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (Input.GetKeyDown(KeyCode.Return) && inputField.text != "")
        {
            client.Message(inputField.text);
            inputField.text = "";
        }
        saturation -= Time.deltaTime;
        if (saturation < 0) saturation = 0;
    }

    //
    public void AddMessage(string message, Vector3 pos)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		GameObject newMessage = Instantiate(messagePrefab);
        Text newTextBox = newMessage.GetComponentInChildren<Text>();

        // From https://answers.unity.com/questions/921726/how-to-get-the-size-of-a-unityengineuitext-for-whi.html,
        // used to calculate the height of the message once integrated in the chat to avoid having to rely on VerticalLayoutGroup and running into performance issue.
        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = newTextBox.GetGenerationSettings(newTextBox.rectTransform.rect.size);
        float height = textGen.GetPreferredHeight(message, generationSettings);

        chatBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chatBox.rect.height + height);

        newMessage.transform.SetParent(chatBox);
        newMessage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        newMessage.GetComponent<RectTransform>().localPosition = new Vector3(10, 50, 0);
        newTextBox.text = message;
        newMessage.GetComponent<ClientMessage>().sourcePosition = pos;
        if(saturation < 3f)
        {
            saturation += 0.5f;
            if(saturation > 3f)
            {
                saturation = 5f;
            }
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        
    }
#endif
}



