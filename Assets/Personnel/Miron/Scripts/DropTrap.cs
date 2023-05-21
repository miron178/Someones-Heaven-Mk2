using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTrap : MonoBehaviour, IPushable
{
    [SerializeField]
    private Rigidbody dropTrap;

    public void Push(Vector3 force)
    {
        dropTrap.isKinematic = false;
        GameObject.Destroy(this.gameObject);
    }
}
