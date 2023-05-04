using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private static RoomGenerator m_instance;
    public static RoomGenerator Instance { get { return m_instance; } }

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
    public void ClearTraps()
    {
        foreach(GameObject gO in m_traps) { Destroy(gO); }
        m_traps.Clear();
    }

    [SerializeField] List<GameObject> m_enemies;
    public int EnemyCount { get { return m_enemies.Count; } }
    public void ClearEnemies()
    {
        foreach(GameObject gO in m_enemies) { Destroy(gO); }
        m_enemies.Clear();
    }

    void Awake()
    {
        if(Instance == null) { m_instance = this; }
        else { Destroy(this); }

        m_enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
    }

    public void ClearRoom()
    {
        foreach(GameObject gO in m_traps) { Destroy(gO); }
        foreach(GameObject gO in m_enemies) { Destroy(gO); }

        m_traps.Clear();
        m_enemies.Clear();
    }

    public void GenerateTraps(bool nextRoom = false)
    {
        LevelGenerator levelGen = LevelGenerator.Instance;
        System.Random randomGen;

        if(!nextRoom) {  randomGen = levelGen.RandomGenerator;}
        else { randomGen = levelGen.RandomGeneratorSame; }

        if(!m_setTrapsAmount) { m_trapAmount = randomGen.Next(m_minTrapAmount, m_maxEnemyAmount + 1); }

        bool skipFirstRoom = true;

        foreach(GameObject gO in levelGen.Rooms)
        {
            if(skipFirstRoom) { skipFirstRoom = false; continue; }

            List<GameObject> floors = Utilities.FindRoomFloors(gO);
            List<int> pickedIndex = new List<int>();

            for(int i = 0; i < m_trapAmount; i++)
            {
                bool trapPlaced = false;
                int passes = 0;

                while(!trapPlaced)
                {
                    if(passes == 10) { break; }

                    int index = randomGen.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;

                    Vector3 trapPlacement = new Vector3(
                        floorTransform.position.x + 2,
                        floorTransform.position.y,
                        floorTransform.position.z - 3.5f
                    );

                    GameObject trap = Instantiate(m_trapPrefab, trapPlacement, Quaternion.identity, floorTransform.parent);

                    m_traps.Add(trap);

                    trapPlaced = true;
                }
            }  
        }
    }

    public void GenerateEnemies(bool nextRoom = false)
    {
        LevelGenerator levelGen = LevelGenerator.Instance;
        System.Random randomGen;

        if(!nextRoom) {  randomGen = levelGen.RandomGenerator; }
        else { randomGen = levelGen.RandomGeneratorSame; }

        if(!m_setEnemyAmount) { m_enemyAmount = randomGen.Next(m_minEnemyAmount, m_maxEnemyAmount + 1); }

        bool skipFirstRoom = true;

        foreach(GameObject gO in levelGen.Rooms)
        {
            if(skipFirstRoom) { skipFirstRoom = false; continue; }

            List<GameObject> floors = Utilities.FindRoomFloors(gO);
            List<int> pickedIndex = new List<int>();

            for(int i = 0; i < m_enemyAmount; i++)
            {
                bool enemyPlaced = false;
                int passes = 0;

                while(!enemyPlaced)
                {
                    if(passes == 10) { break; }

                    int index = randomGen.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;
                    Vector3 pos = new Vector3(floorTransform.position.x, floorTransform.position.y + 1, floorTransform.position.z);

                    GameObject enemy = Instantiate(m_enemyPrefabs[randomGen.Next(0, m_enemyPrefabs.Length)], pos, Quaternion.identity, m_enemyParent.transform);

                    m_enemies.Add(enemy);
                    print(gO.name);
                    gO.GetComponent<RoomActivation>().AddEnemy(enemy);
                    gO.GetComponent<RoomActivation>().AddEnemyPosition(pos);

                    enemyPlaced = true;
                }
            }

            gO.GetComponent<RoomActivation>().DisableEnemies();
        }
    }
}