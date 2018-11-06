using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	public Transform targetTrans;
	public float distance;
	public float lerpSpeed;

	private Vector3 pos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (targetTrans)
		{
			pos =  targetTrans.position - Vector3.forward*distance;
			transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime*lerpSpeed);
		}
	}
}
