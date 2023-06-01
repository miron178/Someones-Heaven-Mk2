using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathField : MonoBehaviour
{
    [SerializeField]
    private Transform m_destination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            other.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = new Vector3(0, 1, 0);
            other.GetComponent<CharacterController>().enabled = true;

            other.GetComponent<Player>().TakeDamage(1);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
