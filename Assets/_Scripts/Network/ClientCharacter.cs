using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCharacter : MonoBehaviour
{
#if CLIENT
    public Vector3 speed = new Vector3(0, 0, 0);
    public int number;
    public bool isTacle;

    private Client client;
    private HackInterface hackinterface;

    private float floatingRange = 0.1f;
    private float floatingFreq = 0;
    private Transform mesh;
    private float startingY;

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();

        floatingFreq = Random.Range(0.2f, 0.3f);
        mesh = transform.Find("Mesh").transform;
        startingY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * Time.deltaTime;

        // Floating animation
        mesh.position = new Vector3(transform.position.x, startingY - floatingRange / 2 + floatingRange * (Mathf.Sin(Time.time * 2 * Mathf.PI * floatingFreq)), transform.position.z);
    }

    // Tacle
    public void OnMouseOver()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1) && number != client.playerIndex&&!hackinterface.GetComponent<CanvasGroup>().blocksRaycasts) //Control+click is for energy-related actions
        {
            client.Tacle(number);
        }
    }
#endif
}

