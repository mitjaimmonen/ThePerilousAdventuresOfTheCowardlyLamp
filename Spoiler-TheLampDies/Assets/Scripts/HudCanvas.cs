using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudCanvas : MonoBehaviour {

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
}
