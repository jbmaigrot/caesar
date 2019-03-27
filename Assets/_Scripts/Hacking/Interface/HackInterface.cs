using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HackInterface : MonoBehaviour, ISelectObject
{
    static public int SelectedInputButton=-1;
    
    static public GameObject SelectedGameObject;
    public GameObject bonhomme;
    public HackingAssetScriptable HackingAsset;

    static public List<string> accessibleInputCode;
    static public List<string> accessibleOutputCode;

    static public List<InOutVignette> inputCodes = new List<InOutVignette>();
    static public List<InOutVignette> outputCodes = new List<InOutVignette>();
    static public List<Arrow> graph = new List<Arrow>();

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<CanvasGroup>().alpha=0f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        foreach (TextButtonHackInterface ryan in this.GetComponentsInChildren<TextButtonHackInterface>(false))
        {
            ryan.GetHackingAsset(HackingAsset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClose()
    {
        bonhomme.SetActive(true);
        SelectedInputButton = -1;
        SelectedGameObject.GetComponent<ProgrammableObjectsData>().inputCodes = inputCodes;
        SelectedGameObject.GetComponent<ProgrammableObjectsData>().outputCodes = outputCodes;
        SelectedGameObject.GetComponent<ProgrammableObjectsData>().graph = graph;
        SelectedGameObject = null;
        this.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SelectedProgrammableObject(GameObject SelectedObject)
    {
        SelectedGameObject = SelectedObject;
        accessibleInputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().accessibleInputCode);
        accessibleOutputCode = new List<string>(SelectedObject.GetComponent<ProgrammableObjectsData>().accessibleOutputCode);
        inputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().inputCodes);
        outputCodes = new List<InOutVignette>(SelectedObject.GetComponent<ProgrammableObjectsData>().outputCodes);
        graph = new List<Arrow>(SelectedObject.GetComponent<ProgrammableObjectsData>().graph);
        reloadInterface();
        reloadArrow();
        this.gameObject.GetComponent<CanvasGroup>().alpha =1f;
        this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
        
    }

    public void reloadInterface()
    {
        foreach (TextButtonHackInterface ryan in this.GetComponentsInChildren<TextButtonHackInterface>(false))
        {
            ryan.UpdateOptions(inputCodes.Count,outputCodes.Count);
        }        
    }

    public void reloadArrow()
    {
        foreach (ArrowHackInterface ryan in this.GetComponentsInChildren<ArrowHackInterface>(false))
        {
            ryan.UpdateArrow();
        }
    }
}
