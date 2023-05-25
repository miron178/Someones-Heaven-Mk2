using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughChecker : MonoBehaviour
{
    [SerializeField]
    private Camera useCamera;

    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    [Range (1,30)]
    private float size = 10f;

    [SerializeField]
    [Range (0.5f,10f)]
    float speed = 2f;

    float targetSize;

    // Start is called before the first frame update
    void Start()
    {
        targetSize = size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = transform.position - useCamera.transform.position;
        float distance = direction.magnitude;
        if (Physics.Raycast(useCamera.transform.position, direction, distance, layers))
        {
            //make sure camera is not in sphere
            float maxSize = 2 * (distance - useCamera.nearClipPlane);
            targetSize = Mathf.Min(size, maxSize);
        }
        else
        {
            targetSize = 0;
        }

        float current = transform.localScale.x;
        float updated = Mathf.Lerp(current, targetSize, speed * Time.deltaTime);
        transform.localScale = new Vector3(updated, updated, updated);
    }
}
