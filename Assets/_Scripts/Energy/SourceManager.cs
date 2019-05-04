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
    private Server server;
#endif

    // Start is called before the first frame update
    void Start()
    {
#if SERVER
        startingTime = Time.time;
        timeBeforeSource = 0;
        Random.InitState(System.DateTime.Now.Second);
        server = FindObjectOfType<Server>();
#endif
    }

    // Update is called once per frame
    void Update()
    {
#if SERVER
        timeBeforeSource -= Time.deltaTime;
        if (timeBeforeSource <= 0)
        {
            server.AddMessage("A NEW DATA POOL HAS APPEARED", Vector3.zero);
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
    }
}
