using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject m_winScreen;

    void Awake()
    {
        // m_winScreen.SetActive(false);
    }

    public void RestartSameSeed()
    {
        SceneManager.LoadScene(1);

        GameManager.Instance.RestartGame();
    }

    public void RestartNewSeed()
    {
        SceneManager.LoadScene(1);
        GameManager.Instance.StartGame();    
    }

    public void MainMenu()
    {
        GameManager.Instance.ResetTimer();
        SceneManager.LoadScene(0);
    }
}
