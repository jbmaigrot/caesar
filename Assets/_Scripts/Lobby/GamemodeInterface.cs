using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamemodeInterface : MonoBehaviour
{
	public GameObject SoloPanel;
	public GameObject ServerPanel;
	public GameObject ClientPanel;
	public GameObject goServerLobby;
	public GameObject goClientLobby;


	public void SelectServer()
	{
		GameState.CLIENT = true;
		GameState.SERVER = true;

		goClientLobby.SetActive(true);
		goServerLobby.SetActive(true);
		//ServerPanel.SetActive(true);
		ClientPanel.SetActive(true);

		gameObject.SetActive(false);
	}


	public void SelectClient()
	{
		GameState.CLIENT = true;

		goClientLobby.SetActive(true);
		ClientPanel.SetActive(true);

		gameObject.SetActive(false);
	}


	public void SelectSolo()
	{
		GameState.CLIENT = true;
		GameState.SERVER = true;

		SoloPanel.SetActive(true);

		SceneManager.LoadScene(1);
	}
}
