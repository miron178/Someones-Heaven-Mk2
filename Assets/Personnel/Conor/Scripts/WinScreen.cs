using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject m_winScreen;

    public void RestartSameSeed()
    {
        SceneManager.LoadScene(1);

        GameManager.Instance.RestartGame();
    }

    public void RestartNewSeed()
    {
        GameManager.Instance.StartGame();    
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
