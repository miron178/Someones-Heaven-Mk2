using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(LevelGenerator))]
[RequireComponent(typeof(RoomGenerator))]
[RequireComponent(typeof(PlayerManager))]
//[RequireComponent(typeof(DearIMGUI))]
public class GameManager : MonoBehaviour
{
    #region Variables
    private static GameManager m_instance; // Singleton instance
    public static GameManager Instance { get { return m_instance; } }

    LevelGenerator m_levelGenerator;
    RoomGenerator m_roomGenerator;
    PlayerManager m_playerManager;

    //Random and Seed
    System.Random m_randomGenerator;
    public System.Random RandomGenerator 
    { 
        get 
        {
            //Init Random Engine using Seed
            m_randomGenerator = new System.Random(m_levelSeed); 
            return m_randomGenerator;
        } 
    }
    public System.Random RandomGeneratorSame { get { return m_randomGenerator; } }

    int m_levelSeed = 4444;
    public int LevelSeed { get { return m_levelSeed; } set  { m_levelSeed = value; }}
    public void GenerateSeed()
    {
        //Gets a Seed from Now.Millisecond and Gets a Random Value from it
        m_levelSeed = new System.Random(DateTime.Now.Millisecond).Next();
    }

    bool m_enableDevMenu = false;
    public bool EnableDevMenu { get { return m_enableDevMenu; } }

    bool m_timerRun = false;
    public void StopTimer() { m_timerRun = false; }
    public float CurrentTimeFloat { get { return m_timer; } }
    public string CurrentTimeString { get { return m_minutes.ToString("00") + ":" + m_seconds.ToString("00") + "." + m_milliseconds.ToString("00"); } }
    public void ResetTimer() { m_milliseconds = 0; m_seconds = 0; m_minutes = 0; m_timer = 0; }

    float m_milliseconds = 0;
    float m_seconds = 0;
    float m_minutes = 0;
    float m_timer;
    TMP_Text timerText;
    TMP_Text m_seedText;
    #endregion

    void Awake() 
    {
        if(!m_instance) { m_instance = this; DontDestroyOnLoad(this); }
        else { Destroy(this.gameObject); }
    }

    void Start()
    {
        m_levelGenerator = LevelGenerator.Instance;
        m_roomGenerator = RoomGenerator.Instance;
        m_playerManager = PlayerManager.Instance;
    }

    public void StartGame()
    {
        m_levelGenerator.GenerateLevel();
        m_roomGenerator.GenerateTraps();
        m_roomGenerator.GeneratePowerUps();
        m_roomGenerator.SpawnEndDoor();
        m_levelGenerator.GenerateNavMesh();
        m_roomGenerator.GenerateEnemies();
        m_playerManager.SpawnPlayer();
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        GameObject.FindGameObjectWithTag("SeedText").GetComponent<TMP_Text>().text = "Seed: " + m_levelSeed.ToString();
        m_timerRun = true;
    }

    public void RestartGame()
    {
        m_playerManager.DeletePlayer();
        m_roomGenerator.ClearRoom();
        m_levelGenerator.ClearLevel();

        m_timer = 0;
        m_minutes = 0;
        m_seconds = 0;
        m_milliseconds = 0;

        m_levelGenerator.GenerateLevel();
        m_roomGenerator.GenerateTraps();
        m_roomGenerator.GeneratePowerUps();
        m_levelGenerator.GenerateNavMesh();
        m_roomGenerator.GenerateEnemies();
        m_playerManager.SpawnPlayer();
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        m_timerRun = true;
    }

    public void NextLevel()
    {
        m_playerManager.DeletePlayer();
        m_roomGenerator.ClearRoom();
        m_levelGenerator.ClearLevel();

        m_levelGenerator.GenerateLevel(true);
        m_roomGenerator.GenerateTraps(true);
        m_roomGenerator.GeneratePowerUps(true);
        m_levelGenerator.GenerateNavMesh();
        m_roomGenerator.GenerateEnemies(true);
        m_playerManager.SpawnPlayer();
        timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TMP_Text>();
        m_timerRun = true;
    }

    public void ClearGame()
    {
        m_playerManager.DeletePlayer();
        m_roomGenerator.ClearRoom();
        m_levelGenerator.ClearLevel();
    }

    void FixedUpdate()
    {
        if(m_timerRun)
        {
            m_timer += Time.deltaTime;

            m_minutes = MathF.Floor(m_timer / 60f);
            m_seconds = MathF.Floor(m_timer % 60f);
            m_milliseconds = (m_timer % 1) * 100;

            timerText.text = m_minutes.ToString("00") + ":" + m_seconds.ToString("00") + ":" + m_milliseconds.ToString("00");
        }
    }
}
