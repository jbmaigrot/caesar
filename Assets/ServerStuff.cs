using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if CLIENT
        this.enabled = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
