using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client_Character : MonoBehaviour
{
    public Vector3 speed = new Vector3(0, 0, 0);
    public int name;
    public NetworkManager networkManager;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + speed * Time.deltaTime;
    }

    void OnMouseOver()
    {
        Debug.Log("voiture");
        if (Input.GetMouseButtonDown(1))
        {
            networkManager.Tacle(name);
        }
    }
}
