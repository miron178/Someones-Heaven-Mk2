using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowToEnd : MonoBehaviour
{
    [SerializeField] GameObject m_arrow;
    [SerializeField] GameObject m_endRoom;
    [SerializeField] GameObject m_player;

    void Awake()
    {
        m_arrow = GameObject.FindGameObjectWithTag("Arrow");
        m_endRoom = GameObject.FindGameObjectWithTag("EndRoom");
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 toPos = m_endRoom.transform.position;
        Vector3 fromPos = m_player.transform.position;
        Vector3 dir = (toPos - fromPos).normalized;
        float angle = Vector3.Angle(dir, Vector3.forward);
        m_arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
