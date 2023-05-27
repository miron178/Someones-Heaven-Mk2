using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBar : MonoBehaviour
{
    Player player;
    [SerializeField]
    GameObject frontBar;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void FixedUpdate()
    {
        float max = player.PushForceMax;
        float current = player.PushForceCurrent;

        frontBar.transform.localScale = new Vector3(current / max, 1, 1);
    }
}
