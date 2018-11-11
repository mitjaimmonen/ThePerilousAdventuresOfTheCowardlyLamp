using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum MainMenuState
{
	startMenu,
	levelMenu
}

public class MenuMaster : MonoBehaviour {

	public MainMenuState menuState;
	public GameObject StartCanvas;
	public GameObject levelCanvas;
	public List<Button> levelButtons = new List<Button>();

	public int unlockedLevel = 0;

	Transform selecetedObject;

	// Use this for initialization
	void Start () {
		Initialize();
	}

	public void Initialize()
	{
		levelCanvas.SetActive(false);
		StartCanvas.SetActive(true);
		Reset();
	}

	public void Reset()
	{
		StartCanvas.gameObject.SetActive(true);
		levelCanvas.gameObject.SetActive(false);
		unlockedLevel = PlayerPrefsManager.GetLevel();
	}

	public void StartMenuStart()
	{
		unlockedLevel = PlayerPrefsManager.GetLevel();
		
		for(int i = 0; i < levelButtons.Count; i++)
		{
			if (!GameMaster.Instance.HasScene("Level" + i)) // No scene in build
				levelButtons[i].GetComponentInChildren<Text>().text = "UNDER CONSTRUCTION!";
			else if (i > unlockedLevel) //Not unlocked yet
				levelButtons[i].GetComponentInChildren<Text>().text = "LOCKED";
			else if (i == 0) //First level = tutorial
				levelButtons[i].GetComponentInChildren<Text>().text = "TUTORIAL";
			else
			{
				levelButtons[i].interactable = true; //Activate button.
				levelButtons[i].GetComponentInChildren<Text>().text ="LEVEL " + i; 
				levelButtons[i].GetComponentInChildren<Text>().color = Color.white;
			}
		}

		StartCanvas.SetActive(false);
		levelCanvas.SetActive(true);
		menuState = MainMenuState.levelMenu;

	}

	public void LevelMenuBack()
	{
		levelCanvas.SetActive(false);
		StartCanvas.SetActive(true);
		menuState = MainMenuState.startMenu;
	}

	public void LevelMenuLoadLevel(int level)
	{
		if (level <= unlockedLevel)
		{
			GameMaster.Instance.LoadLevel("Level" + level);
			GameMaster.Instance.LevelNumber = level;
		}
	}

	public void MenuQuit()
	{
		Application.Quit();
	}
}