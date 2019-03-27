using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextButtonHackInterface : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
    public bool isInput;
    public int numero;
    private bool isFixed;
    private string code;

    private HackingAssetScriptable HackingAsset;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void GetHackingAsset(HackingAssetScriptable HackAss)
    {
        HackingAsset = HackAss;
        GetComponentInChildren<DropdownHackInterface>().GetHackingAsset(HackAss);
        GetComponentInChildren<InputFieldHackerInterface>().GetHackingAsset(HackAss);
    }

    public void OnClick(/*PointerEventData pointerEvent*/)
    {
        if (isInput)
        {
            HackInterface.SelectedInputButton = numero-1;
  
        }
        else if (HackInterface.SelectedInputButton > -1 && HackInterface.SelectedInputButton < HackInterface.inputCodes.Count && numero - 1 < HackInterface.outputCodes.Count)
        {

            Arrow NewArrow = new Arrow();
            NewArrow.input = HackInterface.SelectedInputButton;
            HackInterface.SelectedInputButton = -1;
            NewArrow.output = numero - 1;

            bool isItReallyNew = true;
            foreach (Arrow a in HackInterface.graph)
            {
                if (a.input == NewArrow.input && a.output == NewArrow.output) isItReallyNew = false;
            }
            if (isItReallyNew)
            {
                HackInterface.graph.Add(NewArrow);
                this.GetComponentInParent<HackInterface>().reloadArrow();
            }
        }
    }
    /*
    public void OnPointerUp(PointerEventData pointerEvent)
    {
        
    }*/

    // Update is called once per frame
    public void UpdateOptions(int inputCount, int outputCount)
    {
        int countOfTheTable;
        if (HackInterface.SelectedGameObject != null)
        {
            if (isInput)
            {
                countOfTheTable = inputCount;
            }
            else
            {
                countOfTheTable = outputCount;
            }
            if (numero > countOfTheTable)
            {
                if (numero > countOfTheTable + 1)
                {
                    /*Le bouton est invisible*/
                    this.GetComponent<CanvasGroup>().alpha = 0f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    GetComponentInChildren<DropdownHackInterface>().UpdateOff();
                    GetComponentInChildren<InputFieldHackerInterface>().UpdateOff();
                    isFixed = false;
                    code = "";
                }
                else
                {
                    /*Le bouton est visible mais blank, et sert à ajouter une nouvelle vignette*/
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    GetComponentInChildren<DropdownHackInterface>().UpdateBlank(isInput);
                    GetComponentInChildren<InputFieldHackerInterface>().UpdateOff();
                    isFixed = false;
                    code = "";
                }
            }
            else
            {
                /*Le bouton est visible et représente une vignette*/
                if (isInput)
                {
                    isFixed = HackInterface.inputCodes[numero - 1].is_fixed;
                    code = HackInterface.inputCodes[numero - 1].code;
                }
                else
                {
                    isFixed = HackInterface.outputCodes[numero - 1].is_fixed;
                    code = HackInterface.outputCodes[numero - 1].code;
                }
                this.GetComponent<CanvasGroup>().alpha = 1f;
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                GetComponentInChildren<DropdownHackInterface>().UpdateOn(isInput, isFixed, code);
                GetComponentInChildren<InputFieldHackerInterface>().UpdateOn(isInput, isFixed, code);
            }
        }
        
    }
}
