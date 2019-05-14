using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUISlot : MonoBehaviour
{
    public int numero;
    public Sprite Empty;
    public Sprite Attract;
    public Sprite Stun;
    public Sprite PowerPump;
    public Sprite RedRelay;
    public Sprite BlueRelay;
#if CLIENT
    //private Client client;

    void Start()
    {
        //client = FindObjectOfType<Client>();
    }

    
    void Update()
    {
        
    }

    public void reloadSlot(Client client)
    {
        //if (client.inventory == null)
        //{
        //    client = FindObjectOfType<Client>();
        //}
        switch (client.inventory[numero])
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
#endif
}
