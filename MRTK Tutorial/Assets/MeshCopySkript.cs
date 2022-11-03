using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCopySkript : MonoBehaviour
{
    public GameObject meshHolderPrefab;
    public GameObject testCupPrefab;
    public LayerMask colLayer;
    public Material mat;
    public GameObject mixedRealityPlayspace;
    [HideInInspector]
    public GameObject[] meshCopyCollection;
    [HideInInspector]
    public MeshRenderer[] mcRenderer;
    [HideInInspector]
    public bool gotMesh;

    private Transform OpenSMO;
    private Transform SAS;
    private List<GameObject> placedObjects;

    // bool startedToCopyMesh = false;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();
        mixedRealityPlayspace = GameObject.Find("MixedRealityPlayspace");
        SAS = mixedRealityPlayspace.transform.Find("Spatial Awareness System");
        OpenSMO = SAS.Find("OpenXR Spatial Mesh Observer");
    }

    // Update is called once per frame
    void Update()
    {
        meshCopyCollection = new GameObject[OpenSMO.childCount];
        for (int i = 0; i < OpenSMO.childCount - 1; i++)
        {
            GameObject newMeshHolder = Instantiate(meshHolderPrefab, transform);
            MeshFilter meshFilter = newMeshHolder.GetComponent<MeshFilter>();
            Mesh newMesh = meshFilter.mesh;
            Mesh originalMesh = OpenSMO.GetChild(i).GetComponent<MeshFilter>().mesh;

            newMesh.SetVertices(originalMesh.vertices);
            newMesh.SetNormals(originalMesh.normals);
            newMesh.SetTriangles(originalMesh.triangles, 0);
            newMeshHolder.GetComponent<MeshCollider>().sharedMesh = newMesh;

            Vector3[] vertices = newMesh.vertices;
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int v = 0; v < uvs.Length; v++)
            {
                uvs[v] = new Vector2(vertices[v].x, vertices[v].z);
            }
            newMesh.SetUVs(0, uvs);
            meshCopyCollection[i] = newMeshHolder;
        }

        //clear all objects
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.RemoveAll(o => o == null);
 
        //testing collision:
        for (float x = -2; x <= 2; x += 0.125f)
        {
            for (float z = -2; z <= 2; z += 0.125f)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, 0f, z), transform.TransformDirection(Vector3.down), out hit, 3, colLayer))
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit ");
                    GameObject newObj = Instantiate(testCupPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal));
                    placedObjects.Add(newObj);
                }
            }
        }
    }
}
