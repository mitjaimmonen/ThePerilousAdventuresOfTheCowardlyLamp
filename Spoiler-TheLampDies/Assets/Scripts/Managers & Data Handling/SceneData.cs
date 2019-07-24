using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour {

	public GameState gameState;
	public int levelNumber;
	
	public int TotalShardAmount
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (gameObject.tag != "SceneData")
			gameObject.tag = "SceneData";

		TotalShardAmount = 0;
		foreach (var i in GameObject.FindGameObjectsWithTag("ShardGround"))
		{
			ShardGround sg = i.GetComponent<ShardGround>();
			if (sg)
			{
				TotalShardAmount += sg.shardAmount * sg.shardItemPrefab.value;
			}
		}
	}
}
