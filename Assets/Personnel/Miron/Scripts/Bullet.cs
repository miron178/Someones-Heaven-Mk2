using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage;
    public int Damage { get => damage; set => damage = value; }

    [SerializeField]
    private float speed = 10;

    [SerializeField]
    private float range = 100;

    [SerializeField]
    private Vector3 direction = Vector3.forward;
    public Vector3 Direction { get => direction; set => direction = value; }

    private Vector3 velocity = Vector3.zero;
    private Vector3 start;


    private void Start()
    {
        start = transform.position;
        velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            player.TakeDamage(damage);
        }
        GameObject.Destroy(this.gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += velocity * Time.fixedDeltaTime;
        Vector3 distance = transform.position - start;
        if (distance.sqrMagnitude > range*range)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
