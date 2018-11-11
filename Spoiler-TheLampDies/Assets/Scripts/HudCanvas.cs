using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudCanvas : MonoBehaviour {

	public GameObject hudPanel;
	public Text shardItemsText;

	private void Start()
	{
		UpdateItemCount();
	}

	public void UpdateItemCount () 
	{
		if (shardItemsText)
			shardItemsText.text = GameMaster.Instance.Player.ShardItemCount.ToString();
	}

	public void GamePaused(bool state)
	{
		if (state)
		{
			hudPanel.SetActive(false);
		}
		else
		{
			hudPanel.SetActive(true);
		}
	}
}
