using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public GameObject popupPanel;
#if CLIENT
    public void Show()
    {
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }
#endif
}
