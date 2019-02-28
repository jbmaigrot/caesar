using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HackInterface : MonoBehaviour, ISelectObject
{
    static public int SelectedInputButton=-1;
    
    static public GameObject SelectedGameObject;
    public GameObject bonhomme;
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

    public void OnClose()
    {
        bonhomme.SetActive(true);
        SelectedInputButton = -1;
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SelectedProgrammableObject(GameObject SelectedObject)
    {
        SelectedGameObject = SelectedObject;
        this.gameObject.GetComponent<CanvasGroup>().alpha =1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
    }
}
