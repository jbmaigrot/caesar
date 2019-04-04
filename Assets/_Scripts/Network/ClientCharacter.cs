using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCharacter : MonoBehaviour
{
    public Vector3 speed = new Vector3(0, 0, 0);
    public int number;
    public NetworkManager networkManager;

    // Update is called once per frame
    void Update()
    {
       
        transform.position = transform.position + speed * Time.deltaTime;
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            networkManager.Tacle(number);
        }
    }
}
