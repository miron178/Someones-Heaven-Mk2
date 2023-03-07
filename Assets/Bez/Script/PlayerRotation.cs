using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{

	[SerializeField]
	private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        
    }

	// Update is called once per frame
	void FixedUpdate() {
		if (Input.GetKeyDown(KeyCode.W)) {
			player.transform.rotation.Set(-90, 0, 0, 0);
		}
	}
}
