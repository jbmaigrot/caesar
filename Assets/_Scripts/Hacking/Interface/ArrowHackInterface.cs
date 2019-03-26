using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowHackInterface : MonoBehaviour, IPointerDownHandler
{
    public int numero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnPointerDown(PointerEventData pointerEvent)
    {
        HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph.RemoveAt(numero);
        this.GetComponentInParent<HackInterface>().reloadArrow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateArrow()
    {
        if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph.Count > numero)
        {
            this.GetComponent<CanvasGroup>().alpha = 1f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.GetComponentInChildren<Text>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph[numero].input.ToString() + "   " + HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph[numero].output.ToString();
        }
        else
        {
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            this.GetComponentInChildren<Text>().text = "";
        }
    }
}
