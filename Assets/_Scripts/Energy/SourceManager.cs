using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceManager : MonoBehaviour
{
    public List<ServerSource> sources;


    public float timeBeforeNewSource = 180;
    public float startingTime = 0;
    public float timeBeforeSource;
    private Server server;


    // Start is called before the first frame update
    void Start()
    {
		if (!GameState.SERVER) return; // replacement for preprocessor

		startingTime = Time.time;
        timeBeforeSource = 15.0f;
        Random.InitState(System.DateTime.Now.Second);
        server = FindObjectOfType<Server>();
        foreach(ServerSource ryan in sources)
        {
            ryan.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (!GameState.SERVER) return; // replacement for preprocessor

		timeBeforeSource -= Time.deltaTime;
        if (timeBeforeSource <= 0)
        {
            server.AddMessage("A NEW DATA POOL HAS APPEARED. @everyone", Vector3.zero,null);
            server.NewAnnoncement(1);
            int i;
            do
            {
                i = Random.Range(0, 3);
            } while (sources[i].enabled);
            sources[i].enabled = true;
            sources[i].startingTime = Time.time;

            Debug.Log(i);
            timeBeforeSource = timeBeforeNewSource;
        }
    }
}
