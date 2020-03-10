using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditManager : MonoBehaviour
{
    public GameObject popupPanel;

    public void Show()
    {
        if (!GameState.CLIENT) return; // replacement for preprocessor
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        if (!GameState.CLIENT) return; // replacement for preprocessor
        popupPanel.SetActive(false);
    }

}
