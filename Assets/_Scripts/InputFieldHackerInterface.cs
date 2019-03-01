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
    // Start is called before the first frame update
    void Start()
    {
        UpdateOptions();
        SelectedGameObject = HackInterface.SelectedGameObject;
        previousValue = "";
        previousValueDropdown = 0;
    }


    void UpdateOptions()
    {
        if (HackInterface.SelectedGameObject != null)
        {
            string DropdownCode;
            int indiceHackingAsset = 0;
            if (this.transform.parent.GetComponentInChildren<Dropdown>().value == 0)
            {
                DropdownCode = "";
                this.GetComponent<CanvasGroup>().alpha = 0f;
                this.GetComponent<CanvasGroup>().interactable = false;
                this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    DropdownCode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode[this.transform.parent.GetComponentInChildren<Dropdown>().value - 1];
                }
                else
                {
                    DropdownCode = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode[this.transform.parent.GetComponentInChildren<Dropdown>().value - 1];
                }
            }

            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes.Count; i++)
                {
                    if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes[i].inputCode == DropdownCode) indiceHackingAsset = i;
                }
            }
            else
            {
                for (int i = 0; i < HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes.Count; i++)
                {
                    if (HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes[i].outputCode == DropdownCode) indiceHackingAsset = i;
                }
            }


            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes.Count && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.inputCodes[indiceHackingAsset].parameter_string)
                {
                    this.GetComponent<InputField>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().interactable = true;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                else
                {
                    this.GetComponent<InputField>().text = "";
                    this.GetComponent<CanvasGroup>().alpha = 0f;
                    this.GetComponent<CanvasGroup>().interactable = false;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes.Count && HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().HackingAsset.outputCodes[indiceHackingAsset].parameter_string)
                {
                    this.GetComponent<InputField>().text = HackInterface.SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string;
                    this.GetComponent<CanvasGroup>().alpha = 1f;
                    this.GetComponent<CanvasGroup>().interactable = true;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                else
                {
                    this.GetComponent<InputField>().text = "";
                    this.GetComponent<CanvasGroup>().alpha = 0f;
                    this.GetComponent<CanvasGroup>().interactable = false;
                    this.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
        }
            
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectedGameObject != HackInterface.SelectedGameObject || previousValueDropdown!=this.transform.parent.GetComponentInChildren<Dropdown>().value)
        {
            UpdateOptions();
            SelectedGameObject = HackInterface.SelectedGameObject;
            previousValue = this.GetComponent<InputField>().text;
            previousValueDropdown = this.transform.parent.GetComponentInChildren<Dropdown>().value;
        }

        if (this.GetComponent<InputField>().text != previousValue)
        {

            UpdateHackingGraph();

        }
    }
    
}
