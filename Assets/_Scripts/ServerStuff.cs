using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		if(!GameState.SERVER && GameState.CLIENT) // replacement for preprocessor
			gameObject.SetActive(false);
	}
}
