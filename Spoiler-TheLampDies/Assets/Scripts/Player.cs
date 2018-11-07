using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject visuals;

	public PlayerControl controller;
	public PlayerCrosshair crosshair;
	public PlayerDashAttack dashAttack;

	// Use this for initialization
	private void Awake () {
		controller = GetComponent<PlayerControl>();
		crosshair = GetComponent<PlayerCrosshair>();
		dashAttack = GetComponent<PlayerDashAttack>();
	}

	public void MasterUpdate()
	{
		//Decides which script updates run first.
		crosshair.PlayerUpdate();
		controller.PlayerUpdate();
		dashAttack.PlayerUpdate();
	}

}
