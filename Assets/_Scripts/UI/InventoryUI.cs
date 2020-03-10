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
		if (!GameState.CLIENT) return; // replacement for preprocessor

		//client = FindObjectOfType<Client>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		if (!inventoryLoaded && client.inventory!= null)
        {
            ReloadInventory();
            inventoryLoaded = true;
        }
    }

    public void ReloadInventory()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		foreach (InventoryUISlot slot in inventory)
        {
            slot.reloadSlot(client);
        }
    }
}
