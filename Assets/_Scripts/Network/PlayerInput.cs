using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerInput : MonoBehaviour
{
    public Texture2D stdCursor;
    public Texture2D ctrlCursor;
    public Texture2D hackCursor;
#if CLIENT
    private Client client;
    private HackInterface hackinterface;
    public bool isMouseOverAnOutputTextButtonhackInterface;

    //Start
    private void Start()
    {
        client = FindObjectOfType<Client>();
        hackinterface = FindObjectOfType<HackInterface>();
#if NO_CUSTOM_MOUSE
        stdCursor = null;
        ctrlCursor = null;
        hackCursor = null;
#endif
    }

    //Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Cursor.SetCursor(ctrlCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(stdCursor, Vector2.zero, CursorMode.Auto);
        }

        if (!EventSystem.current.IsPointerOverGameObject()) //Checks if the mouse is not over any UI
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                

                if (Physics.Raycast(ray, out hit, 100f, 49184)) // Layers 5, 14 (hackable), 15
                {
                    Cursor.SetCursor(hackCursor, Vector2.zero, CursorMode.Auto);
                }
                else if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100f, 1)) // Layer 0 (ground)
                {
                    if (hackinterface.GetComponent<CanvasGroup>().blocksRaycasts)
                    {
                        hackinterface.OnClose();
                    }
                    client.SetDestination(hit.point);
                    StartCoroutine(ClickMaintenu()); //No need to check if button is still pressed as we are stopping coroutine once it's released
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && hackinterface.GetComponent<CanvasGroup>().blocksRaycasts && (HackInterface.SelectedInputButton != -1|| HackInterface.SelectedOutputButton != -1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                hackinterface.ClicCancel();
                HackInterface.SelectedInputButton = -1;
                HackInterface.SelectedOutputButton = -1;
            }
            else
            {
                
                if (!isMouseOverAnOutputTextButtonhackInterface)
                {
                    hackinterface.ClicCancel();
                    HackInterface.SelectedInputButton = -1;
                    HackInterface.SelectedOutputButton = -1;
                }
                

            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
        }
    }

    IEnumerator ClickMaintenu()
    {
        while (true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, 49184)) // Layer 5 14 et 15 (objects)
            {
                //Debug.Log(42);
            }
            else if (Physics.Raycast(ray, out hit, 100f, 1)) // Layer 0 (ground)
            {
                client.SetDestination(hit.point);
            }
            yield return null;//new WaitForSeconds(.2f);
        }
    }
#endif
    }