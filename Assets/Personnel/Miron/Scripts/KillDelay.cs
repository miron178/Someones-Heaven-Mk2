using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillDelay : MonoBehaviour
{
    [SerializeField]
    float delay = 10f;

    public void KillStart()
    {
        Invoke(nameof(KillNow), delay);
    }

    public void KillCancel()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    private void KillNow()
    {
        GameObject.Destroy(gameObject);
    }
}
