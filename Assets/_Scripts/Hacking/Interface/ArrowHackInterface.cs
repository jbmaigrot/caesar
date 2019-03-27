using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowHackInterface : MonoBehaviour, IPointerDownHandler
{
    /*Variables pour savoir de quel arrow on parle. C'est rentré à la main dans l'éditeur, ce qui est améliorable.*/
    public int numero;
    
    /*Si on clic sur une arrow, elle est supprimé dans le graphe*/
    public void OnPointerDown(PointerEventData pointerEvent)
    {
        HackInterface.graph.RemoveAt(numero);
        /*On réecrit toutes les flèches. C'est nécessaire car il peut y avoir un décalage des flèches d'après.*/
        this.GetComponentInParent<HackInterface>().reloadArrow();
    }

    /*Fonction pour réecrire la flèche*/
    public void UpdateArrow()
    {
        if (HackInterface.graph.Count > numero)
        {
            this.GetComponent<CanvasGroup>().alpha = 1f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.GetComponentInChildren<Text>().text = HackInterface.graph[numero].input.ToString() + "   " + HackInterface.graph[numero].output.ToString();
        }
        else
        {
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            this.GetComponentInChildren<Text>().text = "";
        }
    }
}
