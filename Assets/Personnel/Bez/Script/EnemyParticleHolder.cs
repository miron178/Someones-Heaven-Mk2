using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleHolder : MonoBehaviour
{

	private bool particle1;
	private bool particle2;
	private bool particle3;
	public int timeout;

	public GameObject ParticleObject1;

	public GameObject ParticleObject2;

	public GameObject ParticleObject3; 

	private void Update() {
		//if (defenceBubble) {
		//	DefenceBubble.SetActive(true);
		//}
		//if (speedBoost) {
		//	SpeedBoost.SetActive(true);
		//}
		//if (healthBoost) {
		//	HealthUp.SetActive(true);
		//}
	}

	public void ParticlesOn() {
		ParticleObject1.SetActive(true);
		ParticleObject2.SetActive(true);
		ParticleObject3.SetActive(true);

		particle1 = true;
		particle2 = true;
		particle3 = true;
		Invoke("ParticlesOff", 1);
	}

	public void ParticlesOff() {
		ParticleObject1.SetActive(false);
		ParticleObject2.SetActive(false);
		ParticleObject3.SetActive(false);

		particle1 = false;
		particle2 = false;
		particle3 = false;
	}
}
