using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
	private void OnTriggerEnter(Collider other) {
		Destroy(gameObject);
	}
}
