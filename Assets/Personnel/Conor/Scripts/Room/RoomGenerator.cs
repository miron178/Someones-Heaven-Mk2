using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    private static RoomGenerator m_instance;
    public static RoomGenerator Instance { get { return m_instance; } }

    GameManager m_gameManager;
    System.Random m_random;

    [Header("Generation Setting", order = 1)]
    //Traps
    [SerializeField] bool m_setTrapsAmount = false;
    public bool SetTrapAmount { get { return m_setTrapsAmount; } set { m_setTrapsAmount = value; } }

    [SerializeField] int m_trapAmount = 1;
    public int TrapAmount { get { return m_trapAmount; } set { m_trapAmount = value; } }

    [SerializeField] int m_minTrapAmount = 1;
    public int MinTrapAmount { get { return m_minTrapAmount; } set { m_minTrapAmount = value; } }
    
    [SerializeField] int m_maxTrapAmount = 3;
    public int MaxTrapAmount { get { return m_maxTrapAmount; } set { m_maxTrapAmount = value; } }

    //Enemies
    GameObject m_enemyParent;

    [SerializeField] bool m_setEnemyAmount = false;
    public bool SetEnemyAmount { get { return m_setEnemyAmount; } set { m_setEnemyAmount = value; } }

    [SerializeField] int m_enemyAmount = 1;
    public int EnemyAmount { get { return m_enemyAmount; } set { m_enemyAmount = value; } }

    [SerializeField] int m_minEnemyAmount = 1;
    public int MinEnemyAmount { get { return m_minEnemyAmount; } set { m_minEnemyAmount = value; } }

    [SerializeField] int m_maxEnemyAmount = 3;
    public int MaxEnemyAmount { get { return m_maxEnemyAmount; } set { m_maxEnemyAmount = value; } }

    //Power Ups
    GameObject m_powerUpParent;

    [SerializeField] bool m_setPowerUpAmount = false;
    [SerializeField] int m_powerUpAmount = 1;
    [SerializeField] int m_minPowerUpAmount = 1;
    [SerializeField] int m_maxPowerUpAmount = 3;
    
    [Header("Prefabs", order = 2)]
    [SerializeField] GameObject m_trapPrefab;
    [SerializeField] GameObject[] m_enemyPrefabs;
    [SerializeField] GameObject[] m_powerUpPrefabs;

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
    public void RemoveEnemy(GameObject gO) { m_enemies.Remove(gO); }
    public void ClearEnemies()
    {
        foreach(GameObject gO in m_enemies) { Destroy(gO); }
        m_enemies.Clear();
    }

    [SerializeField] List<GameObject> m_powerUps;
    public int PowerUpCount { get { return m_powerUps.Count; } }
    public void RemovePowerUp(GameObject gO) { m_powerUps.Remove(gO); }
    public void ClearPowerUps() 
    { 
        foreach(GameObject gO in m_powerUps) { Destroy(gO); }
        m_powerUps.Clear();
    }

    void Awake()
    {
        if(Instance == null) { m_instance = this; }
        else { Destroy(this); }

        m_enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemies");
        m_powerUpPrefabs = Resources.LoadAll<GameObject>("Prefabs/Power Ups");
    }

    void Start() { m_gameManager = GameManager.Instance; }

    public void ClearRoom()
    {
        foreach(GameObject gO in m_traps) { Destroy(gO); }
        foreach(GameObject gO in m_enemies) { Destroy(gO); }
        foreach(GameObject gO in m_powerUps) { Destroy(gO); }

        Destroy(m_enemyParent);
        Destroy(m_powerUpParent);

        m_traps.Clear();
        m_enemies.Clear();
        m_powerUps.Clear();
    }

    public void GenerateTraps(bool nextRoom = false)
    {
        LevelGenerator levelGen = LevelGenerator.Instance;

        if(!nextRoom) {  m_random = m_gameManager.RandomGenerator;}
        else { m_random = m_gameManager.RandomGeneratorSame; }

        if(!m_setTrapsAmount) { m_trapAmount = m_random.Next(m_minTrapAmount, m_maxEnemyAmount + 1); }

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

                    int index = m_random.Next(0, floors.Count);
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
        m_enemyParent = new GameObject();
        m_enemyParent.name = "Enemies";
        LevelGenerator levelGen = LevelGenerator.Instance;

        if(!nextRoom) {  m_random = m_gameManager.RandomGenerator; }
        else { m_random = m_gameManager.RandomGeneratorSame; }

        if(!m_setEnemyAmount) { m_enemyAmount = m_random.Next(m_minEnemyAmount, m_maxEnemyAmount + 1); }

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

                    int index = m_random.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;
                    Vector3 pos = new Vector3(floorTransform.position.x, floorTransform.position.y + 1, floorTransform.position.z);

                    GameObject enemy = Instantiate(m_enemyPrefabs[m_random.Next(0, m_enemyPrefabs.Length)], pos, Quaternion.identity, m_enemyParent.transform);

                    m_enemies.Add(enemy);
                    //print(gO.name);
                    gO.GetComponent<RoomActivation>().AddEnemy(enemy);
                    gO.GetComponent<RoomActivation>().AddEnemyPosition(pos);

                    enemyPlaced = true;
                }
            }

            gO.GetComponent<RoomActivation>().DisableEnemies();
        }
    }

    public void GeneratePowerUps(bool nextRoom = false)
    {
        m_powerUpParent = new GameObject();
        m_powerUpParent.name = "Power Ups";
        LevelGenerator levelGen = LevelGenerator.Instance;

        if(!nextRoom) { m_random = m_gameManager.RandomGenerator; }
        else { m_random = m_gameManager.RandomGeneratorSame; }

        if(!m_setPowerUpAmount) { m_powerUpAmount = m_random.Next(m_minPowerUpAmount, m_maxPowerUpAmount + 1); }

        bool skipFirstRoom = true;

        foreach(GameObject gO in levelGen.Rooms)
        {
            if(skipFirstRoom) { skipFirstRoom = false; continue; }

            List<GameObject> floors = Utilities.FindRoomFloors(gO);
            List<int> pickedIndex = new List<int>();

            for(int i = 0; i < m_powerUpAmount; i++)
            {
                bool powerUpPlaced = false;
                int passes = 0;

                while(!powerUpPlaced)
                {
                    if(passes == 10) { break; }

                    int index = m_random.Next(0, floors.Count);
                    if(pickedIndex.Contains(index)) { passes++; continue; }

                    Transform floorTransform = floors[index].transform;
                    Vector3 pos = new Vector3(floorTransform.position.x, floorTransform.position.y + 1, floorTransform.position.z);

                    GameObject powerUp = Instantiate(m_powerUpPrefabs[m_random.Next(0, m_powerUpPrefabs.Length)], pos, Quaternion.identity, m_powerUpParent.transform);

                    m_powerUps.Add(powerUp);
                    powerUpPlaced = true;
                }
            }
        }
    }

    public void PickRandomRoom(bool nextRoom = false) 
    { 
        LevelGenerator levelGen = LevelGenerator.Instance;
        if(!nextRoom) {  m_random = m_gameManager.RandomGenerator; }
        else { m_random = m_gameManager.RandomGeneratorSame; }

        GameObject endRoom = levelGen.Rooms[m_random.Next(1, levelGen.Rooms.Count)];
        Destroy(endRoom.GetComponent<BoxCollider>());
        Destroy(endRoom.GetComponent<RoomActivation>());

        endRoom.AddComponent<EndRoom>();
        BoxCollider bC = endRoom.AddComponent<BoxCollider>();
        bC.isTrigger = true;
        bC.size = new Vector3(40, 1, 40);
        bC.center = new Vector3(0, 1, 0);
    }
}