using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int LevelSeed { get { return m_levelSeed; } }
    public void GenerateSeed()
    {
        //Gets a Seed from Now.Millisecond and Gets a Random Value from it
        m_levelSeed = new System.Random(DateTime.Now.Millisecond).Next();
    }

    bool m_enableDevMenu = false;
    public bool EnableDevMenu { get { return m_enableDevMenu; } }
    #endregion

    void Awake() 
    {
        if(!m_instance) { m_instance = this; DontDestroyOnLoad(this); }
        else { Destroy(this); }
    }

    void Start()
    {
        m_levelGenerator = LevelGenerator.Instance;
        m_roomGenerator = RoomGenerator.Instance;
        m_playerManager = PlayerManager.Instance;
    }

    public void StartGame()
    {
        GenerateSeed();
        m_levelGenerator.GenerateLevel();
        m_roomGenerator.GenerateTraps();
        m_roomGenerator.GeneratePowerUps();
        m_levelGenerator.GenerateNavMesh();
        m_roomGenerator.GenerateEnemies();
        m_roomGenerator.PickRandomRoom();
        m_playerManager.SpawnPlayer();
    }

    public void RestartGame()
    {
        m_playerManager.DeletePlayer();
        m_roomGenerator.ClearRoom();
        m_levelGenerator.ClearLevel();

        m_levelGenerator.GenerateLevel();
        m_roomGenerator.GenerateTraps();
        m_roomGenerator.GeneratePowerUps();
        m_levelGenerator.GenerateNavMesh();
        m_roomGenerator.GenerateEnemies();
        m_roomGenerator.PickRandomRoom();
        m_playerManager.SpawnPlayer();
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
    }

    public void ClearGame()
    {
        m_playerManager.DeletePlayer();
        m_roomGenerator.ClearRoom();
        m_levelGenerator.ClearLevel();
    }
}
