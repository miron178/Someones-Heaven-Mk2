using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHolder : MonoBehaviour
{

	private bool defenceBubble;
	private bool speedBoost;
	private bool healthBoost;

	public GameObject DefenceBubble;

	public GameObject SpeedBoost;

	public GameObject HealthUp;



	private void Update() {
		if (defenceBubble) {
			DefenceBubble.SetActive(true);
		}
		if (speedBoost) {
			SpeedBoost.SetActive(true);
		}
		if (healthBoost) {
			HealthUp.SetActive(true);
		}
	}

	public void DefenceBubbleOn() {
		defenceBubble = true;
		DefenceBubble.SetActive(true);
	}

	public void DefenceBubbleOff() {
		defenceBubble = false;
		DefenceBubble.SetActive(false);
	}

	public void SpeedBoostOn() {
		speedBoost = true;
		SpeedBoost.SetActive(true);
	}

	public void SpeedBoostOff() {
		speedBoost = false;
		SpeedBoost.SetActive(false);
	}

	public void HealthUpOn() {
		healthBoost = true;
		HealthUp.SetActive(true);
	}

	public void HealthUpOff() {
		healthBoost = false;
		HealthUp.SetActive(false);
	}
}
