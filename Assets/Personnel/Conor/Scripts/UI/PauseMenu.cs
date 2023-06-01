using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    bool m_isPaused;
    [SerializeField] GameObject m_pauseMenu;

    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(!m_isPaused)
            {
                m_isPaused = true;
                Time.timeScale = 0;
                GameManager.Instance.StopTimer();

                m_pauseMenu.SetActive(true);
            }
            else 
            {  
                m_isPaused = false;
                Time.timeScale = 1;
                GameManager.Instance.StartTimer();

                m_pauseMenu.SetActive(false);
            }
        }
    }

    public void Play()
    {
        m_isPaused = false;
        Time.timeScale = 1;
        GameManager.Instance.StartTimer();

        m_pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        GameManager.Instance.RestartGame();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        GameManager.Instance.ClearGame();
        GameManager.Instance.ResetTimer();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}