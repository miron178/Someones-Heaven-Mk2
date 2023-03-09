using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
	public GameObject rotatingObject;

	public int rotationspeed;

	private void Awake() {
		rotatingObject = this.gameObject;
	}
	private void FixedUpdate() {
		rotatingObject.transform.Rotate(0, rotationspeed, 0, Space.World);
	}
}
