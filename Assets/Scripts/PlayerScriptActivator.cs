using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptActivator : MonoBehaviour
{
	public Player player;

	void Start()
    {
		player = GetComponent<Player>();
		player.enabled = !player.enabled;
	}

    
}
