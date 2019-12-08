using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientStuff : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		// if launching directly the main scene in editor
		if (!GameState.CLIENT && !GameState.SERVER)
		{
			Debug.Log("To play in editor, set CLIENT and SERVER to true in GameState.cs");
		}


		if (!GameState.CLIENT) // replacement for preprocessor
			gameObject.SetActive(false);
	}
}
