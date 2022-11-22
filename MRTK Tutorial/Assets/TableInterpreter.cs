using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class TableInterpreter : MonoBehaviour
{
    [HideInInspector] public float floorLevel;
    public LayerMask meshLayer;
    public VisualDebugging visualDebugging;
    public GameObject[] debuggingObjects;
    float rayInterval = 0.05f;
    int firstAbstraction = 6;
    int rayDimension;
    float gridRadius = 3;
    int clusterIndex = 1;
    Tile[,] tileHolder;
    List<List<Tile>> tileClusters;
    List<Tile> currentCluster;
    //bool[,] roughChecked;
    //List<int> densesRefs;

    // Start is called before the first frame update
    void Start()
    {
        int rayDimension = (int)(2 * gridRadius / rayInterval);
        tileHolder = new Tile[rayDimension, rayDimension];
        tileClusters = new List<List<Tile>>();
        currentCluster = new List<Tile>();
        //Debug.Log("holder.length is: " + tileHolder[].Length+"    and should be: "+holderSize);
    }

    // Update is called once per frame
    public void StartTableInterpretation(int floorLevel)
    {
        this.floorLevel = floorLevel;
        for(int xRough=0; xRough < rayDimension; xRough += firstAbstraction)
        {
            for (int zRough = 0; zRough < rayDimension; zRough += firstAbstraction)
            {
                currentCluster = GetTileAndNeighbours(xRough, zRough);
                if (currentCluster.Count > 0)
                {
                    tileClusters.Add(new List<Tile>(currentCluster));
                    currentCluster.Clear();
                    clusterIndex++;
                }
            }
        }
    }


    //does only return unchecked tiles and unchecked neighbours (and marks them as checked)
    List<Tile> GetTileAndNeighbours(int xi, int zi)
    {
        Tile currentTile = tileHolder[xi, zi];

        if (CheckTile(xi, zi))
        {
            List<Tile> meAndNeighbours = new List<Tile> { currentTile };
            List<Tile>[] neighbours = new List<Tile>[] { new List<Tile>(), new List<Tile>(), new List<Tile>(), new List<Tile>() };

            neighbours[0] = GetTileAndNeighbours(xi + 1, zi);
            neighbours[1] = GetTileAndNeighbours(xi - 1, zi);
            neighbours[2] = GetTileAndNeighbours(xi, zi + 1);
            neighbours[3] = GetTileAndNeighbours(xi, zi - 1);

            SetEdgenessEtc(ref currentTile, xi + 1, zi);
            SetEdgenessEtc(ref currentTile, xi - 1, zi);
            SetEdgenessEtc(ref currentTile, xi, zi + 1);
            SetEdgenessEtc(ref currentTile, xi, zi - 1);
            if (visualDebugging != VisualDebugging.no) VisuallyDebugTile(ref currentTile, xi, zi);

            foreach (List<Tile> connectedTiles in neighbours)
            {
                foreach (Tile connectedTile in connectedTiles)
                {
                    meAndNeighbours.Add(connectedTile);
                }
            }

            return meAndNeighbours;
        }
        else return new List<Tile>(); //return empty list, if pixle was already checked or is not part of a star at all
    }


    bool CheckTile(int xi, int zi)
    {
        if (IsOutOfBounds(xi,zi)) return false; //prevent index out of bounds
        if (tileHolder[xi, zi].tested) return false;

        RaycastHit hitInfo;

        if (Physics.Raycast(new Vector3(xi*rayInterval-gridRadius, floorLevel+2  , zi * rayInterval - gridRadius), Vector3.down, out hitInfo, 3, meshLayer))
        {
            Tile testedTile = new Tile(hitInfo.point.y - floorLevel, clusterIndex);
            return testedTile.state == State.fine;
        }
        else return false;
    }

    bool IsOutOfBounds(int xi, int zi)
    {
        if (xi < 0 || zi < 0 || xi > rayDimension || zi > rayDimension) return false;
        else return true;
    }

    void SetEdgenessEtc(ref Tile tile, int neighbourX, int neighbourY)
    {
        if (IsOutOfBounds(neighbourX, neighbourY)) tile.edgeness++;
        else
        {
            if (tileHolder[neighbourX, neighbourY].state == State.tooHigh)
            {
                tile.edgeness++;
                tile.onHillness++;
            }
            else if (tileHolder[neighbourX, neighbourY].state == State.tooLow)
            {
                tile.edgeness++;
                tile.onRiftness++;
            }
        }
    }


    void VisuallyDebugTile(ref Tile tile, int xi, int zi)
    {
        int selectIndex = 0;
        if (visualDebugging == VisualDebugging.byCluster)
        {
            selectIndex = tile.clusterIndex % debuggingObjects.Length;
        }else if (visualDebugging == VisualDebugging.byEdgeness)
        {
            selectIndex = tile.edgeness % debuggingObjects.Length;
        }
        else if (visualDebugging == VisualDebugging.byHillness)
        {
            selectIndex = tile.onHillness % debuggingObjects.Length;
        }
        else if (visualDebugging == VisualDebugging.byRiftness)
        {
            selectIndex = tile.onRiftness % debuggingObjects.Length;
        }
        Instantiate(debuggingObjects[selectIndex], new Vector3(xi * rayInterval - gridRadius, tile.h + floorLevel, zi * rayInterval - gridRadius), Quaternion.identity);
    }

    struct Tile
    {
        public bool tested;
        public float h;
        public State state;
        public bool valid;
        public int distToEdge;
        public int clusterIndex;
        public int edgeness;
        public int onHillness;
        public int onRiftness;
        
        public Tile(float h, int clusterIndex)
        {
            tested = true;
            this.h = h;
            state = h < 0.65f ? State.tooLow : h < 0.9f ? State.fine : State.tooHigh;
            valid = (state == State.fine);
            distToEdge = -1;
            this.clusterIndex = clusterIndex;
            edgeness = 0;
            onHillness = 0;
            onRiftness = 0;
        }
    }

    enum State
    {
        notTested,
        fine,
        tooHigh,
        tooLow
    }


    enum CheckResult
    {
        testedFine,
        testedBad,
        fine,
        bad
        
    }

    public enum VisualDebugging
    {
        no,
        byCluster,
        byEdgeness,
        byHillness,
        byRiftness
    }
}
