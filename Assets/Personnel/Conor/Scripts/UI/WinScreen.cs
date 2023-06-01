using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    [SerializeField] GameObject m_winScreen;
    [SerializeField] TMP_InputField m_name;
    [SerializeField] GameObject m_button;
    [SerializeField] GameObject m_submitted;

    void Awake()
    {
        // m_winScreen.SetActive(false);
    }

    public void SubmitScoreButton() 
    {
        if(m_name.text == "") { return; }
        StartCoroutine(SubmitScore());

        m_name.gameObject.SetActive(false);
        m_button.SetActive(false);
        m_submitted.SetActive(true);
    }

    IEnumerator SubmitScore()
    {
        SubmitJSON jsonData = new SubmitJSON();
        jsonData.name = m_name.text;
        jsonData.seed = GameManager.Instance.LevelSeed;
        jsonData.time = GameManager.Instance.CurrentTimeFloat;

        WWWForm form = new WWWForm();
        form.AddField("name", m_name.text);
        form.AddField("seed", GameManager.Instance.LevelSeed);
        form.AddField("time", GameManager.Instance.CurrentTimeFloat.ToString());

        UnityWebRequest www = UnityWebRequest.Post("https://halfempty.conorlewis.com/submitTime", form);
        // www.SetRequestHeader("content-type", "application/json");

        yield return www.SendWebRequest();
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

[Serializable]
struct SubmitJSON
{
    public string name;
    public int seed;
    public float time;
}