using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HackInterface : MonoBehaviour, ISelectObject
{
    /*Variable qui contient la vignette d'input selectionné comme début de flèche. Est modifié par le script TextButtonHackInterface.*/
    static public int SelectedInputButton=-1;
    
    /*Variable qui contient l'objet connecté en cours de hacking. Est modifié par le script ProgrammableObjectsData lorsque un objet est hacké.*/
    static public GameObject SelectedGameObject;

    /*Variable qui contient le dictionnaire de mot-clefs, lié à leur description dans l'interface et à si la vignette a besoin de parametre. Sous forme de List.*/
    public HackingAssetScriptable HackingAsset;

    /*Variables qui contient la copie du graphe de comportement de l'objet connecté en cours de hacking. Tous les éléments de l'interface ont besoin d'avoir accès à ces variables (en read only pour les deux premières).*/
    static public List<string> accessibleInputCode;
    static public List<string> accessibleOutputCode;
    static public List<InOutVignette> inputCodes = new List<InOutVignette>();
    static public List<InOutVignette> outputCodes = new List<InOutVignette>();
    static public List<Arrow> graph = new List<Arrow>();

    /*Variables utilisées pour le délais de fermeture d'interface*/
    private float timeBeforeClosing;
    private bool isClosing;
    const float TIMEFORCLOSING = 0.1f;

    private Client client;
    private ProgrammableObjectsContainer objectsContainer;

    // Start is called before the first frame update
    void Start()
    {
        client = FindObjectOfType<Client>();
        objectsContainer = FindObjectOfType<ProgrammableObjectsContainer>();
        this.gameObject.GetComponent<CanvasGroup>().alpha=0f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        isClosing = false;
        foreach (TextButtonHackInterface ryan in this.GetComponentsInChildren<TextButtonHackInterface>(false))
        {
            ryan.GetHackingAsset(HackingAsset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*Gestion du délais de fermeture d'interface*/
        if (isClosing)
        {
            timeBeforeClosing -= Time.deltaTime;
            if (timeBeforeClosing <= 0.0f)
            {
                isClosing = false;
                SelectedGameObject = null;
                this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

     /*Fonction appelé lorsque le joueur ferme l'interface. A adapter pour le réseau.*/
    public void OnClose()
    {
        /*Initiation du délais de fermeture*/
        timeBeforeClosing = TIMEFORCLOSING;
        isClosing = true;

        SelectedInputButton = -1;
        
        /*Le graphe de comportement de l'objet hacké est remplacé par les modifications effectués.*/
        client.SetHackState(objectsContainer.GetObjectIndex(SelectedGameObject.GetComponent<ProgrammableObjectsData>()), inputCodes, outputCodes, graph);

        Camera.main.GetComponent<CameraController>().UnlockCamera();
    }

    /*Fonction appelé lorsque un objet est hacké par le joueur. A adapter pour le réseau*/
    public void SelectedProgrammableObject(GameObject SelectedObject)
    {
        /*Copie du graphe de comportement de l'objet*/
        SelectedGameObject = SelectedObject;
        accessibleInputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleInputCode);
        accessibleOutputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleOutputCode);

        //à récupérer depuis le serveur
        inputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().inputCodes);
        outputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().outputCodes);
        graph = new List<Arrow>(SelectedObject.GetComponent<ProgrammableObjectsData>().graph);

        /*Ecriture du contenu de l'interface*/
        reloadInterface();
        reloadArrow();
        isClosing = false;

        /*Ouverture de l'interface*/
        this.gameObject.GetComponent<CanvasGroup>().alpha =1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        
    }


    //Network compatible version of the function
    public void SelectedProgrammableObject(GameObject SelectedObject, List<InOutVignette> _inputCodes, List<InOutVignette> _outputCodes, List<Arrow> _graph)
    {
        /*Copie du graphe de comportement de l'objet*/
        SelectedGameObject = SelectedObject;
        accessibleInputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleInputCode);
        accessibleOutputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleOutputCode);

        //à récupérer depuis le serveur
        inputCodes = _inputCodes;
        outputCodes = _outputCodes;
        graph = _graph;

        /*Ecriture du contenu de l'interface*/
        reloadInterface();
        reloadArrow();
        isClosing = false;

        /*Ouverture de l'interface*/
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

        SelectedObject.GetComponent<ProgrammableObjectsData>().OnInput("OnHack");
        Camera.main.GetComponent<CameraController>().LockCamera();
    }

    /*Ecriture des vignettes de l'interface*/
    public void reloadInterface()
    {
        foreach (TextButtonHackInterface ryan in this.GetComponentsInChildren<TextButtonHackInterface>(false))
        {
            ryan.UpdateOptions(inputCodes.Count,outputCodes.Count);
        }        
    }

    /*Ecriture des fleches de l'interface*/
    public void reloadArrow()
    {
        foreach (ArrowHackInterface ryan in this.GetComponentsInChildren<ArrowHackInterface>(false))
        {
            ryan.UpdateArrow();
        }
    }
}
