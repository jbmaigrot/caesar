using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldHackerInterface : MonoBehaviour, IPointerDownHandler
{

    public Sprite Normal;
    public Sprite Attract;
    public Sprite Stun;
    public Sprite PowerPump;
    public Sprite RedRelay;
    public Sprite BlueRelay;

    public GameObject gadgetRange;

#if CLIENT
    private string previousValue;
    private bool isOnString;
    private bool isOnInt;

    private HackingAssetScriptable HackingAsset;
    private HackInterface hackinterface;
    private int numeroGadget = 0;
    private bool isPointerOver = false;


    void Start()
    {
        hackinterface = FindObjectOfType<HackInterface>();
    }

    /*Fonction pour modifier le parametre d'une vignette en fonction de l'input field*/
    void UpdateHackingGraph()
    {
        if (HackInterface.SelectedGameObject != null)
        {
            if (isOnString)
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
            }/*
            else if (isOnInt)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.inputCodes.Count)
                        HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int = int.Parse(this.GetComponent<InputField>().text);
                }
                else
                {
                    if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.outputCodes.Count)
                        HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int = int.Parse(this.GetComponent<InputField>().text);
                }
            }*/
            
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

        if (isPointerOver)
        {
            gadgetRange.transform.position = hackinterface.GetSelectedProgrammableObject().transform.position;
        }
    }

    /*Eteint l'inputfield car le bouton n'est pas une vignette.*/
    public void UpdateOff()
    {
        this.GetComponent<CanvasGroup>().alpha = 0f;
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;
        isOnString = false;
        isOnInt = false;
        this.GetComponent<InputField>().text = "";
        previousValue = this.GetComponent<InputField>().text;
    }

    /*Ecris l'input field quand le bouton est une vignette.*/
    public void UpdateOn(bool isInput, bool isFixed, string code)
    {
        /*Regarde avec le code de la vignette si le code requiert un input field*/
        isOnString = false;
        isOnInt = false;
        if (isInput)
        {
            for (int i = 0; i < HackingAsset.inputCodes.Count; i++) 
            {
                if (HackingAsset.inputCodes[i].code == code && HackingAsset.inputCodes[i].parameter_string) isOnString=true;
                if (HackingAsset.inputCodes[i].code == code && HackingAsset.inputCodes[i].parameter_int) isOnInt = true;
            }
        }
        else
        {
            for (int i = 0; i < HackingAsset.outputCodes.Count; i++)
            {
                if (HackingAsset.outputCodes[i].code == code && HackingAsset.outputCodes[i].parameter_string) isOnString = true;
                if (HackingAsset.outputCodes[i].code == code && HackingAsset.outputCodes[i].parameter_int) isOnInt = true;
            }
        }

        /*Si le code requiert un input field string, on écrit le contenue tiré du graphe.*/
        if (isOnString)
        {
            this.GetComponent<InputField>().enabled = true;
            this.GetComponent<SVGImage>().sprite = Normal;
            this.GetComponent<InputField>().contentType = InputField.ContentType.Standard;
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
        /*Si le code requiet un input field de type int, on écrit le contenu tiré du graphe*/
        else if (isOnInt)
        {
            this.GetComponent<InputField>().enabled = false;
            if (isInput)
            {
                numeroGadget = HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int;
            }
            else
            {
                numeroGadget = HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int;
            }
            switch (numeroGadget)
            {
                case InventoryConstants.Attract:
                    this.GetComponent<SVGImage>().sprite = Attract;
                        break;
                case InventoryConstants.Stunbox:
                    this.GetComponent<SVGImage>().sprite = Stun;
                    break;
                case InventoryConstants.Powerpump:
                    this.GetComponent<SVGImage>().sprite = PowerPump;
                    break;
                case InventoryConstants.BlueRelay:
                    this.GetComponent<SVGImage>().sprite = BlueRelay;
                    break;
                case InventoryConstants.OrangeRelay:
                    this.GetComponent<SVGImage>().sprite = RedRelay;
                    break;
                default:
                    break;
            }
            this.GetComponent<CanvasGroup>().alpha = 1f;

            /*Si la vignette est fixé dans le graphe, on empèche le joueur de changer le contenu de l'input field, sinon on le laisse faire.*/
            if (isFixed)
            {
                this.GetComponent<CanvasGroup>().interactable = false;
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
            this.GetComponent<InputField>().enabled = true;
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


    public void ShowRange()
    {
        gadgetRange.SetActive(true);
        isPointerOver = true;

        switch (numeroGadget)
        {
            case InventoryConstants.Attract:
                gadgetRange.transform.localScale = new Vector3(5, 5, 1);
                break;

            case InventoryConstants.Stunbox:
                gadgetRange.transform.localScale = new Vector3(10, 10, 1);
                break;

            case InventoryConstants.Powerpump:
                gadgetRange.transform.localScale = new Vector3(14, 14, 1);
                break;

            default:
                gadgetRange.SetActive(false);
                break;
        }
    }

    public void HideRange()
    {
        gadgetRange.SetActive(false);
        isPointerOver = false;
    }
#endif
    public void OnPointerDown(PointerEventData eventData)
    {
        
#if CLIENT
        if (isOnInt)
        {
            
            if (hackinterface.inventory[0] == InventoryConstants.Empty)
            {
                hackinterface.inventory[0] = HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int;
                HackInterface.outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                hackinterface.reloadInterface();
                hackinterface.RemoveVignette(false, this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
            }
            else if (hackinterface.inventory[1] == InventoryConstants.Empty)
            {
                hackinterface.inventory[1] = HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int;
                HackInterface.outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                hackinterface.reloadInterface();
                hackinterface.RemoveVignette(false, this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
            }
            else if (hackinterface.inventory[2] == InventoryConstants.Empty)
            {
                hackinterface.inventory[2] = HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].parameter_int;
                HackInterface.outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                hackinterface.reloadInterface();
                hackinterface.RemoveVignette(false, this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
            }
        }
#endif
    }
}
