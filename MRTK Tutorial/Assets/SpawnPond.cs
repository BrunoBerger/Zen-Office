using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TableInterpreter;

public class SpawnPond : MonoBehaviour
{

    TableInterpreter TI;
    List<List<TwoInt>> pathToEdge;
    List<int> candidateIndices;

    //settings
    int minDistToEdge = 8;
    int maxNumberOfPonds = 3;

    //variables to copy from tableInterpeter
    Tile[,] tileHolder;
    List<List<TwoInt>> tileClusters;
    List<ExtraClusterInfo> extraClustersInfo;


    void Start()
    {
        TI = GetComponent<TableInterpreter>();
        candidateIndices = new List<int>();
    }



    public void StartPondSpawning()
    {
        AccessTileInformation();
        FindClusterCandidates();
        if (candidateIndices.Count > 0)
        {
            CreatePondInfo();
        }
        
    }

    void AccessTileInformation()
    {
        tileHolder = TI.TileHolder;
        tileClusters = TI.TileClusters;
        extraClustersInfo = TI.ExtraClustersInfo;
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
            List<TwoInt> path = TI.FindPathOut(startingTile, 2, 1, new TwoInt(roomCenterAsTileindex,roomCenterAsTileindex));
















        }
    }


}
