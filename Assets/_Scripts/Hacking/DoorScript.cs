using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class DoorScript : MonoBehaviour
{
    private const float timeForDisappearing = 0.1f;
    private const float timeForAppearing = 0.1f;
    bool isOpen = false;
    public bool isClosing=false;
    public bool isOccupied=false;
    public AudioClip holoOn;
    public AudioClip holoOff;

    private float timeBeforeDisappearing;
    private float timeBeforeAppearing;
    private NavMeshObstacle navMeshObstacle;
    private MeshRenderer meshRenderer;
    private Collider collider;
    private ProgrammableObjectsData parent;
    private AudioSource audioSource;

    public void Start()
    {
        parent = GetComponentInParent<ProgrammableObjectsData>();
        navMeshObstacle = parent.GetComponentInChildren<NavMeshObstacle>();
        meshRenderer = this.GetComponent<MeshRenderer>();
        collider = this.GetComponent<Collider>();
        audioSource = this.GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isOpen&& GetComponent<MeshRenderer>().enabled)
        {
            timeBeforeDisappearing -= Time.deltaTime;
            if (timeBeforeDisappearing <= 0)
            {
                meshRenderer.enabled = false;

            }
        }

        if (!isOpen && !GetComponent<MeshRenderer>().enabled)
        {
            timeBeforeAppearing -= Time.deltaTime;
            navMeshObstacle.enabled = true;
            if (timeBeforeAppearing <= 0)
            {
                meshRenderer.enabled = true;

            }
        }


    }

    public void OnOpen()
    {
        if (!isOpen)
        {
            isOpen = true;
            timeBeforeDisappearing = timeForDisappearing;
            audioSource.PlayOneShot(holoOff);
        }
    }

    public void OnClose()
    {
        if (isOpen)
        {
            if (isOccupied)
            {
                isClosing = true;
            }
            else
            {
                isOpen = false;
                timeBeforeAppearing = timeForAppearing;
                audioSource.PlayOneShot(holoOn);

            }
        }
    }


#if CLIENT
    void OnMouseDown()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (parent.client.hackInterface.GetComponent<CanvasGroup>().blocksRaycasts)
        {
            parent.client.hackInterface.OnClose();
        }
        else
        {
            if ((this.GetComponentInParent<Collider>().ClosestPoint(parent.client.characters[parent.client.playerIndex].transform.position) - parent.client.characters[parent.client.playerIndex].transform.position).magnitude < 15 && !Input.GetKey(KeyCode.LeftControl))
            {
                parent.client.DoorInteract(parent.objectsContainer.GetObjectIndexClient(parent));
            }
        }
       
    }
#endif
    
}
