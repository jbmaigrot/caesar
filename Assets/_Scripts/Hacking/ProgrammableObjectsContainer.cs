using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableObjectsContainer : MonoBehaviour
{
    public ProgrammableObjectsScriptable PhilosopherChair;
    public AnimationCurve GivingDataVolumeWindowCurve;
    public AnimationCurve GivingDataPitchWindowCurve;
    public AnimationCurve GivingDataSpeedCurve;
    public AnimationCurve TakingDataVolumeWindowCurve;
    public AnimationCurve TakingDataPitchWindowCurve;
    public AnimationCurve TakingDataSpeedCurve;

    public Client client;
    public List<ProgrammableObjectsData> objectListClient;

    public List<ProgrammableObjectsData> objectListServer;
    public List<ProgrammableObjectsData> chairListServer;
    public Dictionary<string,ProgrammableObjectsData> objectNameServer = new Dictionary<string, ProgrammableObjectsData>();

    public void Start()
    {
		if (GameState.CLIENT) // replacement for preprocessor
		{
			client = FindObjectOfType<Client>();
		}

        foreach(ProgrammableObjectsData ryan in this.GetComponentsInChildren<ProgrammableObjectsData>(true))
        {
			if (GameState.CLIENT) // replacement for preprocessor
			{
				ryan.client = client;
				objectListClient.Add(ryan);
				ryan.objectIndexClient = objectListClient.Count - 1;
			}

			if (GameState.SERVER) // replacement for preprocessor
			{
				objectListServer.Add(ryan);
				if (ryan.gameObject.name.Contains("Armchair"))
				{
					chairListServer.Add(ryan);
				}
			}
        }
		if (GameState.SERVER) // replacement for preprocessor
		{
			float bestScore = 0f;
			ProgrammableObjectsData bestchair = new ProgrammableObjectsData();
			float randScore;
			foreach (ProgrammableObjectsData ryan in chairListServer)
			{
				randScore = Random.Range(0f, 1f);
				if (randScore > bestScore)
				{

					bestchair = ryan;
					bestScore = randScore;
				}
			}
			bestchair.Initiator = Instantiate(PhilosopherChair);
			bestchair.inputCodes = new List<InOutVignette>(bestchair.Initiator.inputCodes);
			bestchair.outputCodes = new List<InOutVignette>(bestchair.Initiator.outputCodes);
			bestchair.graph = new List<Arrow>(bestchair.Initiator.graph);
			foreach (Arrow a in bestchair.graph)
			{
				a.timeBeforeTransmit.Clear();
			}

			foreach (InOutVignette ryan in bestchair.Initiator.initialOutputActions)
			{
				bestchair.OnOutput(ryan.code, ryan.parameter_string, ryan.parameter_int);
			}
		}
    }
    
    public void ChatInstruction(string instruction,string messageCopy)
	{
		if (!GameState.SERVER) return; // replacement for preprocessor
		
		for (int i = 0; i < objectListServer.Count; i++)
        {
            objectListServer[i].ChatInstruction(instruction,messageCopy);
        }
    }

    public int GetObjectIndexClient(ProgrammableObjectsData objectData)
	{
		if (!GameState.CLIENT) return -1; // replacement for preprocessor

		Debug.Log(objectData);
        return objectListClient.IndexOf(objectData);

    }
    
    public int GetObjectIndexServer(ProgrammableObjectsData objectData)
	{
		if (!GameState.SERVER) return -1; // replacement for preprocessor

		return objectListServer.IndexOf(objectData);
    }
}
