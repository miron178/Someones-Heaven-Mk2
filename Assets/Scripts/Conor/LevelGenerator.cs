using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Variables
    //Privates
    private static LevelGenerator m_instance;

    [SerializeField] private int m_levelSeed;
    private System.Random m_randomGen;
    [SerializeField] private int m_levelSize = 10;
    [SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private Transform m_floorParent;
    private List<GameObject> m_floors = new List<GameObject>();

    //Publics
    public static LevelGenerator Instance { get { return m_instance; } }

    public int LevelSeed { get { return m_levelSeed; } }
    public int LevelSize { get { return m_levelSize; } }
    #endregion

    void Awake()
    {
        m_instance = this;
    }

    public void GenerateSeed()
    {
        //Generates our Level Seed
        m_levelSeed = new System.Random(System.DateTime.Now.Millisecond).Next();
        m_randomGen = new System.Random(m_levelSeed);
    }

    // On Player Start
    public void GenerateLevel()
    {
        for(int i = 0; i < m_floors.Count;)
        {
            if(Application.isPlaying) { Destroy(m_floors[i]); }
            else if (Application.isEditor) { DestroyImmediate(m_floors[i]); }
            m_floors.RemoveAt(i);
        }

        int currentLevelSize = 0;
        int north = 0;
        int east = 0;
        int south = 0;
        int west = 0;

        while(currentLevelSize < m_levelSize)
        {
            if(m_floors.Count == 0)
            {
                m_floors.Add(GameObject.Instantiate(m_floorPrefab, new Vector3(0, 0, 0), Quaternion.identity, m_floorParent));
                currentLevelSize++;
            }
            else
            {   
                Vector3 newPos = new Vector3(0, 0, 0);
                bool postionValid = false;

                while(!postionValid)
                {
                    newPos = m_floors[m_randomGen.Next(0, m_floors.Count)].transform.position;
                    
                    int direction = m_randomGen.Next(1, 5);

                    switch(direction)
                    {
                        //North
                        case 1:
                        {
                            newPos.z += 10;
                            north++;
                            break;
                        }
                        case 2:
                        {
                            newPos.x += 10;
                            east++;
                            break;
                        }
                        case 3:
                        {
                            newPos.z -= 10;
                            south++;
                            break;
                        }
                        case 4:
                        {
                            newPos.x -= 10;
                            west++;
                            break;
                        }
                    }
                    
                    bool posFound = false;

                    foreach (GameObject gO in m_floors)
                    {
                        if(newPos == gO.transform.position) { posFound = true; }
                    }

                    if(posFound) { continue; }
                    else { postionValid = true; }
                }

                m_floors.Add(Instantiate(m_floorPrefab, newPos, Quaternion.identity, m_floorParent));
                m_floors[m_floors.Count - 1].GetComponent<MeshRenderer>().material.color = new Color32((byte)m_randomGen.Next(256), (byte)m_randomGen.Next(256), (byte)m_randomGen.Next(256), 255);
                currentLevelSize++;
            }
        }
    }

    public void ClearLevel()
    {
        for(int i = 0; i < m_floors.Count;)
        {
            if(Application.isPlaying) { Destroy(m_floors[i]); }
            else if (Application.isEditor) { DestroyImmediate(m_floors[i]); }
            m_floors.RemoveAt(i);
        }
    }
}
