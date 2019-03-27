using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldHackerInterface : MonoBehaviour
{
    private string previousValue;
    private bool isOn;

    private HackingAssetScriptable HackingAsset;
    // Start is called before the first frame update
    void Start()
    {
        UpdateOff();
        previousValue = "";
    }


    

    void UpdateHackingGraph()
    {
        if (HackInterface.SelectedGameObject != null)
        {
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.inputCodes.Count)
                    HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = this.GetComponent<InputField>().text;
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.outputCodes.Count)
                    HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = this.GetComponent<InputField>().text;
            }
        }
        
        
        
        previousValue = this.GetComponent<InputField>().text;
        this.GetComponentInParent<HackInterface>().reloadInterface();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.GetComponent<InputField>().text != previousValue)
        {

            UpdateHackingGraph();

        }
    }

    public void UpdateOff()
    {
        this.GetComponent<CanvasGroup>().alpha = 0f;
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        isOn = false;
        this.GetComponent<InputField>().text = "";
        previousValue = this.GetComponent<InputField>().text;
    }

    public void UpdateOn(bool isInput, bool isFixed, string code)
    {
        isOn = false;
        if (isInput)
        {
            for (int i = 0; i < HackingAsset.inputCodes.Count; i++) 
            {
                if (HackingAsset.inputCodes[i].code ==code && HackingAsset.inputCodes[i].parameter_string) isOn=true;
            }
        }
        else
        {
            for (int i = 0; i < HackingAsset.outputCodes.Count; i++)
            {
                if (HackingAsset.outputCodes[i].code == code && HackingAsset.outputCodes[i].parameter_string) isOn = true;
            }
        }

        if (isOn)
        {
            if (isInput)
            {
                this.GetComponent<InputField>().text = HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
                previousValue = this.GetComponent<InputField>().text;
            }
            else
            {
                this.GetComponent<InputField>().text = HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
                previousValue = this.GetComponent<InputField>().text;
            }
            this.GetComponent<CanvasGroup>().alpha = 1f;
            if (isFixed)
            {
                this.GetComponent<CanvasGroup>().interactable =false;
            }
            else
            {
                this.GetComponent<CanvasGroup>().interactable = true;
            }            
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else
        {
            this.GetComponent<InputField>().text = "";
            previousValue = this.GetComponent<InputField>().text;
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().interactable = false;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
    }

    public void GetHackingAsset(HackingAssetScriptable HackAss)
    {
        HackingAsset = HackAss;
    }
}
