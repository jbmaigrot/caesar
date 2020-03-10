using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosaceForHacking : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (!GameState.CLIENT) return; // replacement for preprocessor

		this.transform.position = Input.mousePosition;
    }
}

