using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
	public Player player;
	
	
	
	// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnTriggerEnter(Collider other) {
		player.TakeDamage(1);
		Debug.Log("takedamage");
	}
}
