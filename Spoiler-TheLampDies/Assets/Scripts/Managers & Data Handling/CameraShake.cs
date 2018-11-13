using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public Transform cameraTrans;
	public int maxShakes = 5;
	public float maxMagnitude = 2.5f;
	public float maxFrequency = 15f;
	public float maxDuration = 2f;
	public float maxDelay = 1f;

	private int shakes;


	private void Awake()
	{
		cameraTrans = GetComponentInChildren<Camera>().transform;
	}

	/// <param name="magnitude">Strength of shake. Is affected by easing curve.</param>
	/// <param name="frequency">Speed of shake in mHz (1 wave per second).</param>
	/// <param name="easingCurve">How should shake behave over time. </param>
	public void StartShake(float magnitude, float frequency, EasingCurves.Curve easingCurve, float duration, float delay)
	{
		//Too many overlapping shakes look bad.
		if (shakes < maxShakes)
		{
			shakes++;
			//Set passed values within maximum values
			//Coroutine will play the duration of shake.
			//This way multiple shakes can overlap each other.
			StartCoroutine(Shake(Mathf.Min(magnitude, maxMagnitude),Mathf.Min(maxFrequency, frequency), easingCurve,Mathf.Min(maxDuration, duration), Mathf.Min(maxDelay, delay)));
		}
	}

	
	private IEnumerator Shake(float magnitude, float frequency, EasingCurves.Curve easingCurve, float duration, float delay)
	{
		if (delay > 0)
			yield return new WaitForSeconds(delay);
			
		float startTime= Time.time;
		float rotationSpeed = Random.Range(-100f,100f) * frequency;

		float magnitudeMultiplier = 0;
		Vector3 offset = Vector3.zero;
		Vector3 lastOffset = Vector3.zero;
		Vector3 newOffset = Vector3.zero;
		float t = 0;

		//Add a little bit of variation/randomness to values
		Vector2 curMagnitude = new Vector2(Random.Range(0.9f,1.1f) * magnitude, Random.Range(0.9f,1.1f) * magnitude);
		//Randomize start direction
		curMagnitude = Random.insideUnitCircle.normalized * curMagnitude; 

		//Multiply frequency with duration for shake to ignore time.
		Vector2 curFrequency = new Vector2(Random.Range(0.9f,1.1f) * (frequency * duration), Random.Range(0.9f,1.1f) * (frequency*duration));

		//Loops shaking until time > duration
		while (startTime > Time.time - duration)
		{
			//Use last offset to allow shake overlapping.
			lastOffset = offset;

			t = (Time.time - startTime)/duration;
			magnitudeMultiplier = 1f - EasingCurves.Easing(t, easingCurve);

			//"Randomizes" direction of the shake in real time.
			curMagnitude = Rotate(curMagnitude, rotationSpeed*Time.deltaTime);

			//Sine waves make a great shake with proper handling of frequency and magnitude.
			//Magnitude multiplayer makes the shake to ease away smoothly.
			offset.x = Mathf.Sin(2f*Mathf.PI * t * curFrequency.x + 0) * (curMagnitude.x*magnitudeMultiplier);
			offset.y = Mathf.Sin(2f*Mathf.PI * t * curFrequency.y + 0) * (curMagnitude.y*magnitudeMultiplier);


			//Apply offset to camera object under camera container to not interfere with global camera positioning.
			newOffset = offset - lastOffset;
			cameraTrans.localPosition += newOffset;

			
			yield return null;
		}

		shakes--;
		//If last shake, reset offset.
		if (shakes < 1)
		{
			Vector3 oldPos = cameraTrans.localPosition;
			float timer = 0;
			while (cameraTrans.localPosition != Vector3.zero && shakes < 1)
			{
				timer += Time.deltaTime;
				//Smoothly sets position back to zero in case it is not already.
				cameraTrans.localPosition = Vector3.Lerp(oldPos, Vector3.zero, timer*2f); //Uses 0,5 seconds.

				yield return null;
			}
		}


		yield break;
	}


	private Vector2 Rotate(Vector2 v, float degrees) 
	{
		//Rotates vector by degree amount.

         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);

         return v;
    }

}
