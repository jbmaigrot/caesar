using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownHackInterface : MonoBehaviour, IPointerClickHandler
{

    private int previousValue;

    private HackingAssetScriptable HackingAsset;

    private List<string> accessibleCode;

    private HackInterface hackInterface;

    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		accessibleCode = new List<string>();
        hackInterface = FindObjectOfType<HackInterface>();
      
    }

    

    public void OnPointerClick(PointerEventData eventData)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		hackInterface.ClicNeu();
    }

    /*Fonction pour modifier le code d'une vignette en fonction du dropdown*/
    void UpdateHackingGraph()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		/*Si le Dropdown est placé sur blank, on supprime la vignette*/
		if (this.GetComponent<Dropdown>().value == 0)
        {
            this.GetComponentInParent<HackInterface>().RemoveVignette(this.GetComponentInParent<TextButtonHackInterface>().isInput, this.GetComponentInParent<TextButtonHackInterface>().numero-1);
            if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
            {
                HackInterface.inputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero-1);
            }
            else
            {

                HackInterface.outputCodes.RemoveAt(this.GetComponentInParent<TextButtonHackInterface>().numero - 1);
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
                    NewInputHack.code = accessibleCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.inputCodes.Add(NewInputHack);
                }
                else
                {
                    InOutVignette NewOutputHack = new InOutVignette();
                    NewOutputHack.code = accessibleCode[this.GetComponent<Dropdown>().value - 1];
                    HackInterface.outputCodes.Add(NewOutputHack);
                }
            }
            /*Si le Dropdown a été changé entre deux valeurs non-blank, on change le graphe en conséquence.*/
            else
            {
                if (this.GetComponentInParent<TextButtonHackInterface>().isInput)
                {
                    HackInterface.inputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = accessibleCode[this.GetComponent<Dropdown>().value - 1];
                }
                else
                {
                    HackInterface.outputCodes[this.GetComponentInParent<TextButtonHackInterface>().numero - 1].code = accessibleCode[this.GetComponent<Dropdown>().value - 1];
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
		if (!GameState.CLIENT) return; // replacement for preprocessor

		/*Si la valeur du dropdown a été changé par le joueur, on modifie le graphe en conséquence*/

	}

	public void ChangeValue()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (this.GetComponent<Dropdown>().value != 0)
        {
            hackInterface.ClicPos();
        }
        else
        {
            hackInterface.ClicNeg();
        }
        if (this.GetComponent<Dropdown>().value != previousValue)
        {
            UpdateHackingGraph();

        }
    }

    /*Eteint le bouton*/
    public void UpdateOff()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    /*Ecris le bouton en blank. Ne sera utilisé que sur le dernier bouton actif*/
    public void UpdateBlank(bool isInput)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		UpdateOptions(isInput);
        this.GetComponent<Dropdown>().value = 0;
        previousValue = 0;
    }

    /*Ecris le bouton avec le contenu de la vignette correspondante, en connaissant le code actif et si c'est un élément fixe du graphe.*/
    public void UpdateOn(bool isInput, bool isFixed, string code)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

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
		if (!GameState.CLIENT) return -1; // replacement for preprocessor

		this.GetComponent<Dropdown>().ClearOptions();

        Dropdown.OptionData NewData;
        NewData = new Dropdown.OptionData();
        List<Dropdown.OptionData> ListNewData = new List<Dropdown.OptionData>();

        int indice = 0;
        int retour = 0;

        accessibleCode.Clear();
        

        if (HackInterface.SelectedGameObject != null)
        {
            if (isInput)
            {
                NewData.image = null;

                NewData.text = "";
                ListNewData.Add(NewData);
                foreach (string ryan in HackInterface.accessibleInputCode)
                {
                    bool hasToBeWritten = true;

                    for (int i = 0; i < HackInterface.inputCodes.Count; i++)
                    {
                        if (i != this.GetComponentInParent<TextButtonHackInterface>().numero - 1 && HackInterface.inputCodes[i].code == ryan) hasToBeWritten = false;
                    }

                    if (!hasToBeWritten)
                    {
                        for (int i = 0; i < HackingAsset.inputCodes.Count; i++)
                        {
                            if (HackingAsset.inputCodes[i].code == ryan  && HackingAsset.inputCodes[i].parameter_string) hasToBeWritten = true;
                            if (HackingAsset.inputCodes[i].code == ryan && HackingAsset.inputCodes[i].parameter_int) hasToBeWritten = true;
                        }
                    }

                    if (hasToBeWritten) {
                        indice += 1;
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = HackingAsset.DescribeCode(ryan, false, true);

                        ListNewData.Add(NewData);
                        if (ryan == code)
                        {
                            retour = indice;
                        }
                        accessibleCode.Add(ryan);
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
                    bool hasToBeWritten = true;

                    for (int i = 0; i < HackInterface.outputCodes.Count; i++)
                    {
                        if (i != this.GetComponentInParent<TextButtonHackInterface>().numero - 1 && HackInterface.outputCodes[i].code == ryan) hasToBeWritten = false;
                    }

                    if (!hasToBeWritten)
                    {
                        for (int i = 0; i < HackingAsset.outputCodes.Count; i++)
                        {
                            if (HackingAsset.outputCodes[i].code == ryan && HackingAsset.outputCodes[i].parameter_string) hasToBeWritten = true;
                            if (HackingAsset.outputCodes[i].code == ryan && HackingAsset.outputCodes[i].parameter_int) hasToBeWritten = true;
                        }
                    }

                    if (hasToBeWritten)
                    {
                        indice += 1;
                        NewData = new Dropdown.OptionData();
                        NewData.image = null;
                        NewData.text = HackingAsset.DescribeCode(ryan, false, false);

                        ListNewData.Add(NewData);
                        if (ryan == code)
                        {
                            retour = indice;
                        }
                        accessibleCode.Add(ryan);
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
		if (!GameState.CLIENT) return; // replacement for preprocessor

		HackingAsset = HackAss;
    }
}