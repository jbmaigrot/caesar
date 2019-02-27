using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextButtonHackInterface : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
    public bool isInput;
    public int numero;
    private bool isAWorkingNode;
    private bool isVisible;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void OnClick(/*PointerEventData pointerEvent*/)
    {
        if (isInput)
        {
            HackInterface.SelectedInputButton = numero-1;
  
        }
        else if (HackInterface.SelectedInputButton > -1 && HackInterface.SelectedInputButton < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count && numero - 1 < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
        {

            arrow NewArrow = new arrow();
            NewArrow.input = HackInterface.SelectedInputButton;
            HackInterface.SelectedInputButton = -1;
            NewArrow.output = numero - 1;

            bool isItReallyNew = true;
            foreach (arrow a in HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph)
            {
                if (a.input == NewArrow.input && a.output == NewArrow.output) isItReallyNew = false;
            }
            if (isItReallyNew)
            {
                HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph.Add(NewArrow);
 
            }
        }
    }
    /*
    public void OnPointerUp(PointerEventData pointerEvent)
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (HackInterface.SelectedGameObject != null)
        {
            if (isInput)
            {
                if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count)
                {
                    isAWorkingNode = false;
                    if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count + 1)
                    {
                        isVisible = false;
                        this.GetComponent<CanvasGroup>().alpha = 0f;
                        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                    else
                    {
                        isVisible = true;
                        this.GetComponent<CanvasGroup>().alpha = 1f;
                        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                }
                else
                {
                    isAWorkingNode = true;
                    isVisible = true;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    /*this.gameObject.GetComponent<Button>().onClick(){

                    }*/
                }
            }
            else
            {
                if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
                {
                    isAWorkingNode = false;
                    if (numero > HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count + 1)
                    {
                        isVisible = false;
                        this.GetComponent<CanvasGroup>().alpha = 0f;
                        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                    else
                    {
                        isVisible = true;
                        this.GetComponent<CanvasGroup>().alpha = 1f;
                        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    }
                }
                else
                {
                    isAWorkingNode = true;
                    isVisible = true;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }


        }
        
    }
}
