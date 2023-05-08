using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCore : MonoBehaviour
{

	public float multiplier = 1.4f; 

	public GameObject pickupEffect;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			PickUp(other);
		}
	}

	void PickUp(Collider player) {

		Debug.Log("player");

		Instantiate(pickupEffect, transform.position, transform.rotation);

		//this.gameObject.transform.localScale;

		player.transform.localScale *= multiplier;

		DestroyObject();
	}

	private void DestroyObject() {
		Destroy(gameObject);
	}
}
