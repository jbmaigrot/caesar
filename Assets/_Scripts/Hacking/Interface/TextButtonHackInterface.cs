using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if CLIENT
public class TextButtonHackInterface : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
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
        hackInterface = FindObjectOfType<HackInterface>();
    }

    /*Récupère HackingAsset du parent, et le transmet aux enfants.*/
    public void GetHackingAsset(HackingAssetScriptable HackAss)
    {
        HackingAsset = HackAss;
        GetComponentInChildren<DropdownHackInterface>().GetHackingAsset(HackAss);
        GetComponentInChildren<InputFieldHackerInterface>().GetHackingAsset(HackAss);
    }

    /*Fonction appelé lorque le joueur clique sur la vignette. Utilisé pour créer de nouvelles connections dans le graphe.*/
    public void OnClick()
    {
        /*Si la vignette est une vignette d'interface, elle est séléctionné comme point de départ potentiel de la nouvelle connection*/
        if (isInput)
        {
            HackInterface.SelectedInputButton = numero-1;
          }
        /*Si une vignette d'entrée valide est séléctionnée et la vignette de sortie cliqué est valide aussi, on essaie de créer une connection.*/
        else if (HackInterface.SelectedInputButton > -1 && HackInterface.SelectedInputButton < HackInterface.inputCodes.Count && numero - 1 < HackInterface.outputCodes.Count)
        {
            /*Création de la nouvelle connection*/
            Arrow NewArrow = new Arrow();
            NewArrow.input = HackInterface.SelectedInputButton;
            NewArrow.output = numero - 1;

            //Debug.Log("" + HackInterface.SelectedInputButton + ", " + numero);
            NewArrow.inputPos = hackInterface.inputButtons[HackInterface.SelectedInputButton].position;
            NewArrow.outputPos = hackInterface.outputButtons[numero-1].position;

            HackInterface.SelectedInputButton = -1;

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
                this.GetComponentInParent<HackInterface>().reloadArrow();
            }
        }
    }
    
    /*Fonction pour écrire le contenu de la vignette*/
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
#endif