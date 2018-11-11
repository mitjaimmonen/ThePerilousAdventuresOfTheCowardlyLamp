using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PauseType
{
	paused,
	won
}

public class PauseMenuCanvas : MonoBehaviour {

	public GameObject menuPanel; //Main panel containing others
	public GameObject pausePanel;
	public GameObject finishPanel;
	public Text shardItemText;

	public void GamePaused(bool state, PauseType pauseType)
	{
		//All sub-panels inactive first.
		pausePanel.SetActive(false);
		finishPanel.SetActive(false);
		
		if (state)
		{
			//Set corresponding panel active.
			switch (pauseType)
			{
				case PauseType.paused :
					pausePanel.SetActive(true);
				break;
				case PauseType.won :
					finishPanel.SetActive(true);
				break;
			}

			//Main panel active
			menuPanel.SetActive(true);
		}
		else
		{
			menuPanel.SetActive(false);
		}
	}

	public void Continue()
	{
		if (!GameMaster.Instance.IsFinished)
			GameMaster.Instance.IsPaused = false;
	}

	public void ToMenu()
	{
		GameMaster.Instance.LoadLevel("MainMenu");
	}

	public void NextLevel()
	{
		GameMaster.Instance.NextLevel();
	}

	public void Restart()
	{
		GameMaster.Instance.LoadLevel(GameMaster.Instance.GetCurrentSceneName());
	}

	public void UpdateItemCount()
	{
		shardItemText.text = GameMaster.Instance.Player.ShardItemCount.ToString();
		shardItemText.text += " / " + GameMaster.Instance.CurrentSceneData.TotalShardAmount.ToString();
	}
}
