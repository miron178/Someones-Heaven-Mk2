using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject m_deathScreen;

    void Awake()
    {
        m_deathScreen.SetActive(false);
    }
}
