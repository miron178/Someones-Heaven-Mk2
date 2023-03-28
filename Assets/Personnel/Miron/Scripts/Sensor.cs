using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sensor : MonoBehaviour
{
    public float distance = 10;
    public float angle = 30;
    public float height = 1;
    public Color color = Color.red;

    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;

    private List<GameObject> objects = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    void OnDisable()
    {
        Clear();
    }

    void Clear()
    {
        foreach (var obj in objects)
        {
            Callback callback = obj.GetComponent<Callback>();
            if (callback != null)
                callback.RemoveOnDestroyCallback(Destroyed);
        }
        objects.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Destroyed(GameObject obj)
    {
        objects.Remove(obj);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] && colliders[i].gameObject == obj)
            {
                colliders[i] = null;
                break;
            }
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            Transform search = transform;
            while (search != null)
            {
                if (obj == search.gameObject)
                {
                    colliders[i] = null;
                    break;
                }
                search = search.parent;
            };

            if (colliders[i] != null && IsInSight(obj))
            {
                objects.Add(obj);

                Callback callback = obj.GetComponent<Callback>();
                if (callback != null)
                {
                    callback.AddOnDestroyCallback(Destroyed);
                }
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        //Check hight
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        //check if in radius
        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }


        //check wall
        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    public bool IsInSight(LayerMask layers)
    {
        foreach (var obj in objects)
        {
            if ((layers.value & (1 << obj.layer)) != 0)
            {
                return true;
            }
        }

        return false;
    }

    public List<GameObject> InSight(LayerMask layers)
    {
        List<GameObject> matching = new List<GameObject>();

        foreach (var obj in objects)
        {
            if ((layers.value & (1 << obj.layer)) != 0)
            {
                matching.Add(obj);
            }
        }

        return matching;
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;
        // left side 
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side 
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i])
            {
                Gizmos.DrawSphere(colliders[i].transform.position + Vector3.up, 0.2f);
            }
        }

        Gizmos.color = Color.green;
        foreach (var obj in objects)
        {
            if (obj)
            {
                Gizmos.DrawSphere(obj.transform.position + Vector3.up, 0.2f);
            }
        }
    }
}
