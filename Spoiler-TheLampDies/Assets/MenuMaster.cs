using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	public List<TextMesh> levelTexts = new List<TextMesh>();
	public GameObject defaultStartSelection, defaultLevelSelection;

	public int unlockedLevel = 0;

	Transform selecetedObject;

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.Instance.gameState != GameState.game)
		{
			SetSelections();


		}
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
	void SetSelections()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (menuState == MainMenuState.startMenu)
				EventSystem.current.SetSelectedGameObject(defaultStartSelection);
			if (menuState == MainMenuState.levelMenu)
				EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
		}
		else if (selecetedObject != null && selecetedObject != EventSystem.current.currentSelectedGameObject.transform)
		{
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;
		}
		else if (selecetedObject == null)
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;
		
	}

	public void StartMenuStart()
	{
		unlockedLevel = PlayerPrefsManager.GetLevel();
		
		for(int i = 0; i < levelTexts.Count; i++)
		{
			if (!GameMaster.Instance.HasScene("Level" + i))
				levelTexts[i].text = "Coming \n soon!";
			else if (i > unlockedLevel)
				levelTexts[i].text = "Locked";
			else
				levelTexts[i].text ="Level \n" + i; 
		}

		StartCanvas.SetActive(false);
		levelCanvas.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
		menuState = MainMenuState.levelMenu;

	}

	public void LevelMenuBack()
	{
		levelCanvas.SetActive(false);
		StartCanvas.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultStartSelection);
		menuState = MainMenuState.startMenu;
	}

	public void LevelMenuLoadLevel(int level)
	{
		if (level <= unlockedLevel)
		{
			GameMaster.Instance.LoadLevel("Level" + level);
			GameMaster.Instance.levelNumber = level;
		}
	}

	public void MenuQuit()
	{
		Application.Quit();
	}
}