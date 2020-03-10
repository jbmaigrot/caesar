using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextButtonHackInterface : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
    public bool isMouseOver = false;
    private PlayerInput playerinput;

    /*Variables pour savoir de quel bouton on parle. Exemple c'est la 3ième vignette d'input. C'est rentré à la main dans l'éditeur, ce qui est améliorable.*/
    public bool isInput;
    public int numero;

    /*Variable pour savoir si cette vignette peut être modifié ou si elle fait partie des éléments fixes du graphe de comportement.*/
    private bool isFixed;

    /*Variable pour savoir quel code est actuellement dans la vignette.*/
    private string code;

    /*Variable qui contient le dictionnaire de mot-clefs, lié à leur description dans l'interface et à si la vignette a besoin de parametre. Sous forme de List.*/
    private HackingAssetScriptable HackingAsset;

    //(test) used for getting buttons coords
    private HackInterface hackInterface;

    private void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		hackInterface = FindObjectOfType<HackInterface>();
        playerinput = FindObjectOfType<PlayerInput>();
    }

    /*Récupère HackingAsset du parent, et le transmet aux enfants.*/
    public void GetHackingAsset(HackingAssetScriptable HackAss)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		HackingAsset = HackAss;
        GetComponentInChildren<DropdownHackInterface>().GetHackingAsset(HackAss);
        GetComponentInChildren<InputFieldHackerInterface>().GetHackingAsset(HackAss);
    }

    //Setter isMouseOver
    public void SetIsMouseOver(bool b)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		isMouseOver = b;
        playerinput.isMouseOverAnOutputTextButtonhackInterface = b;
    }

    /*Fonction appelé lorque le joueur clique sur la vignette. Utilisé pour créer de nouvelles connections dans le graphe.*/
    public void OnClick()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		/*Si la vignette est une vignette d'interface, elle est séléctionné comme point de départ potentiel de la nouvelle connection*/
		if (isInput)
        {
            HackInterface.SelectedInputButton = numero - 1;
        }
        else
        {
            HackInterface.SelectedOutputButton = numero - 1;
        }
        ComputeSelectedButtons();
        hackInterface.dragAndDropPending = false;
    }

    private void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		// Début de drag and drop
		if (Input.GetMouseButtonDown(0) && isMouseOver)
        {
            if (isInput)
            {
                HackInterface.SelectedInputDragAndDrop = numero - 1;
            }
            else
            {
                HackInterface.SelectedOutputDragAndDrop = numero - 1;
            }
            hackInterface.dragAndDropPending = true;
        }

        // Fin de drag and drop
        if (Input.GetMouseButtonUp(0) && isMouseOver && hackInterface.dragAndDropPending)
        {
            if (isInput)
            {
                if(HackInterface.SelectedInputDragAndDrop != numero - 1)
                {
                    HackInterface.SelectedInputButton = numero - 1;
                    HackInterface.SelectedOutputButton = HackInterface.SelectedOutputDragAndDrop;
                    ComputeSelectedButtons();
                }
                
            }
            else
            {
                if (HackInterface.SelectedOutputDragAndDrop != numero - 1)
                {
                    HackInterface.SelectedOutputButton = numero - 1;
                    HackInterface.SelectedInputButton = HackInterface.SelectedInputDragAndDrop;
                    ComputeSelectedButtons();
                }
            }
            
        }
    }

    private void LateUpdate()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (Input.GetMouseButtonUp(0))
        {
            hackInterface.dragAndDropPending = false;
            HackInterface.SelectedInputDragAndDrop = - 1;
            HackInterface.SelectedOutputDragAndDrop = - 1;
        }
    }

    //Appelé quand on clique (ou quand on relâche le clic sur un output), après l'update des numéros sélectionnés
    private void ComputeSelectedButtons()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (HackInterface.SelectedInputButton > -1 && HackInterface.SelectedInputButton < HackInterface.inputCodes.Count+1  && HackInterface.SelectedOutputButton>-1 && HackInterface.SelectedOutputButton < HackInterface.outputCodes.Count + 1)
        {
            /*Création de la nouvelle connection*/
            Arrow NewArrow = new Arrow();
            NewArrow.input = HackInterface.SelectedInputButton;
            NewArrow.output = HackInterface.SelectedOutputButton;

            //Debug.Log("" + HackInterface.SelectedInputButton + ", " + numero);
            NewArrow.inputPos = hackInterface.inputButtons[HackInterface.SelectedInputButton].position;
            NewArrow.outputPos = hackInterface.outputButtons[HackInterface.SelectedOutputButton].position;

            HackInterface.SelectedInputButton = -1;
            HackInterface.SelectedOutputButton = -1;

            /*Verification que la connection n'existe pas déjà dans le graphe*/
            bool isItReallyNew = true;
            foreach (Arrow a in HackInterface.graph)
            {
                if (a.input == NewArrow.input && a.output == NewArrow.output) isItReallyNew = false;
            }

            /*Ajout de la nouvelle connection au graphe*/
            if (isItReallyNew)
            {
                HackInterface.graph.Add(NewArrow);
                hackInterface.ClicPos();
                this.GetComponentInParent<HackInterface>().reloadArrow();
            }
            else
            {
                hackInterface.ClicCancel();
            }
        }
        else
        {
            hackInterface.ClicNeu();
        }
        /*Si une vignette d'entrée valide est séléctionnée et la vignette de sortie cliqué est valide aussi, on essaie de créer une connection.*/
       
    }
    
    /*Fonction pour écrire le contenu de la vignette*/
    public void UpdateOptions(int inputCount, int outputCount)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

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