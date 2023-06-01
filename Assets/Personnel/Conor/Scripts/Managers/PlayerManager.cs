using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager m_instance;
    public static PlayerManager Instance { get { return m_instance; } }

    [SerializeField] GameObject m_playerPrefab;
    GameObject m_player;
    public void SpawnPlayer() { m_player = Instantiate(m_playerPrefab, new Vector3(0,1,0), Quaternion.Euler(0, -45, 0)); }
    public void DeletePlayer() { Destroy(m_player); m_player = null; }

    void Awake()
    {
        if(!m_instance) { m_instance = this; }
        else { Destroy(this); }
    }
}