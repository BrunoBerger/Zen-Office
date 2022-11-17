using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using Microsoft.MixedReality.Toolkit.SpatialAwareness;

public class MeshCopySkript : MonoBehaviour
{
    public bool permaMeshUpdate = false;
    public GameObject meshHolderPrefab;
    public GameObject[] bushes;
    public GameObject[] grases;
    //public GameObject testCupPrefab;
    public SpawnRocks rockHolder;
    public bool doesTreeSpawning=true;
    public GameObject[] trees;
    public LayerMask colLayer;
    public Material mat;
    public GameObject mixedRealityPlayspace;
    [HideInInspector]
    public GameObject[] meshCopyCollection;
    [HideInInspector]
    public MeshRenderer[] mcRenderer;
    [HideInInspector]
    bool sasReady =false;
    bool updatedOnce = false;

    Transform OpenSMO;
    Transform SAS;
    List<GameObject> placedObjects;
    float updateTimer;
    public float floorHeight;
    public float treeLine;

    // bool startedToCopyMesh = false;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();
        //Debug.Log(SAS.name);
        updateTimer = -6;
        floorHeight = float.MaxValue;
        treeLine = -0.3f;
        updatedOnce = false;
        sasReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.deltaTime;

        // Setup once
        if (!sasReady && updateTimer > 0)
        {
            // With transforms
            mixedRealityPlayspace = GameObject.Find("MixedRealityPlayspace");
            SAS = mixedRealityPlayspace.transform.Find("Spatial Awareness System");
            OpenSMO = SAS.Find("OpenXR Spatial Mesh Observer");

            // 
            // Use CoreServices to quickly get access to the IMixedRealitySpatialAwarenessSystem
            var spatialAwarenessService = CoreServices.SpatialAwarenessSystem;
            // Cast to the IMixedRealityDataProviderAccess to get access to the data providers
            var dataProviderAccess = spatialAwarenessService as IMixedRealityDataProviderAccess;
            var meshObserverName = "Spatial Object Mesh Observer";
            var meshObserver = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);
            Debug.Log("LLLLLLLLLLLLLLLLLLLLLL" + meshObserver.UpdateInterval);
            sasReady = true;
        }
        //
        if (updateTimer > 3f && (permaMeshUpdate || !updatedOnce) )
        {
            StartCoroutine(updateMesh());
            updatedOnce = true;
            updateTimer = 0;
        }
    }

    private IEnumerator updateMesh()
    {


        foreach (GameObject mesh in meshCopyCollection)
        {
            Destroy(mesh);
        }
        meshCopyCollection = new GameObject[OpenSMO.childCount];
        for (int i = 0; i < OpenSMO.childCount; i++)
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

            // Create UVs and find lowest point as the floor
            for (int v = 0; v < uvs.Length; v++)
            {
                uvs[v] = new Vector2(vertices[v].x, vertices[v].z);
                
                if (vertices[v].y < floorHeight)
                {
                    floorHeight = vertices[v].y;
                }
            }
            newMesh.SetUVs(0, uvs);
            meshCopyCollection[i] = newMeshHolder;
        }


        if (doesTreeSpawning)
        {
            //clear trees
            foreach (GameObject obj in placedObjects)
            {
                Destroy(obj);
            }
            placedObjects.RemoveAll(o => o == null);

            //spawn trees:

            Vector3 camPos = Camera.main.transform.position;
            for (float x = -2; x <= 2; x += 0.05f)
            {
                for (float z = -2; z <= 2; z += 0.05f)
                {
                    float noiseSample = Mathf.PerlinNoise(x * 5, z * 5);
                    if (noiseSample > 0.5f)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(new Vector3(camPos.x+x, 0f, camPos.z + z), transform.TransformDirection(Vector3.down), out hit, 3, colLayer))
                        {
                            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                            // Only if flat at not at the ground
                            Vector3 hp = hit.point;

                            if (hit.normal.y > 0.9f && hp.y > floorHeight + treeLine)
                            {
                                //Debug.Log("floorHeight: " + floorHeight + "   hitP: " + hp.y);
                                GameObject newObj = Instantiate(trees[Random.Range(0, trees.Length - 1)], hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal));
                                placedObjects.Add(newObj);
                            }
                        }
                    }
                }
            }
        }




        //start Rock spawning
        rockHolder.DeleteRocks();
        Mesh[] meshesMesh = new Mesh[meshCopyCollection.Length];
        for (int i=0; i < meshCopyCollection.Length;i++)
        {
            meshesMesh[i] = meshCopyCollection[i].GetComponent<MeshFilter>().mesh;
        }
        rockHolder.StartRockSpawning(meshesMesh);
        yield return null;
    }
}
