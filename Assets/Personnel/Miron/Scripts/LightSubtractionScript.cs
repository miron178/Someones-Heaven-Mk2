using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSubtractionScript : MonoBehaviour
{
    [SerializeField]
    Material[] materials;

    enum MaterialType
    {
        LightSolid = 0,
        LightTrans,
        ShadowSolid,
        ShadowTrans,
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Light")
        {
            other.gameObject.GetComponent<Collider>().isTrigger = false;
            other.gameObject.GetComponent<MeshRenderer>().material = materials[(int)MaterialType.LightSolid];
        }
        else if (other.tag == "Shadow")
        {
            other.gameObject.GetComponent<Collider>().isTrigger = true;
            other.gameObject.GetComponent<MeshRenderer>().material = materials[(int)MaterialType.ShadowTrans];
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Material otherMaterial = other.gameObject.GetComponent<Material>();
        if (other.tag == "Light")
        {
            other.gameObject.GetComponent<Collider>().isTrigger = true;
            other.gameObject.GetComponent<MeshRenderer>().material = materials[(int)MaterialType.LightTrans];
        }
        else if (other.tag == "Shadow")
        {
            other.gameObject.GetComponent<Collider>().isTrigger = false;
            other.gameObject.GetComponent<MeshRenderer>().material = materials[(int)MaterialType.ShadowSolid];
        }
    }
}
