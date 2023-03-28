using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callback : MonoBehaviour
{
    public delegate void OnDestroyCallback(GameObject obj);

    List<OnDestroyCallback> onDestroy;

    Callback()
    {
        onDestroy = new List<OnDestroyCallback>();
    }


    private void OnDestroy()
    {
        foreach (var callback in onDestroy)
        {
            callback(gameObject);
        }
    }

    public void AddOnDestroyCallback(OnDestroyCallback callback)
    {
        onDestroy.Add(callback);
    }

    public void RemoveOnDestroyCallback(OnDestroyCallback callback)
    {
        onDestroy.Remove(callback);
    }
}
