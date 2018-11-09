using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
	void GetHit(float dmg);
	void GetHit(float dmg, Vector2 pos);
}
