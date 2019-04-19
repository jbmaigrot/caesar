using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if CLIENT
public class PlayerInput : MonoBehaviour
{
    private Client client;

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
    }

    //Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (! EventSystem.current.IsPointerOverGameObject()) //Checks if the mouse is not over any UI
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    if (Physics.Raycast(ray, out hit, 100f, 49184)) // Layer 5 14 et 15 (objects)
                    {
                        Debug.Log(42);
                    }
                    else if (Physics.Raycast(ray, out hit, 100f, 1)) // Layer 0 (ground)
                    {
                        client.SetDestination(hit.point);
                    }
                }
                /*else //Control is pressed -> energy-related actions
                {
                    if (Physics.Raycast(ray, out hit, 100f, 16384)) // Layer 14 (programmable objects)
                    {
                        if (hit.collider.GetComponent<ServerCarrier>())
                        {
                            client.StartTaking(hit.collider.GetComponent<ServerCarrier>());

                        }
                    }
                }*/
            }
        }
    }
}
#endif