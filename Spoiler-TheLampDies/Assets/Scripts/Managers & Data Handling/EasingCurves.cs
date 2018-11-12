using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Handy little script to apply easing to time.

 */

public class EasingCurves {

	public enum Curve
	{
		linear,
		exponentialIn,
		exponentialOut,
		easeIn,
		easeOut,
		easeInOut,
		one,
		zero
	}

	public static float Easing(float time, Curve curve)
	{
		//Useful little function to calculate easing automatically.

		switch (curve)
		{
			case Curve.one :
				return 1f;
			case Curve.zero :
				return 0;
			case Curve.linear :
				return time;
			case Curve.exponentialIn :
				return time*time;
			case Curve.exponentialOut :
				return time + (time - (time*time));
			case Curve.easeIn :
				return 1f - Mathf.Cos(time * Mathf.PI * 0.5f);
			case Curve.easeOut :
				return time = Mathf.Sin(time * Mathf.PI * 0.5f);
			case Curve.easeInOut :
				return time*time * (3f - 2f*time);
			default:
				return 1f;
		}
}
}
