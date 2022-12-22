using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TableInterpreter;

public class SpawnPond : MonoBehaviour
{
    public GameObject water;
    public GameObject waterfall;

    [SerializeField] TableInterpreter TI;
    [SerializeField] MeshCopySkript meshCopySkript;
    List<List<TwoInt>> pathsToEdge;
    List<float> pondRadii;
    List<int> candidateIndices;
    bool[,] isWater;

    //settings
    int minDistToEdge = 8;
    int maxNumberOfPonds = 3;

    //variables to copy from tableInterpeter
    Tile[,] tileHolder;
    List<List<TwoInt>> tileClusters;
    List<ExtraClusterInfo> extraClustersInfo;
    float rayInterval;


    void Start()
    {
        //TI = GetComponent<TableInterpreter>();
        rayInterval = TI.rayInterval;
        //meshCopySkript = GetComponent<MeshCopySkript>();
        candidateIndices = new List<int>();
        pathsToEdge = new List<List<TwoInt>>();
        pondRadii = new List<float>();
    }



    public void StartPondSpawning()
    {
        AccessTileInformation();
        Debug.Log("TIME afterPondAccessTileInfo " + Time.realtimeSinceStartup);
        FindClusterCandidates();
        Debug.Log("TIME afterFindClusterCandidates " + Time.realtimeSinceStartup);
        if (candidateIndices.Count > 0)
        {
            CreatePondInfo();
            Debug.Log("TIME afterCreatePondInfo " + Time.realtimeSinceStartup);
            SpawnPonds();
            Debug.Log("TIME afterSpawnPonds " + Time.realtimeSinceStartup);
        }
        
    }

    void AccessTileInformation()
    {
        tileHolder = TI.TileHolder;
        tileClusters = TI.TileClusters;
        extraClustersInfo = TI.ExtraClustersInfo;
        isWater = new bool[TI.rayDimension, TI.rayDimension];
    }

    void FindClusterCandidates()
    {
        for(int i=0; i<tileClusters.Count; i++)
        {
            if (extraClustersInfo[i].maxEdgeDist >= minDistToEdge) candidateIndices.Add(i);
        }
        while(candidateIndices.Count > maxNumberOfPonds)
        {
            int smalestEdgeDist = 10000000;
            foreach(int canddtadeIndex in candidateIndices)
            {
                if (extraClustersInfo[canddtadeIndex].maxEdgeDist < smalestEdgeDist) smalestEdgeDist = extraClustersInfo[canddtadeIndex].maxEdgeDist;
            }
            for (int j = 0; j < candidateIndices.Count; j++)
            {
                int index = candidateIndices[j];
                if (extraClustersInfo[j].maxEdgeDist == smalestEdgeDist) {
                    candidateIndices.RemoveAt(j);
                    break;
                }
            }
        }
    }

    void CreatePondInfo()
    {
        foreach(int clusterIndex in candidateIndices)
        {
            
            List<TwoInt> cluster = tileClusters[clusterIndex];
            ExtraClusterInfo cInfo = extraClustersInfo[clusterIndex];

            List<TwoInt> centerTiles = new List<TwoInt>();

            int highestEdgeDist = cInfo.maxEdgeDist;
            foreach(TwoInt tileRef in cluster)
            {
                if (tileHolder[tileRef.xi, tileRef.zi].distEdge == highestEdgeDist) centerTiles.Add(tileRef);
            }

            if (centerTiles.Count == 0) Debug.LogError("THE CLUSTER MAX EDGE DIST IS BIGGER THAN ITS ACTUALLY DEEPEST TILE");

            //NOTE: here we could instead pick the tile by how close it is to Hills or something like that
            TwoInt startingTile = centerTiles[Random.Range(0, centerTiles.Count)];
            int roomCenterAsTileindex = tileHolder.Length / 2 - 1;
            List<TwoInt> path = TI.FindPathOut(startingTile, 2, 1);//, new TwoInt(roomCenterAsTileindex,roomCenterAsTileindex));


            float pondRadius = minDistToEdge*rayInterval*0.3f + (highestEdgeDist - minDistToEdge) * rayInterval * 0.2f;
            float brookRadius = pondRadius / 3;

            pathsToEdge.Add(path);
            pondRadii.Add(pondRadius);
        }
    }


    void SpawnPonds()
    {
        for(int i=0; i< pathsToEdge.Count;i++)
        {
            int clusterIndex = candidateIndices[i];
            ExtraClusterInfo cInfo = extraClustersInfo[clusterIndex];
            float h = cInfo.averageH + meshCopySkript.floorHeight;
            float brookRad = pondRadii[i] / 2;
            float brookMinDist = brookRad / rayInterval *2;
            float secondElmToPondMinDist = pondRadii[i] * 1.5f / rayInterval;

            TwoInt firstElement = pathsToEdge[i][0];
            CraveHoleWithWater(pondRadii[i], firstElement.xi, firstElement.zi, h, true, clusterIndex);


            TwoInt prevElm = firstElement;
            bool spawnedSecondElm = false;
            /*
            foreach(TwoInt partOfPath in pathsToEdge[i])
            {
                Debug.Log("of Path" + i + " one element is at position " + partOfPath.xi + ", " + partOfPath.zi);


                GameObject placedWater = Instantiate(water, new Vector3(TI.IAsF(partOfPath.xi), h, TI.IAsF(partOfPath.zi)), Quaternion.identity);
                placedWater.transform.localScale *= 0.04f;
            }
            */

            //SPAWN OTHER STREAMPARTS HERE LATET!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            foreach (TwoInt partOfPath in pathsToEdge[i])
            {
                float distToPrev = Mathf.Sqrt((partOfPath.xi - prevElm.xi) * (partOfPath.xi - prevElm.xi) + (partOfPath.zi - prevElm.zi) * (partOfPath.zi - prevElm.zi));
                if(distToPrev>=secondElmToPondMinDist || spawnedSecondElm && distToPrev >= brookMinDist)
                {
                    spawnedSecondElm = true;
                    CraveHoleWithWater(brookRad, partOfPath.xi, partOfPath.zi, h, true, clusterIndex);
                    prevElm = partOfPath;
                }

            }

            TwoInt preLastElm = pathsToEdge[i][pathsToEdge[i].Count - 2];
            TwoInt lastElm = pathsToEdge[i][pathsToEdge[i].Count-1];
            CraveHoleWithWater(brookRad*0.7f, lastElm.xi, lastElm.zi, h, true, clusterIndex);
            Vector3 toLastElmDir = new Vector3(lastElm.xi - preLastElm.xi, 0, lastElm.zi - preLastElm.zi);
            Instantiate(waterfall, new Vector3(TI.IAsF(lastElm.xi), h - 0.07f, TI.IAsF(lastElm.zi)), Quaternion.LookRotation(toLastElmDir, Vector3.up));

        }
    }



    void CraveHoleWithWater(float radius, int middleXI, int middleZI , float h, bool withwWater, int clusterIndex)
    {
        Vector3 pos = new Vector3(TI.IAsF(middleXI), h, TI.IAsF(middleZI));

        radius *= 2.4f;
        //float h = pos.y; //MAYBE SET WATER LEVEL LOWER LATER!!!!!!!!!
        Vector3 spherePos = pos + Vector3.up * radius / 10;
        float maxChangeY = radius * 3 / 3;
        //MeshFilter[] meshFilters = transform.parent.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshFilter in meshCopySkript.meshFilterCollection)
        {
            float meshsScale = meshFilter.transform.localScale.x;
            //List<Vector3> meshVertices = meshFilter.mesh.GetVertices();
            Mesh deformingMesh = meshFilter.mesh;
            Vector3[] originalVertices = deformingMesh.vertices;
            Vector3[] displacedVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                displacedVertices[i] = originalVertices[i];
            }
            for (int i = 0; i < originalVertices.Length; i++)
            {
                float dist = (originalVertices[i] * meshsScale - spherePos).magnitude;
                if (dist < radius)
                {
                    displacedVertices[i] = originalVertices[i] + (Vector3.down * maxChangeY * (radius - dist) / radius) / meshsScale;
                }

            }
            deformingMesh.vertices = displacedVertices;

            if (withwWater)
            {
                //GameObject placedWater = Instantiate(water, pos, Quaternion.identity);
                //placedWater.transform.localScale *= radius; //Maybe Change radius here or adapt prefab!!!!!!!!!!!!!!!
                
                float radTileScaledFloat = radius / TI.rayInterval;
                int radTilescaled = (int)Mathf.Round(radTileScaledFloat);
                int maxXi = middleXI + radTilescaled;
                int maxZi = middleZI + radTilescaled;

                for(int xi=middleXI-radTilescaled; xi<=maxXi; xi++)
                {
                    for (int zi = middleZI - radTilescaled; zi <= maxZi; zi++)
                    {
                        float xiDist = xi - middleXI;
                        float ziDist = zi - middleZI;
                        if (Mathf.Sqrt(xiDist * xiDist + ziDist * ziDist) <= radTileScaledFloat)
                        {
                            if (!TI.IsOutOfBounds(xi, zi))
                            {
                                if (tileHolder[xi,zi].clusterIndex == clusterIndex)
                                {
                                    Instantiate(water, new Vector3(TI.IAsF(xi), h-0.07f, TI.IAsF(zi)), Quaternion.Euler(90,180,0), transform);
                                }
                            }
                        }
                    }
                }

                TI.MarkObjSpawnDist(middleXI, middleZI, radTilescaled);
            }
            
            deformingMesh.RecalculateNormals();
        }
    }

    public void DeletePonds()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}
