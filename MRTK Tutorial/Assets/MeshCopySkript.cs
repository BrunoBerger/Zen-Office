using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//using Microsoft.MixedReality.Toolkit.SpatialAwareness;

public class MeshCopySkript : MonoBehaviour
{
    public bool permaMeshUpdate = false;
    public bool doesTreeSpawning=true;


    public SpawnRocks rockHolder;
    public SpawnPlants spawnPlants;
    public Material mat;

    public GameObject meshHolderPrefab;
    public GameObject mixedRealityPlayspace;
    [HideInInspector]
    public GameObject[] meshCopyCollection;
    bool sasReady = false;

    Transform transformOpenSMO;
    Transform transformSAS;
    IMixedRealitySpatialAwarenessSystem scriptSpatialAwarenessService;
    IMixedRealityDataProviderAccess scriptDataProviderAccess;
    IMixedRealitySpatialAwarenessMeshObserver scriptMeshObserver;

    float updateTimer;
    public float floorHeight;

    // Start is called before the first frame update
    void Start()
    {
        updateTimer = -6;
        floorHeight = float.MaxValue;
        sasReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.deltaTime;

        // Setup once
        if (!sasReady && updateTimer > 0)
        {
            // As transforms
            mixedRealityPlayspace = GameObject.Find("MixedRealityPlayspace");
            transformSAS = mixedRealityPlayspace.transform.Find("Spatial Awareness System");
            transformOpenSMO = transformSAS.Find("OpenXR Spatial Mesh Observer");

            // As objects
            scriptSpatialAwarenessService = CoreServices.SpatialAwarenessSystem;
            scriptDataProviderAccess = scriptSpatialAwarenessService as IMixedRealityDataProviderAccess;
            var meshObserverName = "OpenXR Spatial Mesh Observer";
            scriptMeshObserver = scriptDataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>(meshObserverName);

            scriptMeshObserver.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
            sasReady = true;
            UpdateMesh();
        }


        if (updateTimer > 5f && (permaMeshUpdate) )
        {
            UpdateMesh();
            updateTimer = 0;
        }
    }

    public void UpdateMesh()
    {
        if (!sasReady)
        {
            Debug.Log("SAS not ready yet");
            return;
        }
        foreach (GameObject mesh in meshCopyCollection)
        {
            Destroy(mesh);
        }
        meshCopyCollection = new GameObject[transformOpenSMO.childCount];
        for (int i = 0; i < transformOpenSMO.childCount; i++)
        {
            GameObject newMeshHolder = Instantiate(meshHolderPrefab, transform);
            MeshFilter meshFilter = newMeshHolder.GetComponent<MeshFilter>();
            Mesh newMesh = meshFilter.mesh;
            Mesh originalMesh = transformOpenSMO.GetChild(i).GetComponent<MeshFilter>().mesh;

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

        if (doesTreeSpawning && !spawnPlants.currentlySpawningTrees)
            StartCoroutine(spawnPlants.UpdateTrees(floorHeight));

        //start Rock spawning
        rockHolder.DeleteRocks();
        Mesh[] meshesMesh = new Mesh[meshCopyCollection.Length];
        for (int i=0; i < meshCopyCollection.Length;i++)
        {
            meshesMesh[i] = meshCopyCollection[i].GetComponent<MeshFilter>().mesh;
        }
        rockHolder.StartRockSpawning(meshesMesh);
    }
}
