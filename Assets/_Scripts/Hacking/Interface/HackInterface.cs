using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HackInterface : MonoBehaviour/*, ISelectObject*/
{
    public AudioClip Clic_Cancel;
    public AudioClip Clic_Negatif;
    public AudioClip Clic_Neutre;
    public AudioClip Clic_Positif;
    public GameObject ErrorTextZone;
#if CLIENT
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

    private float timeReadyToOpen;
    private bool isReadyToOpen;
    const float TIMEREADYTOOPEN = 0.75f;

    private float timeToOpen;
    private bool isOpening;
    const float TIMETOOPEN = 0.2f;

    private Client client;
    private ProgrammableObjectsContainer objectsContainer;

    public int[] inventory = new int[3];
    //
    public RectTransform[] inputButtons = new RectTransform[0];
    public RectTransform[] outputButtons = new RectTransform[0];

    private Sprite[] SpriteList;
    public Sprite[] ArrowSpriteTable;

    public InventoryUI inventoryUI;

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
        SpriteList = Resources.FindObjectsOfTypeAll<Sprite>();
        ArrowSpriteTable = new Sprite[25];
        for (int ryan = 0; ryan<5; ryan++)
        {
            for(int reynolds = 0; reynolds < 5; reynolds++)
            {
                string name = ryan.ToString() + "" + reynolds.ToString();
                foreach (Sprite sp in SpriteList)
                {
                    if (sp.name == name)
                    {
                        ArrowSpriteTable[5 * ryan + reynolds] = sp;
                    }
                }
            }
        }
        reloadArrow();

        //var colorInventory = inventoryUI.GetComponent<SVGImage>().color;
        //inventoryUI.GetComponent<SVGImage>().color = new Color(colorInventory.r, colorInventory.g, colorInventory.b, 0f);
        
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
                inventoryUI.interfaceInventory.SetActive(true);
            }
        }
        if (isReadyToOpen)
        {
            timeReadyToOpen -= Time.deltaTime;
            if(timeReadyToOpen <=0 && SelectedGameObject != null)
            {
                isReadyToOpen = false;
                OpenInterface();
            }
        }
        if (isOpening)
        {
            timeToOpen -= Time.deltaTime;
            if (timeToOpen <= 0)
            {
                isOpening = false;
                ReallyOpenInterface();
                inventoryUI.interfaceInventory.SetActive(false);
            }
        }
        //click outside
        //TO DO   if ()
    }

     /*Fonction appelé lorsque le joueur ferme l'interface. A adapter pour le réseau.*/
    public void OnClose()
    {
        int RelayHasMoved = 0;
        ErrorTextZone.GetComponent<Animator>().SetTrigger("Interface_Close");
        if (!isClosing)
        {

            
            /*Initiation du délais de fermeture*/
            timeBeforeClosing = TIMEFORCLOSING;
            isClosing = true;
            ClicNeu();
            SelectedInputButton = -1;
            for(int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] == InventoryConstants.OrangeRelay)
                {
                    RelayHasMoved += 1;
                }
                else if (inventory[i] == InventoryConstants.BlueRelay)
                {
                    RelayHasMoved += 3;
                }
            }
            foreach (InOutVignette ryan in outputCodes)
            {
                if (ryan.code == "UseGadget" &&ryan.parameter_int == InventoryConstants.OrangeRelay)
                {
                    RelayHasMoved += 2;
                }
                else if (ryan.code == "UseGadget" && ryan.parameter_int == InventoryConstants.BlueRelay)
                {
                    RelayHasMoved += 6;
                }
            }

                /*Le graphe de comportement de l'objet hacké est remplacé par les modifications effectués.*/
                int objectId = objectsContainer.GetObjectIndexClient(SelectedGameObject.GetComponent<ProgrammableObjectsData>());
            client.SetHackState(objectId, inputCodes, outputCodes, graph,RelayHasMoved);
            client.GiveBackHackToken(objectId);
            client.inventory[0] = inventory[0];
            client.inventory[1] = inventory[1];
            client.inventory[2] = inventory[2];

            inventoryUI.ReloadInventory();
            //var colorInventory = inventoryUI.GetComponent<SVGImage>().color;
            //inventoryUI.GetComponent<SVGImage>().color = new Color(colorInventory.r, colorInventory.g, colorInventory.b, 1f);
            //inventoryUI.GetComponent<GameObject>().SetActive(true);

            Camera.main.GetComponent<CameraController>().UnlockCamera();
        }        
    }

    public void CloseByStun()
    {
        ErrorTextZone.GetComponent<Animator>().SetTrigger("Interface_Close");
        if (!isClosing)
        {
            if(SelectedGameObject != null)
            {
                client.GiveBackHackToken(objectsContainer.GetObjectIndexClient(SelectedGameObject.GetComponent<ProgrammableObjectsData>()));
            }            
            timeBeforeClosing = TIMEFORCLOSING;
            isClosing = true;

            SelectedInputButton = -1;
            Camera.main.GetComponent<CameraController>().UnlockCamera();
        }        
    }

    /*Fonction appelé lorsque un objet est hacké par le joueur. A adapter pour le réseau*/
    //public void SelectedProgrammableObject(GameObject SelectedObject)
    //{
    //    /*Copie du graphe de comportement de l'objet*/
    //    SelectedGameObject = SelectedObject;
    //    accessibleInputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleInputCode);
    //    accessibleOutputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleOutputCode);

    //    //à récupérer depuis le serveur
    //    inputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().inputCodes);
    //    outputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().outputCodes);
    //    graph = new List<Arrow>(SelectedObject.GetComponent<ProgrammableObjectsData>().graph);

    //    /*Ecriture du contenu de l'interface*/
    //    reloadInterface();
    //    reloadArrow();
    //    isClosing = false;

    //    /*Ouverture de l'interface*/
    //    this.gameObject.GetComponent<CanvasGroup>().alpha =1f;
    //    this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        
    //}


    //Network compatible version of the function
    public void SelectedProgrammableObject(GameObject SelectedObject, List<InOutVignette> _inputCodes, List<InOutVignette> _outputCodes, List<Arrow> _graph)
    {
        if (isReadyToOpen)
        {
            /*Copie du graphe de comportement de l'objet*/
            SelectedGameObject = SelectedObject;
            accessibleInputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleInputCode);
            accessibleOutputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().Initiator.accessibleOutputCode);

            //à récupérer depuis le serveur
            inputCodes = _inputCodes;
            outputCodes = _outputCodes;
            graph = _graph;

            inventory[0] = client.inventory[0];
            inventory[1] = client.inventory[1];
            inventory[2] = client.inventory[2];
            /*Ecriture du contenu de l'interface*/
            reloadInterface();
            reloadArrow();
            isClosing = false;
            
        }
    }

    public GameObject GetSelectedProgrammableObject()
    {
        return SelectedGameObject;
    }


    public void ReadyToOpen()
    {
        isReadyToOpen = true;
        timeReadyToOpen = TIMEREADYTOOPEN;
    }

    public void DoNotOpenActually(int objectIndex)
    {
        if (isReadyToOpen)
        {
            isReadyToOpen = false;
            client.GiveBackHackToken(objectIndex);
            SelectedGameObject = null;
        }        
    }

    public void OpenInterface()
    {
        client.CutSoundOfHackPlease();
        client.OpenedHackInterface(SelectedGameObject.GetComponent<ProgrammableObjectsData>().objectIndexClient);
        SelectedGameObject.GetComponent<ProgrammableObjectsData>().isWaitingHack = false;
        isOpening = true;
        timeToOpen = TIMETOOPEN;
    }

    public void ReallyOpenInterface()
    {
        /*Ouverture de l'interface*/
        this.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Camera.main.GetComponent<CameraController>().CameraModeFollow(SelectedGameObject);
    }

    /*Ecriture des vignettes de l'interface*/
    public void reloadInterface()
    {
        foreach (TextButtonHackInterface ryan in this.GetComponentsInChildren<TextButtonHackInterface>(false))
        {
            ryan.UpdateOptions(inputCodes.Count,outputCodes.Count);
        }
        foreach(InventoryHackInterface ryan in this.GetComponentsInChildren<InventoryHackInterface>(false))
        {
            ryan.reloadInventory();
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

    public void RemoveVignette(bool isInput, int num)
    {
        if (isInput)
        {
            foreach (Arrow ryan in graph.ToArray()) // tricks using ToArray() from stackoverflow.com/questions/3545731 to prevent index exception when removing elements from the list in the loop
            {
                if (ryan.input == num)
                {
                    graph.Remove(ryan);
                }
                else if (ryan.input > num)
                {
                    ryan.input -= 1;
                }
            }
        }
        else
        {
            foreach (Arrow ryan in graph.ToArray())
            {
                if (ryan.output == num)
                {
                    graph.Remove(ryan);
                }
                else if (ryan.output > num)
                {
                    ryan.output -= 1;
                }
            }
        }
        reloadArrow();
    }

    public void SomeoneHackedTheSameObject()
    {
        ErrorTextZone.GetComponent<Animator>().SetTrigger("Fade_Text");
    }
    /*
    //Draw arrows
    public Material lineMat = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");

    private void OnPostRender()
    {
        GL.Begin(GL.LINES);
        lineMat.SetPass(0);
        GL.Color(new Color(0f, 0f, 0f, 1f));
        GL.Vertex3(0f, 0f, 0f);
        GL.Vertex3(1f, 1f, 1f);
        GL.End();
    }*/

    public void ClicCancel()
    {
        GetComponent<AudioSource>().PlayOneShot(Clic_Cancel);
    }

    public void ClicPos()
    {
        GetComponent<AudioSource>().PlayOneShot(Clic_Positif);
    }

    public void ClicNeu()
    {
        GetComponent<AudioSource>().PlayOneShot(Clic_Neutre);
    }

    public void ClicNeg()
    {
        GetComponent<AudioSource>().PlayOneShot(Clic_Negatif);
    }
#endif
}

