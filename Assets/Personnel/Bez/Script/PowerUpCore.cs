using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCore : MonoBehaviour
{

	public float multiplier = 1.4f;

	public int effectIncrease;
	public int effectTimeOut;

	public bool isHealth = false;
	public bool isSpeed = false;
	public bool isDefence = false;

	public int originalInt;
	public float originalFloat;

	public GameObject pickupEffect;

	public GameObject playerObject;

	public Player player;

	public void Awake() {
		playerObject = GameObject.FindGameObjectWithTag("Player");
		player = playerObject.GetComponent<Player>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			PickUp(other);
		}
	}

	void PickUp(Collider player) {

		Debug.Log("player");

		Instantiate(pickupEffect, transform.position, transform.rotation);

		//this.gameObject.transform.localScale;

		//player.transform.localScale *= multiplier;

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
		originalFloat = player.speed;
		player.speed += effectIncrease;
		Invoke("ResetStats", effectTimeOut);
	}

	public void DefenceUp() {
		player.canDamage = false;
		Invoke("ResetStats", effectTimeOut);
	}

	public void HealthUp() {
		originalInt = player.health;
		player.health += effectIncrease;
	}

	public void ResetStats() {
		if (isSpeed) {
			player.speed = originalFloat;
		}
		if (isDefence) {
			player.canDamage = true;
		}
	}

	private void DestroyObject() {
		Destroy(gameObject);
	}
}
