using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
	private GameObject movingObject;

	public float speed;

	private void FixedUpdate() {

		movingObject.transform.position += transform.forward * speed * Time.deltaTime;
	}
}
