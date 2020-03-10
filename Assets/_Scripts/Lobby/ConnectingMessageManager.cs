using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingMessageManager : MonoBehaviour
{
    public GameObject connectingPanel;

    public void Show()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		connectingPanel.SetActive(true);
    }

    public void Hide()
	{
		if (!GameState.CLIENT) return; // replacement for preprocessor

		connectingPanel.SetActive(false);
    }
}
