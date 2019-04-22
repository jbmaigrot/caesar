using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if CLIENT
public class ArrowHackInterface : MonoBehaviour, IPointerDownHandler
{
    /*Variables pour savoir de quel arrow on parle. C'est rentré à la main dans l'éditeur, ce qui est améliorable.*/
    public int numero;

    private Sprite[] SpriteList;
    
    /*Si on clic sur une arrow, elle est supprimé dans le graphe*/
    public void OnPointerDown(PointerEventData pointerEvent)
    {
        HackInterface.graph.RemoveAt(numero);
        /*On réecrit toutes les flèches. C'est nécessaire car il peut y avoir un décalage des flèches d'après.*/
        this.GetComponentInParent<HackInterface>().reloadArrow();
    }
    //public void OnClick()
    //{
    //    Debug.Log(name + "ONCLICK");
    //}
    /*Fonction pour réecrire la flèche*/
    public void UpdateArrow()
    {
        if (HackInterface.graph.Count > numero)
        {
            SpriteList = Resources.FindObjectsOfTypeAll<Sprite>();
            Debug.Log(SpriteList);
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponentInChildren<Text>().text = HackInterface.graph[numero].input.ToString() + "   " + HackInterface.graph[numero].output.ToString();
            string name = HackInterface.graph[numero].input.ToString() + "" + HackInterface.graph[numero].output.ToString();
            //foreach (Sprite sp in SpriteList)
            //{
            //    if (sp.name == name)
            //    {
            //        Debug.Log("weeeeeeeeeeeeeeeeeeee");
            //       //GetComponent<Image>().sprite = sp;
            //       Debug.Log("alpha" + GetComponent<Image>().alphaHitTestMinimumThreshold);
            //        //GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
            //        Debug.Log("alpha" + GetComponent<Image>().alphaHitTestMinimumThreshold);
            //    }
            //}
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
#endif