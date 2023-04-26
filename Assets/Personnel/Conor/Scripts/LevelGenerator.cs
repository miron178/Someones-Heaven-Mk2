using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //Random and Seed
    System.Random m_randomGenerator;
    int m_levelSeed = 4444;

    [Header("Generator Settings", order = 0)]
    [SerializeField] bool m_randBranches = false;
    [SerializeField] int m_numofBranches = 1;
    [SerializeField] int m_branchLength = 4;
    [SerializeField] bool m_doubleUp = false;
    [SerializeField] int m_offBranchChance = 50;

    [Header("Floor Settings", order = 1)]
    [SerializeField] Transform m_floorParent;

    [SerializeField] GameObject[] m_roomPrefabs;
    [SerializeField] List<GameObject> m_northPrefabs;
    [SerializeField] List<GameObject> m_eastPrefabs;
    [SerializeField] List<GameObject> m_southPrefabs;
    [SerializeField] List<GameObject> m_westPrefabs;
    
    [Header("Generated Variables", order = 2)]
    [SerializeField] List<GameObject> m_floors;
    [SerializeField] List<Vector3> m_floorPositions;

    void Awake()
    {
        //Load all Prefabs into Generator
        m_roomPrefabs = Resources.LoadAll<GameObject>("Prefabs/Procedural Rooms/Floor1");

        //Sort into directions
        foreach(GameObject gO in m_roomPrefabs)
        {
            foreach(Direction dir in gO.GetComponent<RoomInfo>().GetAllDirections())
            {
                switch(dir)
                {
                    case Direction.North:
                    {
                        m_northPrefabs.Add(gO);
                        break;
                    }
                    case Direction.East:
                    {
                        m_eastPrefabs.Add(gO);
                        break;
                    }
                    case Direction.South:
                    {
                        m_southPrefabs.Add(gO);
                        break;
                    }
                    case Direction.West:
                    {
                        m_westPrefabs.Add(gO);
                        break;
                    }
                }
            }
        }

        //Generate
        Debug.Log("Generating Level. Please Wait...");
        GenerateSeed();
        
        int passes = 0;
        while(m_floors.Count < (m_branchLength * m_numofBranches))
        {
            if(passes == 100) { Debug.LogError("Gave up on Generating!"); break; }

            foreach(GameObject gO in m_floors)
            {
                Destroy(gO);
            }

            m_floors.Clear();
            m_floorPositions.Clear();

            GenerateLevel();
            passes++;
        }
    }

    void GenerateSeed()
    {
        //Gets a Seed from Now.Millisecond and Gets a Random Value from it
        m_levelSeed = new System.Random(DateTime.Now.Millisecond).Next();

        //Init Random Engine using Seed
        m_randomGenerator = new System.Random(m_levelSeed);
    }

    //Tasked Generate Level - Async Function
    void GenerateLevel()
    {
        //In-Function Variables
        RoomInfo baseRoom;
        Vector3 currentPosition = Vector3.zero;
        int numberOfBranches = 0;
        List<Direction> branchDirections = new List<Direction>();

        //Create Base - 0, 0, 0
        baseRoom = SpawnFloor(m_roomPrefabs[0], Vector3.zero);

        //Decide the number of Branches
        if(m_randBranches == true) { numberOfBranches = m_randomGenerator.Next(1, 5); }
        else { numberOfBranches = m_numofBranches; }

        Debug.Log("Generating Directions from Base Branch");
        
        //Decide Branch Directions if less than 4
        if(numberOfBranches == 4)
        {
            branchDirections.Add(Direction.North);
            branchDirections.Add(Direction.East);
            branchDirections.Add(Direction.South);
            branchDirections.Add(Direction.West);
        }
        else
        {
            //Go through each Branch and give it a Direction
            for(int i = 0; i < numberOfBranches; i++)
            {
                Direction pickedDir = Direction.None;

                while(pickedDir == Direction.None)
                {
                    Direction temp = (Direction)m_randomGenerator.Next(1, 5);

                    if(!branchDirections.Contains(temp)) { pickedDir = temp; }
                }

                branchDirections.Add(pickedDir);
            }
        }

        //Generate the Branches
        for(int i = 0; i < numberOfBranches; i++)
        {
            Debug.Log($"Generating Branch {i+1}");
            GenerateBranch(branchDirections[i], baseRoom);
        }
    }

    void GenerateBranch(Direction initalDir, RoomInfo baseBranch)
    {
        //Push out 1 Room First
        Direction dir = initalDir;
        Vector3 currentPosition = Vector3.zero;
        RoomInfo currentRoom = baseBranch;
        GameObject roomObject = m_roomPrefabs[0];

        Vector3 newPos = PushDirection(currentPosition, currentRoom, dir);
        if(newPos == new Vector3(-1, -1, -1)) { Debug.LogWarning("Branch First Terminated from Base!"); return; }

        if(m_doubleUp)
        {
            RoomInfo tempSpawn = SpawnFloor(roomObject, newPos);
            tempSpawn.RemoveDirection(GetReverseDirection(dir));

            Direction tempDir = PickDirection(tempSpawn);
            GameObject tempObject = PickNextRoom(tempDir);

            Vector3 tempPos = PushDirection(newPos, tempSpawn, tempDir);
            if(tempPos == new Vector3(-1, -1, -1)) 
            { 
                Debug.LogWarning("Branch Second Terminated from Base!");

                m_floors.Remove(tempSpawn.gameObject);
                m_floorPositions.Remove(newPos);

                Destroy(tempSpawn.gameObject);

                return;
            }

            roomObject = tempObject;
            newPos = tempPos;
            dir = tempDir;
        }

        currentRoom = SpawnFloor(roomObject, newPos);
        currentRoom.RemoveDirection(dir);

        currentPosition = newPos;

        int passes = 0;
        for(int i = 1; i < m_branchLength; i++)
        {
            bool posFound = false;

            while(!posFound)
            {
                if(passes == 10) { break; }

                dir = PickDirection(currentRoom);
                roomObject = PickNextRoom(dir);

                newPos = PushDirection(currentPosition, currentRoom, dir);
                if(newPos == new Vector3(-1, -1, -1)) { passes++; continue; }

                if(m_doubleUp)
                {
                    RoomInfo tempSpawn = SpawnFloor(roomObject, newPos);
                    tempSpawn.RemoveDirection(GetReverseDirection(dir));

                    Direction tempDir = PickDirection(tempSpawn);
                    GameObject tempObject = PickNextRoom(tempDir);

                    Vector3 tempPos = PushDirection(newPos, tempSpawn, tempDir);
                    if(tempPos == new Vector3(-1, -1, -1)) 
                    {
                        m_floors.Remove(tempSpawn.gameObject);
                        m_floorPositions.Remove(newPos);

                        Destroy(tempSpawn.gameObject);

                        passes++;

                        continue;
                    }

                    roomObject = tempObject;
                    newPos = tempPos;
                    dir = tempDir;
                }

                currentRoom = SpawnFloor(roomObject, newPos);
                currentRoom.RemoveDirection(GetReverseDirection(dir));

                currentPosition = newPos;

                posFound = true;
            }

            if(!posFound) { Debug.LogWarning($"Branch Terminated at {i}!"); break; }
            else { }
        }

        if(passes < 10) { Debug.Log("Branch Complete"); }
    }

    void OffBranching(int currentOffBranchCount, Vector3 currentPos, RoomInfo currentRoom)
    {
        Vector3 currentOffPos = currentPos;
        Direction dir = Direction.None;
        GameObject gO = null;
        RoomInfo rI = currentRoom;
        Vector3 newPos = currentOffPos;

        int passes = 0;
        for(int i = 0; i < Mathf.FloorToInt(m_branchLength / currentOffBranchCount); i++)
        {
            bool posFound = false;

            while(!posFound)
            {
                if(passes == 10) { break; }

                dir = PickDirection(rI);

                gO = PickNextRoom(dir);

                newPos = PushDirection(currentOffPos, rI, dir);
                if(newPos == new Vector3(-1, -1, -1)) { passes++; continue; }

                if(m_doubleUp)
                {
                    RoomInfo tempSpawn = SpawnFloor(gO, newPos);
                    tempSpawn.RemoveDirection(GetReverseDirection(dir));

                    Direction newNewDir = PickDirection(tempSpawn);
                    GameObject newNextRoom = m_roomPrefabs[0];
                    Vector3 newNewPos = PushDirection(newPos, tempSpawn, newNewDir);
                    if(newNewPos == new Vector3(-1, -1, -1)) { passes++; m_floorPositions.Remove(newPos); m_floors.Remove(tempSpawn.gameObject); Destroy(tempSpawn.gameObject); continue; }

                    dir = newNewDir;
                    gO = newNextRoom;
                    newPos = newNewPos;
                }

                rI = SpawnFloor(gO, newPos);
                rI.RemoveDirection(dir);

                currentOffPos = newPos;

                posFound = true;
            }

            if(!posFound) { Debug.LogWarning("Off-Branch Terminated!"); return; }
            else
            {
                if(m_randomGenerator.Next(1, 101) > (100 - m_offBranchChance))
                {
                    OffBranching(currentOffBranchCount + 1, currentOffPos, rI);
                }
            }
        }
        
        Debug.Log($"Off-Branch {currentOffBranchCount} Complete");
    }

    GameObject PickNextRoom(Direction dir)
    {
        GameObject nextRoom = null;
        
        switch(dir)
        {
            case Direction.North:
            {
                int index = m_randomGenerator.Next(0, m_southPrefabs.Count);

                nextRoom = m_southPrefabs[index];

                break;
            }
            case Direction.East:
            {
                int index = m_randomGenerator.Next(0, m_westPrefabs.Count);

                nextRoom = m_westPrefabs[index];

                break;
            }
            case Direction.South:
            {
                int index = m_randomGenerator.Next(0, m_northPrefabs.Count);

                nextRoom = m_northPrefabs[index];

                break;
            }
            case Direction.West:
            {
                int index = m_randomGenerator.Next(0, m_eastPrefabs.Count);

                nextRoom = m_eastPrefabs[index];

                break;
            }
        }

        return nextRoom;
    }

    Vector3 PushDirection(Vector3 currentPos, RoomInfo lastRoom, Direction dir)
    {
        Vector3 newPos = currentPos + (lastRoom.GetDirectionalOffset(dir) * 2);

        if(m_floorPositions.Contains(newPos)) { newPos = new Vector3(-1, -1, -1); }
    
        return newPos;
    }

    Direction PickDirection(RoomInfo currentRoom)
    {
        List<Direction> avDir = currentRoom.GetAvailableDirections;

        if(avDir.Count == 1) { return avDir[0]; }
        else
        {
            int index = m_randomGenerator.Next(0, avDir.Count);

            return avDir[index];
        }
    }

    RoomInfo SpawnFloor(GameObject gO, Vector3 pos)
    {
        GameObject newObject = Instantiate(gO, pos, Quaternion.identity, m_floorParent);

        m_floors.Add(newObject);
        m_floorPositions.Add(pos);

        return newObject.GetComponent<RoomInfo>();
    }

    Direction GetReverseDirection(Direction dir)
    {
        switch(dir)
        {
            case Direction.North: { return Direction.South; }
            case Direction.East: { return Direction.West; }
            case Direction.South: { return Direction.North; }
            case Direction.West: { return Direction.East; }
        }

        return Direction.None;
    }
}