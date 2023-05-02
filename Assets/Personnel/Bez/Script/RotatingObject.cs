using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
	[SerializeField]
	private int rotationspeed;

	private void FixedUpdate() {
		transform.Rotate(0, rotationspeed, 0, Space.World);
	}
}
