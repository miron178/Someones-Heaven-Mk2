using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEffect : MonoBehaviour
{
	//private GameObject Effect;

	private void Start() {
		//Effect = this.gameObject;
		Invoke("DestroyEffect", 3);
	}

	private void DestroyEffect() {
		Destroy(gameObject);
	}
}
