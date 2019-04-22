using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingMessageManager : MonoBehaviour
{
    public GameObject connectingPanel;
#if CLIENT
    public void Show()
    {
        connectingPanel.SetActive(true);
    }

    public void Hide()
    {
        connectingPanel.SetActive(false);
    }
#endif
}
