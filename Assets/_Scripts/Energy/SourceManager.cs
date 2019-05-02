using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceManager : MonoBehaviour
{
    public List<ServerSource> sources;

#if SERVER
    public float timeBeforeNewSource = 180;
    public float startingTime = 0;
    public float timeBeforeSource;
#endif

    // Start is called before the first frame update
    void Start()
    {
        //sources.AddRange(GetComponentsInChildren<ServerSource>());

#if SERVER
        startingTime = Time.time;
        timeBeforeSource = 0;
        Random.InitState(System.DateTime.Now.Second);
#endif
        /*for (int i = 0; i < 3; i++)
        {
            sources[i].gameObject.SetActive(false);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
#if SERVER
        timeBeforeSource -= Time.deltaTime;
        if (timeBeforeSource <= 0)
        {
            int i;
            do
            {
                i = Random.Range(0, 3);
            } while (sources[i].gameObject.activeSelf);
            sources[i].gameObject.SetActive(true);
            sources[i].startingTime = Time.time;
            Debug.Log(i);
            timeBeforeSource = timeBeforeNewSource;
        }
        
#endif

#if CLIENT
        //TO DO: set active sources when the serverside SourceManager decides to
        //
        //
#endif
    }
}
