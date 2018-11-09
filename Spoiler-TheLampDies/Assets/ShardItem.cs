using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardItem : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		var p = other.gameObject.GetComponent<Player>();
		if (p)
		{
			p.CollectShardItem();
			Destroy(this.gameObject);
		}
	}
}
