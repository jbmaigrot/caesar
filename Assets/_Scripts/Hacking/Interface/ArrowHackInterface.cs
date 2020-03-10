using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowHackInterface : MonoBehaviour, IPointerDownHandler
{
    /*Variables pour savoir de quel arrow on parle. C'est rentré à la main dans l'éditeur, ce qui est améliorable.*/
    public int numero;
    private HackInterface hackinterface;


    void Start()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		hackinterface = FindObjectOfType<HackInterface>();
    }
    /*Si on clic sur une arrow, elle est supprimé dans le graphe*/
    public void OnPointerDown(PointerEventData pointerEvent)
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (HackInterface.inputCodes.Count<= HackInterface.graph[numero].input || HackInterface.outputCodes.Count <= HackInterface.graph[numero].output || !HackInterface.inputCodes[HackInterface.graph[numero].input].is_fixed|| !HackInterface.outputCodes[HackInterface.graph[numero].output].is_fixed)
        {
            HackInterface.graph.RemoveAt(numero);
            hackinterface.ClicNeg();
            /*On réecrit toutes les flèches. C'est nécessaire car il peut y avoir un décalage des flèches d'après.*/
            this.GetComponentInParent<HackInterface>().reloadArrow();
        }
        else
        {
            hackinterface.ClicCancel();
        }
    }
    //public void OnClick()
    //{
    //    Debug.Log(name + "ONCLICK");
    //}
    /*Fonction pour réecrire la flèche*/
    public void UpdateArrow()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (HackInterface.graph.Count > numero)
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponentInChildren<Text>().text = "";
            //GetComponentInChildren<Text>().text = HackInterface.graph[numero].input.ToString() + "   " + HackInterface.graph[numero].output.ToString();

            GetComponent<Image>().sprite = this.GetComponentInParent<HackInterface>().ArrowSpriteTable[HackInterface.graph[numero].input * 5 + HackInterface.graph[numero].output];
            GetComponent<Image>().alphaHitTestMinimumThreshold = 0.01f;
            /*
            GetComponent<LineRenderer>().SetPosition(0, HackInterface.graph[numero].inputPos);
            GetComponent<LineRenderer>().SetPosition(1, HackInterface.graph[numero].outputPos);*/
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponentInChildren<Text>().text = "";
            /*
            GetComponent<LineRenderer>().SetPosition(0, new Vector3(0,0,0));
            GetComponent<LineRenderer>().SetPosition(1, new Vector3(0,0,0));*/
        }
    }
}