using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour {

	public GameState gameState;
	public int levelNumber;

	private int totalShardAmount;
	public int TotalShardAmount
	{
		get { return totalShardAmount; }
	}

	private void Awake()
	{
		if (gameObject.tag != "SceneData")
			gameObject.tag = "SceneData";

		totalShardAmount = 0;
		foreach (var i in GameObject.FindGameObjectsWithTag("ShardGround"))
		{
			ShardGround sg = i.GetComponent<ShardGround>();
			if (sg)
			{
				totalShardAmount += sg.shardAmount * sg.shardItemPrefab.value;
			}
		}
	}
}
