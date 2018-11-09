using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	public GameObject visuals;

	[HideInInspector]public PlayerControl controller;
	[HideInInspector]public PlayerCrosshair crosshair;
	[HideInInspector]public PlayerDashAttack dashAttack;
	[HideInInspector]public PlayerProjectileAttack projectileAttack;

	private int shardItemCount = 0;
	public int ShardItemCount
	{
		get { return shardItemCount; }
		set
		{
			shardItemCount = value;
			GameMaster.Instance.GameCanvas.HudCanvas.UpdateItemCount();
		}
	}

	// Use this for initialization
	private void Awake () {
		controller = GetComponent<PlayerControl>();
		crosshair = GetComponent<PlayerCrosshair>();
		dashAttack = GetComponent<PlayerDashAttack>();
		projectileAttack = GetComponent<PlayerProjectileAttack>();
	}

	public void MasterUpdate()
	{
		//Decides which script updates run first.
		crosshair.PlayerUpdate();
		controller.PlayerUpdate();
		projectileAttack.PlayerUpdate();
		dashAttack.PlayerUpdate();
	}

	public void GetHit(float dmg)
	{

	}

	public void CollectShardItem()
	{
		++ShardItemCount;
	}

}
