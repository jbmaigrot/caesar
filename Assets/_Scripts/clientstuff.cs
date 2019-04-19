using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if !CLIENT
        gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
