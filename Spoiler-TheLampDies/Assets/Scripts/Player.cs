using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	[Tooltip("Reference to gameObject containing player mesh & textures.")]
	public GameObject visuals;
	[Tooltip("Max distance from where player attracts items.")]
	public float magnetDistance;

	[HideInInspector]public PlayerControl control;
	[HideInInspector]public PlayerCrosshair crosshair;
	[HideInInspector]public PlayerDashAttack dashAttack;
	[HideInInspector]public PlayerProjectileAttack projectileAttack;


	public Rigidbody2D rb
	{
		get { return GetComponent<Rigidbody2D>(); }
	}

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
		control = GetComponent<PlayerControl>();
		crosshair = GetComponent<PlayerCrosshair>();
		dashAttack = GetComponent<PlayerDashAttack>();
		projectileAttack = GetComponent<PlayerProjectileAttack>();
	}

	public void MasterUpdate()
	{
		//Decides which script updates run first.
		crosshair.PlayerUpdate();
		control.PlayerUpdate();
		projectileAttack.PlayerUpdate();
		dashAttack.PlayerUpdate();
	}

	public void GetHit(float dmg)
	{

	}
	public void GetHit(float dmg, Vector2 pos)
	{

	}
	public void CollectShardItem()
	{
		++ShardItemCount;
	}

}
