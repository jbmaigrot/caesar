using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSurfaceScript : MonoBehaviour
{
    public bool hasToBeRebake;
    // Start is called before the first frame update
    void Start()
    {
  
    }



    // Update is called once per frame
    void LateUpdate()
    {
        if (hasToBeRebake)
        {
            hasToBeRebake = false;
            this.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
