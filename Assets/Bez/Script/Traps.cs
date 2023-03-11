using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
	public Player player;
	
	
	

	public void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			player.TakeDamage(1);
			Debug.Log("takedamage");
		}
		
	}
}
