using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActivation : MonoBehaviour
{
	[SerializeField]
	private GameObject EnemyObjects;

	private void Awake() {
		EnemyObjects.SetActive(false);
	}

	private void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			EnemyObjects.SetActive(true);
		}
		
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			EnemyObjects.SetActive(false);
		}
	}
}
