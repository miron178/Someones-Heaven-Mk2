using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering.Universal;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    //Singleton
    private static LevelGenerator m_instance;
    public static LevelGenerator Instance { get { return m_instance; } }

    [Header("Level Seed", order = 1)]
    
    [SerializeField] private int m_levelSeed = 1;
    public int GetLevelSeed { get { return m_levelSeed; } }

    private System.Random m_randomGenerator;

    [Header("Level Generation Type", order = 2)]
    [SerializeField] private int m_maxBranchSize = 10;
    public int MaxBranchySize { get { return m_maxBranchSize; } set { m_maxBranchSize = value; } }

    [Header("Floor Variables", order = 3)]
    [SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private Transform m_floorParent;
    private List<GameObject> m_floors = new List<GameObject>();
    private List<Vector3> m_floorsPos = new List<Vector3>();

    //Other
    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    Direction[] defaultDir = { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
    int branchTermination = 0;
    bool m_allowConnections = false;
    public bool AllowConnections { get { return m_allowConnections; } set { m_allowConnections = value; } }

    #endregion

    void Awake() { m_instance = this; }

    public void GenerateSeed()
    {
        //Generates our Level Seed
        m_levelSeed = new System.Random(System.DateTime.Now.Millisecond).Next();
        m_randomGenerator = new System.Random(m_levelSeed);
    }

    //Branchy Generation
    public void GenerateBranchyLevel() 
    {
        int bOrW = 0;

        //Init Floor
        Vector3 baseBranch = new Vector3(0, 0, 0);
        InitFloor(baseBranch, ref bOrW);

        int numofBranchesFromBase = m_randomGenerator.Next(2, 5);
        int currentBranchesFromBase = 0;

        List<Direction> pickedDirs = new List<Direction>();

        while(currentBranchesFromBase < numofBranchesFromBase)
        {
            Vector3 newPos = Vector3.zero;
            Direction[] avDirs = defaultDir;
            Direction currentDir = Direction.Up;
            int currentBranchSize = 0;

            //Remove Directions Already Gone
            foreach (Direction dir in pickedDirs) { avDirs = Array.FindAll(avDirs, i => i != dir).ToArray(); }

            //Init 2 Floors
            PickDirection(ref newPos, avDirs, ref currentDir);
            InitFloor(newPos, ref bOrW);

            PushDirection(ref newPos, currentDir);
            InitFloor(newPos, ref bOrW);

            currentBranchSize += 2;

            pickedDirs.Add(currentDir);

            while (currentBranchSize < m_maxBranchSize)
            {
                avDirs = Array.FindAll(defaultDir, i => i != ReverseDirection(currentDir)).ToArray();

                //Init 2 Floors in Directions
                PickDirection(ref newPos, avDirs, ref currentDir);

                if (newPos.x == int.MaxValue) { branchTermination++; break; }

                InitFloor(newPos, ref bOrW);

                PushDirection(ref newPos, currentDir);
                InitFloor(newPos, ref bOrW);
                
                currentBranchSize += 2;

                if (m_randomGenerator.Next(0, 100) > 50)
                {
                    avDirs = Array.FindAll(defaultDir, i => i != ReverseDirection(currentDir)).ToArray();

                    OffBranching(1, newPos, avDirs, ref bOrW);
                }
            }

            currentBranchesFromBase++;
        }

        Debug.Log("Number of Branch Terminations: " + branchTermination);
    }

    public void ClearLevel()
    {
        for (int i = 0; i < m_floors.Count;)
        {
            if (Application.isPlaying) { Destroy(m_floors[i]); }
            else if (Application.isEditor) { DestroyImmediate(m_floors[i]); }
            m_floors.RemoveAt(i);
        }

        m_floorsPos.Clear();
    }

    //Utility
    Direction ReverseDirection(Direction directionToReverse)
    {
        switch (directionToReverse)
        {
            case Direction.Up:
                directionToReverse = Direction.Down;
                break;
            case Direction.Right:
                directionToReverse = Direction.Left;
                break;
            case Direction.Down:
                directionToReverse = Direction.Up;
                break;
            case Direction.Left:
                directionToReverse = Direction.Right;
                break;
        }

        return directionToReverse;
    }
    
    void PickDirection(ref Vector3 lastVector, Direction[] validDirections, ref Direction outDir)
    {
        bool posValid = false;
        int passes = 0;

        while (!posValid)
        {
            Vector3 newPos = lastVector;
            int dir = m_randomGenerator.Next(0, 4);

            if (validDirections.Contains((Direction)dir))
            {
                switch (dir)
                {
                    case 0:
                        {
                            newPos.z += (10 * m_floorPrefab.transform.localScale.z);
                            break;
                        }
                    case 1:
                        {
                            newPos.x += (10 * m_floorPrefab.transform.localScale.z);
                            break;
                        }
                    case 2:
                        {
                            newPos.z -= (10 * m_floorPrefab.transform.localScale.z);
                            break;
                        }
                    case 3:
                        {
                            newPos.x -= (10 * m_floorPrefab.transform.localScale.z);
                            break;
                        }
                }

                if (!m_floorsPos.Contains(newPos))
                {
                    if(!m_allowConnections)
                    {
                        Vector3 tempPos = newPos;

                        PushDirection(ref tempPos, (Direction)dir);

                        if (!m_floorsPos.Contains(tempPos))
                        {
                            lastVector = newPos;
                            outDir = (Direction)dir;
                            posValid = true;
                        }
                    }
                    else
                    {
                        lastVector = newPos;
                        outDir = (Direction)dir;
                        posValid = true;
                    }
                }
            }

            passes++;
            if (passes > 100)
            {
                lastVector.x = int.MaxValue;
                break;
            }
        }
    }

    void PushDirection(ref Vector3 lastVector, Direction dir)
    {
        switch ((int)dir)
        {
            case 0:
                {
                    lastVector.z += (10 * m_floorPrefab.transform.localScale.z);;
                    break;
                }
            case 1:
                {
                    lastVector.x += (10 * m_floorPrefab.transform.localScale.z);
                    break;
                }
            case 2:
                {
                    lastVector.z -= (10 * m_floorPrefab.transform.localScale.z);
                    break;
                }
            case 3:
                {
                    lastVector.x -= (10 * m_floorPrefab.transform.localScale.z);
                    break;
                }
        }
    }

    void InitFloor(Vector3 pos, ref int bOrW)
    {
        GameObject obj = Instantiate(m_floorPrefab, pos, Quaternion.identity, m_floorParent);

        Color col;

        if (bOrW == 0) { col = Color.white; bOrW = 1; }
        else { col = Color.black; bOrW = 0; }

        obj.GetComponent<MeshRenderer>().material.color = col;

        m_floors.Add(obj);
        m_floorsPos.Add(pos);
    }

    void OffBranching(int currentOffBranch, Vector3 lastBranch, Direction[] avDirs, ref int bOrW)
    {
        Vector3 newPos = lastBranch;
        Direction currentDir = Direction.Up;

        PickDirection(ref newPos, avDirs, ref currentDir);

        if (newPos.x == int.MaxValue) { branchTermination++; return; }

        InitFloor(newPos, ref bOrW);

        PushDirection(ref newPos, currentDir);
        InitFloor(newPos, ref bOrW);
        int newBranchSize = 2;

        while (newBranchSize < Mathf.FloorToInt(m_maxBranchSize / currentOffBranch))
        {
            avDirs = Array.FindAll(defaultDir, i => i != ReverseDirection(currentDir)).ToArray();

            PickDirection(ref newPos, avDirs, ref currentDir);
            if (newPos.x == int.MaxValue) { branchTermination++; break; }

            InitFloor(newPos, ref bOrW);

            PushDirection(ref newPos, currentDir);
            InitFloor(newPos, ref bOrW);

            newBranchSize += 2;

            if (m_randomGenerator.Next(1, 100) > 50)
            {
                avDirs = Array.FindAll(defaultDir, i => i != ReverseDirection(currentDir)).ToArray();

                OffBranching(currentOffBranch + 1, newPos, avDirs, ref bOrW);
            }
        }
    }

    //2116218693
    //2017
}