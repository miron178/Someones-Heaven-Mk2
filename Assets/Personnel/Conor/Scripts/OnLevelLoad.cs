using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelLoad : MonoBehaviour
{
    void Awake() { GameManager.Instance.StartGame(); }
}
