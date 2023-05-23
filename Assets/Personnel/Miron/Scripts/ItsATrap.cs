using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItsATrap : MonoBehaviour
{
	[SerializeField]
	private float force = 20;

	[SerializeField]
	private int damage = 1;

	[SerializeField]
	private float damageCD = 2f;
	private float damageCDEnd = 0;
	private Player player;


	[SerializeField]
	[Range(0, Mathf.Infinity)]
	private float trapDeadlySpeed = 0.1f;

	[SerializeField]
	private bool enemyPushedToKill = false;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private bool IsDeadly()
	{
		return (trapDeadlySpeed == 0) ||
			   (rb && rb.velocity.magnitude > trapDeadlySpeed);
	}


	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player") { print("Player Collision"); }
		Player player = collision.gameObject.GetComponent<Player>();
		if (player != null)
		{
			Debug.Log("YES PLAYER");
			ContactPoint cp = collision.contacts[0];
			Vector3 knock = cp.normal * force * -1;
			player.Push(knock);
		}

		if (!IsDeadly())
		{
			return;
		}

		IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
		if (damageable != null)
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			if (enemyPushedToKill && (enemy == null || !enemy.IsPushActive()))
			{
				return;
			}
			if (Time.time > damageCDEnd)
			{
				Debug.Log("YES DAMAGE");
				damageable.TakeDamage(damage);
				damageCDEnd = Time.time + damageCD;
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		OnCollisionEnter(collision);
	}
}
