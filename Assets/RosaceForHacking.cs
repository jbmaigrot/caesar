using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if CLIENT
public class RosaceForHacking : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = Input.mousePosition;

    }
}
#endif
