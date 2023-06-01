using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameObject m_buttons;
    [SerializeField] GameObject m_seedPanel;
    [SerializeField] TMPro.TMP_InputField m_seedInput;

    public void StartGameButton()
    {
        m_buttons.SetActive(false);
        m_seedPanel.SetActive(true);
    }

    public void UseSeed()
    {
        GameManager.Instance.LevelSeed = System.Int32.Parse(m_seedInput.text);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void UseRandomSeed()
    {
        GameManager.Instance.GenerateSeed();

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
