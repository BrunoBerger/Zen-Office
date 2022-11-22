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
    List<TwoInt> toCheck0;
    List<TwoInt> toCheck1;
    int currentlyCleared = 0;

    // Start is called before the first frame update
    void Start()
    {
        rayDimension = (int)(2 * gridRadius / rayInterval);
        tileHolder = new Tile[rayDimension, rayDimension];
        tileClusters = new List<List<Tile>>();
        currentCluster = new List<Tile>();
        toCheck0 = new List<TwoInt>();
        toCheck1 = new List<TwoInt>();
        
        //Debug.Log("holder.length is: " + tileHolder[].Length+"    and should be: "+holderSize);
    }

    // Update is called once per frame
    public void StartTableInterpretation(float floorLevel)
    {
        this.floorLevel = floorLevel;
        for(int xRough=0; xRough < rayDimension; xRough += firstAbstraction)
        {
            for (int zRough = 0; zRough < rayDimension; zRough += firstAbstraction)
            {
                //Debug.Log("outer For at " + xRough + ", " + zRough);
                TwoInt startPoint = new TwoInt(xRough, zRough);
                toCheck0.Add(startPoint);
                CheckAndCollectNeighbours(startPoint);
                while (toCheck0.Count > 0 || toCheck1.Count > 0)
                {
                    if (currentlyCleared == 0)
                    {
                        foreach(TwoInt xz in toCheck0)
                        {
                            CheckAndCollectNeighbours(xz);
                        }
                        currentlyCleared = 1;
                    }
                    else
                    {
                        foreach (TwoInt xz in toCheck1)
                        {
                            CheckAndCollectNeighbours(xz);
                        }
                        currentlyCleared = 0;
                    }
                }
                if (currentCluster.Count > 0)
                {
                    tileClusters.Add(new List<Tile>(currentCluster));
                    currentCluster.Clear();
                    Debug.Log("created new cluster: " + clusterIndex);
                    clusterIndex++;
                }
            }
        }
    }


    //does only return unchecked tiles and unchecked neighbours (and marks them as checked)
    void CheckAndCollectNeighbours(TwoInt xz)
    {
        int xi = xz.xi;
        int zi = xz.zi;

        Tile? testedTile;
        testedTile = TryCreateTile(xi, zi);
        if (testedTile!=null)
        {
            TryToAdd(xi + 1, zi);
            TryToAdd(xi - 1, zi);
            TryToAdd(xi, zi + 1);
            TryToAdd(xi, zi - 1);
            currentCluster.Add((Tile)testedTile);
        }

        if (currentlyCleared == 0) toCheck0.Remove(xz);
        else toCheck1.Remove(xz);





        //    Tile currentTile = tileHolder[xi, zi];
        //    List<Tile> meAndNeighbours = new List<Tile> { currentTile };
        //    List<Tile>[] neighbours = new List<Tile>[] { new List<Tile>(), new List<Tile>(), new List<Tile>(), new List<Tile>() };

        //    neighbours[0] = CheckAndCollectNeighbours(xi + 1, zi, fromX, fromZ);
        //    neighbours[1] = CheckAndCollectNeighbours(xi - 1, zi, fromX, fromZ);
        //    neighbours[2] = CheckAndCollectNeighbours(xi, zi + 1, fromX, fromZ);
        //    neighbours[3] = CheckAndCollectNeighbours(xi, zi - 1, fromX, fromZ);

        //    SetEdgenessEtc(ref currentTile, xi + 1, zi);
        //    SetEdgenessEtc(ref currentTile, xi - 1, zi);
        //    SetEdgenessEtc(ref currentTile, xi, zi + 1);
        //    SetEdgenessEtc(ref currentTile, xi, zi - 1);
        //    if (visualDebugging != VisualDebugging.no) VisuallyDebugTile(ref currentTile, xi, zi);

        //    foreach (List<Tile> connectedTiles in neighbours)
        //    {
        //        foreach (Tile connectedTile in connectedTiles)
        //        {
        //            meAndNeighbours.Add(connectedTile);
        //        }
        //    }

        //    return meAndNeighbours;
        //}
        //else return new List<Tile>(); //return empty list, if pixle was already checked or is not part of a star at all
    }


    void TryToAdd(int xi, int zi)
    {
        if (!IsOutOfBounds(xi, zi))
        {
            if (!tileHolder[xi, zi].tested)
            {
                if (currentlyCleared == 1) toCheck0.Add(new TwoInt(xi,zi));
                else toCheck1.Add(new TwoInt(xi,zi));

            }
        }
    }

    Tile? TryCreateTile(int xi, int zi)
    {
        //Debug.Log("checktile at " + xi + "," + zi);
        if (IsOutOfBounds(xi,zi)) return null; //prevent index out of bounds
        //Debug.Log("in bound checktile at " + xi + "," + zi);
        if (tileHolder[xi, zi].tested) return null;
        //Debug.Log("untested tile at " + xi + "," + zi);
        RaycastHit hitInfo;

        if (Physics.Raycast(new Vector3(xi*rayInterval-gridRadius, floorLevel+2  , zi * rayInterval - gridRadius), Vector3.down, out hitInfo, 3, meshLayer))
        {
            //Debug.Log("try create tile at " + xi + "," + zi);
            Tile testedTile = new Tile(hitInfo.point.y - floorLevel, xi, zi, clusterIndex);
            if (testedTile.state == State.fine)
            {
                VisuallyDebugTile(ref testedTile, xi, zi);
                return testedTile;
            }
            else return null;
        }
        else return null;
    }

    bool IsOutOfBounds(int xi, int zi)
    {
        if (xi < 0 || zi < 0 || xi > rayDimension || zi > rayDimension) return true;
        else return false;
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
        public int xi;
        public int zi;
        //public float fromX;
        //public float fromZ;
        public State state;
        public bool valid;
        public int distToEdge;
        public int clusterIndex;
        public int edgeness;
        public int onHillness;
        public int onRiftness;
        
        public Tile(float h, int clusterIndex, int xi, int zi)
        {
            tested = true;
            this.h = h;
            this.xi = xi;
            this.zi = zi;
            //this.fromX = fromX;
            //this.fromZ = fromZ;
            state = h < 0.65f ? State.tooLow : h < 0.9f ? State.fine : State.tooHigh;
            valid = (state == State.fine);
            distToEdge = -1;
            this.clusterIndex = clusterIndex;
            edgeness = 0;
            onHillness = 0;
            onRiftness = 0;
        }
    }

    struct TwoInt
    {
        public int xi;
        public int zi;
        public TwoInt(int xi, int zi)
        {
            this.xi = xi;
            this.zi = zi;
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
