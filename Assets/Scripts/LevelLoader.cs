using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public int LevelIndex = 1;

    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelIndex);
    }
}