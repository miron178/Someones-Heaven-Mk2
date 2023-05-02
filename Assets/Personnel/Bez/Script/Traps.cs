using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
	[SerializeField]
	private float force = 20;

	[SerializeField]
	private int damage = 1;

	[SerializeField]
	private float damageCD = 2f;
	private float damageCDEnd = 0;
	private Player player;

	private void OnCollisionEnter(Collision collision)
    {
		if (collision.collider.tag != "Player")
			return;

		if (player == null)
			player = collision.gameObject.GetComponent<Player>();

		ContactPoint cp = collision.contacts[0];

		Vector3 pull = cp.normal * force * -1;
		player.AddPull(pull);

		if (Time.time > damageCDEnd)
		{
			player.TakeDamage(damage);
			damageCDEnd = Time.time + damageCD;
		}
	}
}
