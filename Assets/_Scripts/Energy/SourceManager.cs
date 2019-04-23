using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceManager : MonoBehaviour
{
    public List<ServerSource> sources;

#if SERVER
    public float timeBeforeNewSource = 240;
    private float startingTime = 0;
#endif

    // Start is called before the first frame update
    void Start()
    {
        //sources.AddRange(GetComponentsInChildren<ServerSource>());

#if SERVER
        startingTime = Time.time;
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if SERVER
        if (Time.time - startingTime < timeBeforeNewSource)
        {
            //sources[0].startingTime = Time.time;
            sources[0].gameObject.SetActive(true);
        }
        else if (Time.time - startingTime < 2*timeBeforeNewSource)
        {
            //sources[1].startingTime = Time.time;
            sources[1].gameObject.SetActive(true);
        }
        else if (Time.time - startingTime < 3 * timeBeforeNewSource)
        {
            //sources[2].startingTime = Time.time;
            sources[2].gameObject.SetActive(true);
        }
        else
        {
            startingTime = Time.time; //LOOP
        }
#endif

#if CLIENT
        //TO DO: set active sources when the serverside SourceManager decides to
        //
        //
#endif
    }
}
