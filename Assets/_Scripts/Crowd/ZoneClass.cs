using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneClass
{
    public GameObject ZoneGameObject;
    public GameObject ConnectedGameObject;
    private List<SlotClass> _listSlots = new List<SlotClass>();

    public void FillListSlot()
    {
        //if (!ZoneGameObject == null)
        //{
        ///////TODO CLEAN
            foreach (Transform child in ZoneGameObject.transform)
            {
                if (child.tag == "ConnectedObject")
                {
                    bool test = child.gameObject != null;
                    Debug.Log("ConnectedObject " + test);
                    ConnectedGameObject = child.gameObject;
                }

            }
            foreach (Transform child in ZoneGameObject.transform)
            {
                SlotClass sc = new SlotClass();
                if (child.tag == "Zone")
                {
                    sc.SlotGameObject = child.gameObject;
                    sc.ConnectedGameObject = ConnectedGameObject;
                    _listSlots.Add(sc);
                }
                
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
					slot.IsUsed = true;
			    bool test = slot.ConnectedGameObject != null;
			    Debug.Log("GetFreeSlot " + test);
                return slot;
			}
		}
		return null;
	}
}

public class SlotClass
{
    public GameObject SlotGameObject;
    public GameObject ConnectedGameObject;
    public bool IsUsed = false;


}
