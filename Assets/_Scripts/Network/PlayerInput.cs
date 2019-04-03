using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public NetworkManager networkManager;
    
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

                if (Physics.Raycast(ray, out hit, 100f, 1<<4)) // Layer 4 (objects)
                {
                    Debug.Log(42);
                }
                else if (Physics.Raycast(ray, out hit, 100f, 1)) // Layer 0 (ground)
                {
                    networkManager.SetDestination(hit.point);
                }
            }

            
        }
    }
}
