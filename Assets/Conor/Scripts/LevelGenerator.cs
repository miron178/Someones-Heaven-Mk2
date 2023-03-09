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
    [SerializeField] private int m_maxExpandSize = 10;
    public int MaxBlockySize { get { return m_maxExpandSize; } set { m_maxExpandSize = value; } }
    [SerializeField] private int m_maxBranchSize = 10;
    public int MaxBranchySize { get { return m_maxBranchSize; } set { m_maxBranchSize = value; } }

    [SerializeField] bool m_isBlockyGeneration = true;
    public bool IsBlockyGeneration { get { return m_isBlockyGeneration; }  set { m_isBlockyGeneration = value; m_isBranchyGeneration = !value; } }
    [SerializeField] bool m_isBranchyGeneration = false;
    public bool IsBranchyGeneration { get { return m_isBranchyGeneration; } set { m_isBranchyGeneration = value; m_isBlockyGeneration = !value; } }

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

    #endregion

    void Awake()
    {
        m_instance = this;

        if(m_isBlockyGeneration && m_isBranchyGeneration) { m_isBranchyGeneration = false; }
    }

    void Update()
    {
        if (m_isBlockyGeneration && m_isBranchyGeneration) { m_isBranchyGeneration = false; }
    }


    public void GenerateSeed()
    {
        //Generates our Level Seed
        m_levelSeed = new System.Random(System.DateTime.Now.Millisecond).Next();
        m_randomGenerator = new System.Random(m_levelSeed);
    }
    //Blocky Generation
    public void GenerateBlockyLevel() 
    {
        int currentLevelSize = 1;
        int bOrW = 0;

        //InitFloor(Vector3.zero, ref bOrW);

        Vector3 newPos = new Vector3(0, 0, 0);
        
        while(currentLevelSize < m_maxExpandSize)
        {
            bool postionValid = false;

            while (!postionValid)
            {
                newPos = m_floors[m_randomGenerator.Next(0, m_floors.Count)].transform.position;

                int direction = m_randomGenerator.Next(1, 5);

                //newPos = PickDirection(newPos);

                bool posFound = false;

                foreach (GameObject gO in m_floors)
                {
                    if (newPos == gO.transform.position) { posFound = true; break; }
                }

                if (posFound) { continue; }
                else { postionValid = true; }
            }

            //InitFloor(newPos, ref bOrW);

            currentLevelSize++;
        }
    }

    //Branchy Generation
    public void GenerateBranchyLevel() 
    {        
        //Init Floor
        Vector3 baseBranch = new Vector3(0, 0, 0);
        InitFloor(baseBranch);

        int currentBranchSize = 1;

        //First Direction
        Vector3 newPos = Vector3.zero;
        Direction currentDir = Direction.Up;
        
        while(currentBranchSize < m_maxBranchSize)
        {
            Direction reverseDir = Direction.Up;
            
            switch(currentDir)
            {
                case Direction.Up:
                    reverseDir = Direction.Down;
                    break;
                case Direction.Right:
                    reverseDir = Direction.Left;
                    break;
                case Direction.Down:
                    reverseDir = Direction.Up;
                    break;
                case Direction.Left:
                    reverseDir = Direction.Right;
                    break;
            }

            Direction[] avDirs = Array.FindAll(defaultDir, i => i != reverseDir).ToArray();

            PickDirection(ref newPos, avDirs, ref currentDir);
            InitFloor(newPos);
            currentBranchSize++;

            PushDirection(ref newPos, currentDir);
            InitFloor(newPos);

            currentBranchSize++;
        }

        ////First Branch
        //int currentBranchSize = 1;
        //Vector3 nextBranch = baseBranch;

        //while (currentBranchSize < m_maxBranchSize)
        //{
        //    nextBranch = FindNewPos(nextBranch);

        //    if(nextBranch.x == int.MaxValue)
        //    {
        //        Debug.Log("Info From Generate Branchy Level:");
        //        Debug.Log("Current Branch Size: " + currentBranchSize);
        //        Debug.Log("Next Branch Pos: " + nextBranch);
        //        break;
        //    }

        //    InitFloor(nextBranch, ref bOrW);
        //    currentBranchSize++;

        //    if (m_randomGenerator.Next(1, 100) > 50)
        //    {
        //        OffBranching(1, nextBranch, ref bOrW);
        //    }
        //}

        ////for (int i = 0; i < m_randomGenerator.Next(1, 4); i++)
        ////{

        ////}
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
    void PickDirection(ref Vector3 lastVector, Direction[] validDirections, ref Direction outDir)
    {
        bool posValid = false;

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

                if (m_floorsPos.Contains(newPos)) { continue; }
                else 
                {
                    lastVector = newPos;
                    outDir = (Direction)dir;
                    posValid = true; 
                }
            }
            else { continue; }
        }

        //switch (m_randomGenerator.Next(0, 4))
        //{
        //    case 0:
        //        {
        //            lastVector.z += (10 * m_floorPrefab.transform.localScale.z);
        //            break;
        //        }
        //    case 1:
        //        {
        //            lastVector.x += (10 * m_floorPrefab.transform.localScale.z);
        //            break;
        //        }
        //    case 2:
        //        {
        //            lastVector.z -= (10 * m_floorPrefab.transform.localScale.z);
        //            break;
        //        }
        //    case 3:
        //        {
        //            lastVector.x -= (10 * m_floorPrefab.transform.localScale.z);
        //            break;
        //        }
        //}
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

    void InitFloor(Vector3 pos)
    {
        GameObject obj = Instantiate(m_floorPrefab, pos, Quaternion.identity, m_floorParent);

        //Color col;

        //if (bOrW == 0) { col = Color.white; bOrW = 1; }
        //else { col = Color.black; bOrW = 0; }

        //obj.GetComponent<MeshRenderer>().material.color = col;

        m_floors.Add(obj);
        m_floorsPos.Add(pos);
    }

    Vector3 FindNewPos(Vector3 pos)
    {
        int pass = 0;

        Vector3 tempPos = Vector3.zero;
        bool posValid = false;

        while (!posValid)
        {
            if(pass >= 100) { posValid = true; }

            //tempPos = PickDirection(pos);
            bool posFound = false;

            foreach (GameObject gO in m_floors) { if (tempPos == gO.transform.position) { posFound = true; break; } }

            if (posFound) { pass++;  continue; }
            else { posValid = true; }
        }

        if(pass >= 100) 
        {
            Debug.LogError("Passes exceeded in Find New Pos! Logging Info...");
            return new Vector3(int.MaxValue, int.MaxValue, int.MaxValue); 
        }
        else { return tempPos; }
    }

    void OffBranching(int currentOffBranch, Vector3 lastBranch, ref int bOrW)
    {
        int newBranchSize = 1;
        Vector3 offBranch = lastBranch;

        while (newBranchSize < Mathf.FloorToInt(m_maxBranchSize / (2 * currentOffBranch)))
        {
            offBranch = FindNewPos(lastBranch);

            if(offBranch.x == int.MaxValue) 
            {
                Debug.Log("Info from OffBranching:");
                Debug.Log("Current off Branch: " + currentOffBranch);
                Debug.Log("Last Branch: " + lastBranch);
                break; 
            }

            //InitFloor(offBranch, ref bOrW);
            newBranchSize++;

            if (m_randomGenerator.Next(1, 100) > 50) 
            {
                OffBranching(currentOffBranch + 1, offBranch, ref bOrW); 
            }
        }
    }

    //2116218693
    //2017
}