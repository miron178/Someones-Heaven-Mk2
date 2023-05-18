using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCore : MonoBehaviour
{
	public int effectIncrease;
	public int effectTimeOut;

	public bool isHealth = false;
	public bool isSpeed = false;
	public bool isDefence = false;

	public GameObject pickupEffect;

	public GameObject playerObject;

	public Player player;

	public void Update() {
		if(player == null) 
		{ 
			playerObject = GameObject.FindGameObjectWithTag("Player");

			if(playerObject != null) { player = playerObject.GetComponent<Player>(); }
		}
	}

	//private void OnTriggerEnter(Collider other) {
	//	if (other.CompareTag("Player")) {
	//		PickUp(other);
	//	}
	//}

	public void PickUp() {

		Debug.Log("player");

		Instantiate(pickupEffect, transform.position, transform.rotation);
		
		if (isSpeed) {
			SpeedUp();
		}
		if (isHealth) {
			HealthUp();
		}
		if (isDefence) {
			DefenceUp();
		}
		DestroyObject();
	}

	public void SpeedUp() {
		player.speed += effectIncrease;
		Invoke("ResetStats", effectTimeOut);
	}

	public void DefenceUp() {
		player.canDamage = false;
		Invoke("ResetStats", effectTimeOut);
	}

	public void HealthUp() {
		player.health += effectIncrease;
	}

	private void DestroyObject() {
		Destroy(gameObject);
	}
}
