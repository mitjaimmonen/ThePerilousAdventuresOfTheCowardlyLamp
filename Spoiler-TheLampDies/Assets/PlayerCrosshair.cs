using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrosshair : MonoBehaviour {


	public GameObject defaultCrosshair;
	public GameObject lockOnCrosshair;


	bool lockOn;

	public GameObject CurrentCursorPoint(LayerMask layermask)
	{
		//Returns whatever object cursor is hovering on


		return null;
	}

	public void ClickEffect()
	{

	}

	public void LockOnEffectStart()
	{

	}
	public void LockOnEffectEnd()
	{

	}

	public Vector2 GetMouseDirectionFromPosition (Vector2 pos)
	{
		Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
		Vector2 dir = new Vector2(worldPoint.x - pos.x, worldPoint.y - pos.y);

		return dir;
	}


	// Update is called once per frame
	public void PlayerUpdate () {
		
	}
}
