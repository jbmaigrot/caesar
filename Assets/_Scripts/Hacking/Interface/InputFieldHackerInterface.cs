using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldHackerInterface : MonoBehaviour
{
    private GameObject SelectedGameObject;
    private string previousValue;
    private int previousValueDropdown;
    private bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        UpdateOff();
        SelectedGameObject = HackInterface.SelectedGameObject;
        previousValue = "";
    }


    

    void UpdateHackingGraph()
    {
        if (HackInterface.SelectedGameObject != null)
        {
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count)
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = this.GetComponent<InputField>().text;
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count)
                    HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = this.GetComponent<InputField>().text;
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
            for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes.Count; i++)
            {
                if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes[i].code ==code && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes[i].parameter_string) isOn=true;
            }
        }
        else
        {
            for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes.Count; i++)
            {
                if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes[i].code == code && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes[i].parameter_string) isOn = true;
            }
        }

        if (isOn)
        {
            if (isInput)
            {
                this.GetComponent<InputField>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
                previousValue = this.GetComponent<InputField>().text;
            }
            else
            {
                this.GetComponent<InputField>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
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
}
