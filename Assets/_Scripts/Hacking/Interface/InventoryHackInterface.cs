using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryHackInterface : MonoBehaviour
{
    public int numero;
    public Sprite Empty;
    public Sprite Attract;
    public Sprite Stun;
    public Sprite PowerPump;
    public Sprite BlueRelay;
    public Sprite RedRelay;
    public GameObject gadgetRange;
#if CLIENT
    private HackInterface hackinterface;
    private bool isPointerOver = false;
    // Start is called before the first frame update
    private void Start()
    {
        hackinterface = FindObjectOfType<HackInterface>();
    }

    private void Update()
    {
        if (isPointerOver)
        {
            gadgetRange.transform.position = hackinterface.GetSelectedProgrammableObject().transform.position;
            
            if (Input.GetMouseButtonDown(0) && hackinterface.inventory[numero] != InventoryConstants.Empty && HackInterface.outputCodes.Count < 5)
            {
                hackinterface.ClicNeu();
                InOutVignette NewOutputHack = new InOutVignette();
                NewOutputHack.code = "UseGadget";
                NewOutputHack.parameter_int = hackinterface.inventory[numero];
                NewOutputHack.is_fixed = true;
                hackinterface.inventory[numero] = InventoryConstants.Empty;
                HackInterface.outputCodes.Add(NewOutputHack);
                hackinterface.reloadInterface();

                HideRange();
            }
        }
    }

    public void reloadInventory()
    {
        
        switch (hackinterface.inventory[numero])
        {
            case InventoryConstants.Empty:
                this.GetComponent<SVGImage>().sprite = Empty;
                break;

            case InventoryConstants.Attract:
                this.GetComponent<SVGImage>().sprite = Attract;
                break;

            case InventoryConstants.Stunbox:
                this.GetComponent<SVGImage>().sprite = Stun;
                break;

            case InventoryConstants.Powerpump:
                this.GetComponent<SVGImage>().sprite = PowerPump;
                break;

            case InventoryConstants.BlueRelay:
                this.GetComponent<SVGImage>().sprite = BlueRelay;
                break;

            case InventoryConstants.OrangeRelay:
                this.GetComponent<SVGImage>().sprite = RedRelay;
                break;

            default:
                break;
        }
            
    }

    public void ShowRange()
    {
        gadgetRange.SetActive(true);
        isPointerOver = true;

        switch (hackinterface.inventory[numero])
        {
            case InventoryConstants.Attract:
                gadgetRange.transform.localScale = InventoryConstants.AttractRange * Vector3.one;
                break;

            case InventoryConstants.Stunbox:
                gadgetRange.transform.localScale = InventoryConstants.StunboxRange * Vector3.one;
                break;

            case InventoryConstants.Powerpump:
                gadgetRange.transform.localScale = InventoryConstants.PowerpumpRange * Vector3.one;
                break;

            default:
                gadgetRange.SetActive(false);
                break;
        }
    }

    public void HideRange()
    {
        gadgetRange.SetActive(false);
        isPointerOver = false;
    }

#endif
}
