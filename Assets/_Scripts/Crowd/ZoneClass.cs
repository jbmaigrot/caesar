using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneClass
{
    public GameObject ZoneGameObject;
    private List<SlotClass> _listSlots = new List<SlotClass>();

    public void FillListSlot()
    {
        //if (!ZoneGameObject == null)
        //{
            foreach (Transform child in ZoneGameObject.transform)
            {
                SlotClass sc = new SlotClass();
                sc.SlotGameObject = child.gameObject;
                _listSlots.Add(sc);
            }
        //}
    }

    public void EmptyListSlot()
    {
        foreach (SlotClass slot in _listSlots)
        {
            slot.IsUsed = false;
        }
    }
	
	public SlotClass GetFreeSlot(){
		foreach(SlotClass slot in _listSlots){
			if(!slot.IsUsed){
                Debug.Log("slot test");
					slot.IsUsed = true;
					return slot;
			}
		}
		return null;
	}
}

public class SlotClass
{
    public GameObject SlotGameObject;
    public bool IsUsed = false;
}
