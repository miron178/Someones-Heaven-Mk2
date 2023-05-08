using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRoom : MonoBehaviour
{
    [SerializeField] GameObject winScreen;

    private void Awake()
    {
        winScreen = GameObject.FindGameObjectWithTag("WinScreen");
        winScreen.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.Instance.ClearGame();

            winScreen.SetActive(true);
        }
    }
}
