using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	public float distance;
	public float offsetV;
	public float lerpSpeed;

	private Vector3 pos;
	private Transform targetTrans;

	public CameraShake CameraShake
	{
		get 
		{
			return GetComponent<CameraShake>();
		}
	}

	// Use this for initialization
	void Start () {
		targetTrans = GameMaster.Instance.Player.transform;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (targetTrans)
		{
			pos =  targetTrans.position - Vector3.forward*distance;
			pos.y += offsetV;
			transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime*lerpSpeed);
		}
	}
}
