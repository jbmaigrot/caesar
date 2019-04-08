using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if CLIENT
public class DropdownHackInterface : MonoBehaviour
{

    private int previousValue;

    private HackingAssetScriptable HackingAsset;


    /*Fonction pour modifier le code d'une vignette en fonction du dropdown*/
    void UpdateHackingGraph()
    {
        /*Si le Dropdown est placé sur blank, on supprime la vignette*/
        if (this.GetComponent<Dropdown>().value == 0)
        {
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.inputCodes.Count)
                {
                    HackInterface.inputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                }
            }
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().numero <= HackInterface.outputCodes.Count)
                {
                    HackInterface.outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
                }
            }
        }
        else
        {   
            /*Si le Dropdown était placé sur blank mais a été changé, on crée une nouvelle vignette. Cela ne peut arriver que sur le dernier bouton actif, car c'est le seul qui est blank.*/
            if (previousValue == 0)
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    InOutVignette NewInputHack = new InOutVignette();
                    NewInputHack.code = HackInterface.accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.inputCodes.Add(NewInputHack);
                }
                else
                {
                    InOutVignette NewOutputHack = new InOutVignette();
                    NewOutputHack.code = HackInterface.accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.outputCodes.Add(NewOutputHack);
                }
            }
            /*Si le Dropdown a été changé entre deux valeurs non-blank, on change le graphe en conséquence.*/
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = HackInterface.accessibleInputCode[this.GetComponent<Dropdown>().value - 1];
                }
                else
                {
                    HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = HackInterface.accessibleOutputCode[this.GetComponent<Dropdown>().value - 1];
                }
            }
        }
        /*On réecris le contenu de tous les boutons. C'est nécessaire car il peut y avoir eu suppression ou ajout de vignette et donc décalage de certaines vignettes*/
        this.GetComponentInParent<HackInterface>().reloadInterface();

        /*On retiens la nouvelle valeur du dropdown*/
        previousValue = this.GetComponent<Dropdown>().value;

    }

    // Update is called once per frame
    void Update()
    {
        /*Si la valeur du dropdown a été changé par le joueur, on modifie le graphe en conséquence*/
        if (this.GetComponent<Dropdown>().value != previousValue)
        {
            UpdateHackingGraph();

        }
    }

    /*Eteint le bouton*/
    public void UpdateOff()
    {
        this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    /*Ecris le bouton en blank. Ne sera utilisé que sur le dernier bouton actif*/
    public void UpdateBlank(bool isInput)
    {
        UpdateOptions(isInput);
        this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    /*Ecris le bouton avec le contenu de la vignette correspondante, en connaissant le code actif et si c'est un élément fixe du graphe.*/
    public void UpdateOn(bool isInput, bool isFixed, string code)
    {
        /*Si la vignette est un élement fixe du graphe, le dropdown contient seulement le code actif*/
        if (isFixed)
        {
            this.GetComponent<Dropdown>().ClearOptions();
            Dropdown.OptionData NewData; NewData = new Dropdown.OptionData();
            List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();
            NewData.image = null;

            if (isInput)
            {
                NewData.text = HackingAsset.DescribeCode(code, false, true);
            }
            else
            {
                NewData.text = HackingAsset.DescribeCode(code, false, false);
            }
            ListNewData.Add(NewData);
            this.GetComponent<Dropdown>().AddOptions(ListNewData);
            this.GetComponent<Dropdown>().value = 0;
            previousValue = 0;
        }
        /*Si la vignette n'est pas un element fixe, on appelle UpdateOptions pour ecrire la liste des options, et on place le dropdown sur la valeur active.*/
        else
        {
            this.GetComponent<Dropdown>().value = UpdateOptions(isInput, code);
            previousValue = this.GetComponent<Dropdown>().value;
        }
    }

    /*Ecrit les differentes options du dropdown et renvoie le numéro correspondant au code envoyer en parametre*/
    private int UpdateOptions(bool isInput, string code = "")
    {
        this.GetComponent<Dropdown>().ClearOptions();

        Dropdown.OptionData NewData; NewData = new Dropdown.OptionData();
        List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();

        int indice = 0;
        int retour = 0;

        if (HackInterface.SelectedGameObject != null)
        {
            if (isInput)
            {
                NewData.image = null;

                NewData.text = "";
                ListNewData.Add(NewData);
                foreach (string ryan in HackInterface.accessibleInputCode)
                {
                    indice += 1;
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackingAsset.DescribeCode(ryan, false, true);

                    ListNewData.Add(NewData);
                    if (ryan == code)
                    {
                        retour= indice;
                    }
                }
            }
            else
            {
                NewData.image = null;

                NewData.text = "";
                ListNewData.Add(NewData);
                foreach (string ryan in HackInterface.accessibleOutputCode)
                {
                    indice += 1;
                    NewData = new Dropdown.OptionData();
                    NewData.image = null;
                    NewData.text = HackingAsset.DescribeCode(ryan, false, false);

                    ListNewData.Add(NewData);
                    if (ryan == code)
                    {
                        retour= indice;
                    }
                }
            }
            this.GetComponent<Dropdown>().AddOptions(ListNewData);
            

        }
        return retour;
    }

    /*Récupère HackingAsset du parent.*/
    public void GetHackingAsset(HackingAssetScriptable HackAss)
    {
        HackingAsset = HackAss;
    }
}
#endif