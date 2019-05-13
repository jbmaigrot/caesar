using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventoryUISlot[] inventory;
    public Client client;
    public GameObject interfaceInventory;

    private bool inventoryLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        //client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inventoryLoaded && client.inventory!= null)
        {
            ReloadInventory();
            inventoryLoaded = true;
        }
    }

    public void ReloadInventory()
    {
        foreach (InventoryUISlot slot in inventory)
        {
            slot.reloadSlot(client);
        }
    }
}
