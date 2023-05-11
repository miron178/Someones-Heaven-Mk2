using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTrap : Pushable
{
    [SerializeField]
    private Rigidbody dropTrap;

    public override void Push(Vector3 force)
    {
        dropTrap.isKinematic = false;
        GameObject.Destroy(this.gameObject);
    }
}
