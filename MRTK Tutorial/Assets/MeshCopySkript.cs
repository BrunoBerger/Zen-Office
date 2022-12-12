using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
    [HideInInspector]
    public Mesh[] meshesMeshCollection;
    [HideInInspector]
    public MeshFilter[] meshFilterCollection;
    bool sasReady = false;

    Transform transformOpenSMO;
    Transform transformSAS;
    IMixedRealitySpatialAwarenessSystem scriptSpatialAwarenessService;
    IMixedRealityDataProviderAccess scriptDataProviderAccess;
    IMixedRealitySpatialAwarenessMeshObserver scriptMeshObserver;

    float updateTimer;
    public float floorHeight;
    public List<float> verticiHeights;

    // Start is called before the first frame update
    void Start()
    {
        updateTimer = -6;
        floorHeight = float.MaxValue;
        sasReady = false;
        verticiHeights = new List<float>();
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
        Debug.Log("TIME1 " + Time.realtimeSinceStartup);
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
            Debug.Log("TIME "+i+" " + Time.realtimeSinceStartup);
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

                //neu
                verticiHeights.Add(vertices[v].y);


                
                //if (vertices[v].y < floorHeight)
                //{
                //    floorHeight = vertices[v].y;
                //}
            }
            //Debug.Log("floorHeight is: " + floorHeight);
            newMesh.SetUVs(0, uvs);
            meshCopyCollection[i] = newMeshHolder;
            
        }

        
        



        //TEST
        /*
        Debug.Log("TIME " + Time.realtimeSinceStartup);
        Mesh combinedMesh = new Mesh();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 1; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        Debug.Log("TIME " + Time.realtimeSinceStartup);
        MeshFilter newMF = meshCopyCollection[0].GetComponent<MeshFilter>();
        //meshCopyCollection[0].GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
        newMF.mesh.CombineMeshes(combine, true, true);
        Debug.Log("TIME " + Time.realtimeSinceStartup);
        newMF.mesh.Optimize();
        Debug.Log("TIME " + Time.realtimeSinceStartup);
        Vector3[] vertices = newMF.mesh.vertices;
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
        Debug.Log("floorHeight is: " + floorHeight);
        newMF.mesh.SetUVs(0, uvs);

        meshCopyCollection = new GameObject[] { meshCopyCollection[0] };
        */
        Debug.Log("TIME " + Time.realtimeSinceStartup);

        //new way to set floorheight
        float[] heights = verticiHeights.ToArray();
        Array.Sort(heights);
        floorHeight = heights[(int)((heights.Length - 1) * 0.05f)];
        Debug.Log("FloorHeight: " + floorHeight);


        meshesMeshCollection = new Mesh[meshCopyCollection.Length];
        meshFilterCollection = new MeshFilter[meshCopyCollection.Length];
        for (int j = 0; j < meshCopyCollection.Length; j++)
        {
            meshFilterCollection[j] = meshCopyCollection[j].GetComponent<MeshFilter>();
            meshesMeshCollection[j] = meshFilterCollection[j].mesh;
        }

        Debug.Log("TIME " + Time.realtimeSinceStartup);

        if (doesTreeSpawning && !spawnPlants.currentlySpawningTrees)
            StartCoroutine(spawnPlants.UpdateTrees(floorHeight));

        Debug.Log("TIMEafterPlants " + Time.realtimeSinceStartup);
        //start Rock spawning
        rockHolder.DeleteRocks();
        Debug.Log("TIMEafterDelrock " + Time.realtimeSinceStartup);
        rockHolder.StartRockSpawning(meshesMeshCollection);
        Debug.Log("TIMEafterRock " + Time.realtimeSinceStartup);


        GetComponent<TableInterpreter>().StartTableInterpretation(floorHeight);
        Debug.Log("TIME afterTableInterpr " + Time.realtimeSinceStartup);
        GetComponent<SpawnPond>().StartPondSpawning();
        Debug.Log("TIME afterPond " + Time.realtimeSinceStartup);
    }
}
