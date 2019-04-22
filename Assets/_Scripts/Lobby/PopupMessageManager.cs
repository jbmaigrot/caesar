using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageManager : MonoBehaviour
{
    public Text messageField;
    public GameObject popupPanel;
#if CLIENT
    public void Show(string message)
    {
        messageField.text = message;
        popupPanel.SetActive(true);
    }

    public void Hide()
    {
        popupPanel.SetActive(false);
    }
#endif
}
