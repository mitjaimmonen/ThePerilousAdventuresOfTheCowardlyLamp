using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {

	public GameObject visuals;

	[HideInInspector]public PlayerControl controller;
	[HideInInspector]public PlayerCrosshair crosshair;
	[HideInInspector]public PlayerDashAttack dashAttack;
	[HideInInspector]public PlayerProjectileAttack projectileAttack;

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

}
