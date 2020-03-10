using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageManager : MonoBehaviour
{
    public Text messageField;
    public GameObject popupPanel;

    public void Show(string message)
    {
        if (!GameState.CLIENT) return; // replacement for preprocessor
        messageField.text = message;
        popupPanel.SetActive(true);
    }

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
