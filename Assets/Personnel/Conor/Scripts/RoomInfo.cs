using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] DirectionalOffset[] m_directionalOffsets;
    public Vector3 GetDirectionalOffset(Direction dir) 
    { 
        foreach(DirectionalOffset dO in m_directionalOffsets)
        {
            if(dO.Direction == dir) { return dO.Offset; }
        }

        return Vector3.positiveInfinity;
    }
    public List<Direction> GetAllDirections() 
    {
        List<Direction> returnDirections = new List<Direction>();

        foreach(DirectionalOffset dO in m_directionalOffsets)
        {
            returnDirections.Add(dO.Direction);
        }

        return returnDirections;
    }

    [SerializeField] List<Direction> m_availableDirections;
    public List<Direction> GetAvailableDirections { get { return m_availableDirections; } }
    public void RemoveDirection(Direction toRemove) { m_availableDirections.Remove(toRemove); }

    void Awake()
    {
        foreach(DirectionalOffset dO in m_directionalOffsets)
        {
            m_availableDirections.Add(dO.Direction);
        }
    }
}