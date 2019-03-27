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



    /*Fonction pour modifier le parametre d'une vignette en fonction de l'input field*/
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
        
        /* On retient la nouvelle valeur de l'input field.*/
        previousValue = this.GetComponent<InputField>().text;
    }

    // Update is called once per frame
    void Update()
    {
        /*Si le contenu de l'input field change, on modifie le graphe*/
        if (this.GetComponent<InputField>().text != previousValue)
        {

            UpdateHackingGraph();

        }
    }

    /*Eteint l'inputfield car le bouton n'est pas une vignette.*/
    public void UpdateOff()
    {
        this.GetComponent<CanvasGroup>().alpha = 0f;
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        isOn = false;
        this.GetComponent<InputField>().text = "";
        previousValue = this.GetComponent<InputField>().text;
    }

    /*Ecris l'input field quand le bouton est une vignette.*/
    public void UpdateOn(bool isInput, bool isFixed, string code)
    {
        /*Regarde avec le code de la vignette si le code requiert un input field*/
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

        /*Si le code requiert un input field, on écrit le contenue tiré du graphe.*/
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

            /*Si la vignette est fixé dans le graphe, on empèche le joueur de changer le contenu de l'input field, sinon on le laisse faire.*/
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
        /*Si le code ne requiert pas d'input field, on l'éteint*/
        else
        {
            this.GetComponent<InputField>().text = "";
            previousValue = this.GetComponent<InputField>().text;
            if (isInput)
            {
                HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = "";
            }
            else
            {
                HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_string = "";
            }            
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().interactable = false;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        
    }

    /*Récupère HackingAsset du parent.*/
    public void GetHackingAsset(HackingAssetScriptable HackAss)
    {
        HackingAsset = HackAss;
    }
}
