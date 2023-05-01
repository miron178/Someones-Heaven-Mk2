using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    private static RoomGeneration m_instance;
    public static RoomGeneration Instance { get { return m_instance; } }

    [Header("Generation Setting", order = 1)]
    //Traps
    [SerializeField] GameObject m_trapParent;

    [SerializeField] bool m_setTrapsAmount = false;
    public bool SetTrapAmount { get { return m_setTrapsAmount; } set { m_setTrapsAmount = value; } }

    [SerializeField] int m_trapAmount = 1;
    public int TrapAmount { get { return m_trapAmount; } set { m_trapAmount = value; } }

    [SerializeField] int m_minTrapAmount = 1;
    public int MinTrapAmount { get { return m_minTrapAmount; } set { m_minTrapAmount = value; } }
    
    [SerializeField] int m_maxTrapAmount = 3;
    public int MaxTrapAmount { get { return m_maxTrapAmount; } set { m_maxTrapAmount = value; } }

    //Enemies
    [SerializeField] GameObject m_enemyParent;

    [SerializeField] bool m_setEnemyAmount = false;
    public bool SetEnemyAmount { get { return m_setEnemyAmount; } set { m_setEnemyAmount = value; } }

    [SerializeField] int m_enemyAmount = 1;
    public int EnemyAmount { get { return m_enemyAmount; } set { m_enemyAmount = value; } }

    [SerializeField] int m_minEnemyAmount = 1;
    public int MinEnemyAmount { get { return m_minEnemyAmount; } set { m_minEnemyAmount = value; } }

    [SerializeField] int m_maxEnemyAmount = 3;
    public int MaxEnemyAmount { get { return m_maxEnemyAmount; } set { m_maxEnemyAmount = value; } }
    
    [Header("Prefabs", order = 2)]
    [SerializeField] GameObject m_trapPrefab;
    [SerializeField] GameObject[] m_enemyPrefabs;

    [Header("Generated Variables", order = 3)]
    [SerializeField] List<GameObject> m_traps;
    public int TrapCount { get { return m_traps.Count; } }

    [SerializeField] List<GameObject> m_enemies;
    public int EnemyCount { get { return m_enemies.Count; } }

    void Awake()
    {
        if(Instance == null) { m_instance = this; }
        else { Destroy(this); }

        m_enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
    }

    public void GenerateRooms()
    {
        LevelGenerator levelGen = LevelGenerator.Instance;

        if(!m_setTrapsAmount) { m_trapAmount = levelGen.RandomGenerator.Next(m_minTrapAmount, m_maxEnemyAmount + 1); }
        if(!m_setEnemyAmount) { m_enemyAmount = levelGen.RandomGenerator.Next(m_minEnemyAmount, m_maxEnemyAmount + 1); }

        foreach(GameObject gO in levelGen.Rooms)
        {
            List<GameObject> floors = Utilities.FindRoomFloors(gO);
            List<int> pickedIndex = new List<int>();

            for(int i = 0; i < m_trapAmount; i++)
            {
                bool trapPlaced = false;
                int passes = 0;

                while(!trapPlaced)
                {
                    if(passes == 10) { break; }

                    int index = levelGen.RandomGenerator.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;

                    m_traps.Add(Instantiate(m_trapPrefab, new Vector3(floorTransform.position.x, floorTransform.position.y + 1, floorTransform.position.z), Quaternion.identity, m_trapParent.transform));

                    trapPlaced = true;
                }
            }

            for(int i = 0; i < m_trapAmount; i++)
            {
                bool enemyPlaced = false;
                int passes = 0;

                while(!enemyPlaced)
                {
                    if(passes == 10) { break; }

                    int index = levelGen.RandomGenerator.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;

                    m_enemies.Add(Instantiate(m_enemyPrefabs[levelGen.RandomGenerator.Next(0, m_enemyPrefabs.Length)], new Vector3(floorTransform.position.x, floorTransform.position.y + 1, floorTransform.position.z), Quaternion.identity, m_enemyParent.transform));

                    enemyPlaced = true;
                }
            }
        }
    }
}