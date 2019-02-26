using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HackInterface : MonoBehaviour, ISelectObject
{
    
    static public GameObject SelectedGameObject;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<CanvasGroup>().alpha=0f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectedProgrammableObject(GameObject SelectedObject)
    {
        SelectedGameObject = SelectedObject;
        this.gameObject.GetComponent<CanvasGroup>().alpha =1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
