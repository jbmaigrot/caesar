﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryHackInterface : MonoBehaviour, IPointerDownHandler
{
    public int numero;
    public Sprite Empty;
    public Sprite Attract;
    public Sprite Stun;
    public Sprite PowerPump;
#if CLIENT
    private HackInterface hackinterface;
    // Start is called before the first frame update
    void Start()
    {
        hackinterface = FindObjectOfType<HackInterface>();
    }

    public void reloadInventory()
    {
        
        switch (hackinterface.inventory[numero])
        {
            case InventoryConstants.Empty:
                this.GetComponent<SVGImage>().sprite = Empty;
                Debug.Log("affichage0");
                break;
            case InventoryConstants.Attract:
                this.GetComponent<SVGImage>().sprite = Attract;
<<<<<<< HEAD
                Debug.Log("affichage2");
=======
>>>>>>> parent of 8cb9dc8... modif prefab hackinginterface
                break;
            case InventoryConstants.Stunbox:
                this.GetComponent<SVGImage>().sprite = Stun;
                Debug.Log("affichage3");
                break;
            case InventoryConstants.Powerpump:
                Debug.Log("affichage4");
                this.GetComponent<SVGImage>().sprite = PowerPump;
                break;
            default:
                break;
        }
            
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(hackinterface.inventory[numero] != InventoryConstants.Empty && HackInterface.outputCodes.Count < 5)
        {
            InOutVignette NewOutputHack = new InOutVignette();
            NewOutputHack.code = "UseGadget";
            NewOutputHack.parameter_int = hackinterface.inventory[numero];
            hackinterface.inventory[numero] = InventoryConstants.Empty;
            HackInterface.outputCodes.Add(NewOutputHack);
            hackinterface.reloadInterface();
        }
    }
#endif 
}
