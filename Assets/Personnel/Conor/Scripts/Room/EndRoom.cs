using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndRoom : MonoBehaviour
{
    [SerializeField] GameObject m_winScreen;
    [SerializeField] TMP_Text m_winTimerText;

    private void Awake()
    {
        m_winScreen = GameObject.FindGameObjectWithTag("WinScreen");
        m_winTimerText = GameObject.FindGameObjectWithTag("WinTimerText").GetComponent<TMP_Text>();

        m_winScreen.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.Instance.StopTimer();
            GameManager.Instance.ClearGame();

            m_winTimerText.text = GameManager.Instance.CurrentTime;
            m_winScreen.SetActive(true);
        }
    }
}
