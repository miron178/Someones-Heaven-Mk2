using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActivation : MonoBehaviour
{
	List<GameObject> m_enemies;
	public void AddEnemy(GameObject enemy) { m_enemies.Add(enemy); }

	List<Vector3> m_enemyPositions;
	public void AddEnemyPosition(Vector3 enemyPos) { m_enemyPositions.Add(enemyPos); }

	void Awake() { m_enemies = new List<GameObject>(); m_enemyPositions = new List<Vector3>(); }

	public void DisableEnemies() 
	{
		for(int i = 0; i < m_enemies.Count; i++)
		{
			m_enemies[i].SetActive(false);
			m_enemies[i].transform.position = m_enemyPositions[i];
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Player") 
		{
			for(int i = 0; i < m_enemies.Count; i++)
			{
				m_enemies[i].SetActive(true);
				m_enemies[i].transform.position = m_enemyPositions[i];
			}
		}	
	}

	void OnTriggerExit(Collider other) 
	{
		if (other.tag == "Player") 
		{
			for(int i = 0; i < m_enemies.Count; i++)
			{
				m_enemies[i].SetActive(false);
				m_enemies[i].transform.position = m_enemyPositions[i];
			}
		}
	}
}
