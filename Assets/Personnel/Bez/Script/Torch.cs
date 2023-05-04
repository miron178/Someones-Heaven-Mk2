using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{

	public Volume volume;
	public Vignette vignette;
	public GameObject torch1;
	public GameObject torch2;
	public GameObject torch3;

	public GameObject currentTorch;

	public int torchLevel;
	public int maxTorchLevel;
	//public int torchQuantity;

	public bool hasTorch;

	public Enemy enemy;

	public void Awake() {
		ChooseTorch();
		currentTorch.SetActive(true);
		//volume.profile.TryGet(out vignette);
	}

	public void AddLevel() {
		PutAway();
		torchLevel++;
		ChooseTorch();
		//SetVignette();
	}

	public void ChooseTorch() {
		if (torchLevel <= 2) {
			if (torchLevel == 0) {
				currentTorch = torch1;
				Invoke("Weild", 0.1f);
			}
			if (torchLevel == 1) {
				currentTorch = torch2;
				Invoke("Weild", 0.1f);
			}
			if (torchLevel >= 1) {
				currentTorch = torch3;
				Invoke("Weild", 0.1f);
			}
		}		
	}

	//public void SetVignette() {
	//	if (torchLevel == 0) {
	//		vignette.intensity.value = 1f;
	//	}
	//	if (torchLevel == 1) {
	//		vignette.intensity.value = 0.4f;
	//	}
	//	if (torchLevel >= 1) {
	//		vignette.intensity.value = 0.2f;
	//	}
	//}

	public void Weild() {
		currentTorch.SetActive(true);
	}

	public void PutAway() {
		currentTorch.SetActive(false);
	}

	public void FixedUpdate() {
		if (Keyboard.current[Key.B].wasReleasedThisFrame) {
			AddLevel();
		}
		if (Keyboard.current[Key.N].wasReleasedThisFrame) {
			Weild();
		}
		if (Keyboard.current[Key.M].wasReleasedThisFrame) {
			PutAway();
		}
	}
}
