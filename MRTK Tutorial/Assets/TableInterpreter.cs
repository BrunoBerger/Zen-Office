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
    List<List<TwoInt>> tileClusters;
    List<TwoInt> currentCluster;
    //bool[,] roughChecked;
    //List<int> densesRefs;
    List<TwoInt> toCheck0;
    List<TwoInt> toCheck1;
    int currentlyCleared = 0;
    bool[,] TriedToAddOnce;

    // Start is called before the first frame update
    void Start()
    {
        rayDimension = (int)(2 * gridRadius / rayInterval);
        tileHolder = new Tile[rayDimension, rayDimension];
        tileClusters = new List<List<TwoInt>>();
        currentCluster = new List<TwoInt>();
        toCheck0 = new List<TwoInt>();
        toCheck1 = new List<TwoInt>();
        TriedToAddOnce = new bool[rayDimension,rayDimension];
        //Debug.Log("holder.length is: " + tileHolder[].Length+"    and should be: "+holderSize);
    }

    // Update is called once per frame
    public void StartTableInterpretation(float floorlevel)
    {
        floorLevel = floorlevel;
        for(int xRough=0; xRough < rayDimension; xRough += firstAbstraction)
        {
            for (int zRough = 0; zRough < rayDimension; zRough += firstAbstraction)
            {

                TryToAdd(xRough, zRough);
                while (toCheck0.Count > 0 || toCheck1.Count > 0)
                {
                    if (currentlyCleared == 0)
                    {
                        while (toCheck0.Count > 0) //NOTE: might be more performant to run with a for loop backwards
                        {
                            CheckAndCollectNeighbours(toCheck0[0]);
                        }
                        currentlyCleared = 1;
                    }
                    else
                    {
                        while (toCheck1.Count > 0)//NOTE: same here
                        {
                            CheckAndCollectNeighbours(toCheck1[0]);
                        }
                        currentlyCleared = 0;
                    }
                }
                if (currentCluster.Count > 0)
                {
                    tileClusters.Add(new List<TwoInt>(currentCluster));
                    currentCluster.Clear();
                    Debug.Log("created new cluster: " + clusterIndex);
                    clusterIndex++;
                }
            }
        }
        CalcAllEdgenessesEtc();

        CalcEdgeDistEtc();

        if (visualDebugging != VisualDebugging.no) VisualyDebugAll();

    }

    void CalcEdgeDistEtc()
    {

        for(int i=0; i<2;i++)
        {
            bool byHill = (i == 0);
            TriedToAddOnce = new bool[rayDimension, rayDimension];
            
            foreach (List<TwoInt> cluster in tileClusters)
            {
                currentlyCleared = 0;
                foreach (TwoInt tile in cluster)
                {
                    int xi = tile.xi;
                    int zi = tile.zi;

                    
                    
                    if (tileHolder[xi, zi].hillness > 0 && byHill || tileHolder[xi, zi].riftness >0 && !byHill)
                    {

                        //DEBUG RAYS:
                        //int c = tileHolder[xi, zi].clusterIndex % 4;
                        //Color clusterColor = c == 0 ? Color.gray : c == 1 ? Color.green : c == 2 ? Color.blue : Color.red;
                        //Debug.DrawRay(new Vector3(xi * rayInterval - gridRadius, tileHolder[xi, zi].h + floorLevel, zi * rayInterval - gridRadius), Vector3.up, clusterColor, 1000000);

                        toCheck0.Add(new TwoInt(xi, zi));
                        TriedToAddOnce[xi, zi]=true;
                    }

                }

                while (toCheck0.Count > 0 || toCheck1.Count > 0)
                {
                    if (currentlyCleared == 0)
                    {
                        while (toCheck0.Count > 0) //NOTE: might be more performant to run with a for loop backwards
                        {
                            SetDistAndAddNeighbours(toCheck0[0], byHill);
                        }
                        currentlyCleared = 1;
                    }
                    else
                    {
                        while (toCheck1.Count > 0)//NOTE: same here
                        {
                            SetDistAndAddNeighbours(toCheck1[0], byHill);
                        }
                        currentlyCleared = 0;
                    }
                }
            }
            
        }
        foreach (List<TwoInt> cluster in tileClusters)
        {
            foreach (TwoInt tile in cluster)
            {
                int xi = tile.xi;
                int zi = tile.zi;
                int distHill = tileHolder[xi, zi].distHill;
                int distRift = tileHolder[xi, zi].distRift;
                tileHolder[xi, zi].distEdge = distHill < distRift ? distHill : distRift;
            }
        }

    }

    void SetDistAndAddNeighbours(TwoInt xz, bool byHill)
    {
        int xi = xz.xi;
        int zi = xz.zi;

        int dist = byHill ? tileHolder[xi, zi].distHill : tileHolder[xi, zi].distRift;

        TryToAddForDist(dist, byHill, xi + 1, zi);
        TryToAddForDist(dist, byHill, xi - 1, zi);
        TryToAddForDist(dist, byHill, xi, zi + 1);
        TryToAddForDist(dist, byHill, xi, zi - 1);


        if (currentlyCleared == 0)
            toCheck0.Remove(xz);
        else
            toCheck1.Remove(xz);
    }
    void TryToAddForDist(int prevIndex, bool byHill, int xi, int zi)
    {
        if (!IsOutOfBounds(xi, zi))
        {
            if (!TriedToAddOnce[xi, zi])
            {
                TriedToAddOnce[xi, zi] = true;
                if (tileHolder[xi, zi].state == State.fine)
                {
                    if (byHill) tileHolder[xi, zi].distHill = prevIndex + 1;
                    else tileHolder[xi, zi].distRift = prevIndex + 1;

                    if (currentlyCleared == 1) toCheck0.Add(new TwoInt(xi, zi));
                    else toCheck1.Add(new TwoInt(xi, zi));
                }

            }
        }
    }

    //does only return unchecked tiles and unchecked neighbours (and marks them as checked)
    void CheckAndCollectNeighbours(TwoInt xz)
    {
        int xi = xz.xi;
        int zi = xz.zi;

        TwoInt? testedTile;
        testedTile = TryCreateTile(xi, zi);
        if (testedTile!=null)
        {
            TryToAdd(xi + 1, zi);
            TryToAdd(xi - 1, zi);
            TryToAdd(xi, zi + 1);
            TryToAdd(xi, zi - 1);
            currentCluster.Add((TwoInt)testedTile);
        }


        if (currentlyCleared == 0)
            toCheck0.Remove(xz);
        else
            toCheck1.Remove(xz);


    }


    void TryToAdd(int xi, int zi)
    {
        //Debug.Log("beforeBoundcheck");
        if (!IsOutOfBounds(xi, zi))
        {
            //Debug.Log("afterBoundcheck");
            if (!TriedToAddOnce[xi, zi])
            {
                TriedToAddOnce[xi, zi] = true;
                if (!tileHolder[xi, zi].tested)
                {
                    //Debug.Log("tryToAdd: " + xi + "," + zi + "   currentlyCleared is " + currentlyCleared);
                    if (currentlyCleared == 1) toCheck0.Add(new TwoInt(xi, zi));
                    else toCheck1.Add(new TwoInt(xi, zi));
                    //Debug.Log("afterTryToAdd");
                }
            }
        }
        //else Debug.Log("tryToAdd: " + xi + "," + zi + " but it was out of bounds");
    }

    TwoInt? TryCreateTile(int xi, int zi)
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
            tileHolder[xi, zi] = testedTile;
            if (testedTile.state == State.fine)
            {
                //Debug.Log("try visualyDebug tile at " + xi + "," + zi);

                return new TwoInt(testedTile.xi, testedTile.zi);
            }
            else
            {
                return null;
            }
        }
        else return null;
    }

    bool IsOutOfBounds(int xi, int zi)
    {
        if (xi < 0 || zi < 0 || xi > rayDimension-1 || zi > rayDimension-1) return true;
        else return false;
    }


    void CalcAllEdgenessesEtc()
    {
        foreach(List<TwoInt> cluster in tileClusters)
        {
            foreach(TwoInt tile in cluster)
            {
                int tileXi = tile.xi;
                int tileZi = tile.zi;
                SetEdgenessEtcByNeighbour(tileXi, tileZi, 1, 0);
                SetEdgenessEtcByNeighbour(tileXi, tileZi, -1, 0);
                SetEdgenessEtcByNeighbour(tileXi, tileZi, 0, 1);
                SetEdgenessEtcByNeighbour(tileXi, tileZi, 0, -1);
                //VisuallyDebugTile(tileXi, tileZi);
            }
        }
    }

    void SetEdgenessEtcByNeighbour(int xi, int zi, int changeX, int changeZ)
    {
        int neighbourX = xi + changeX;
        int neighbourZ = zi + changeZ;
        Tile tile = tileHolder[xi, zi];
        if (IsOutOfBounds(neighbourX, neighbourZ)) tileHolder[xi, zi].edgeness++;
        else
        {
            if (tileHolder[neighbourX, neighbourZ].state == State.tooHigh)
            {
                tileHolder[xi, zi].edgeness++;
                tileHolder[xi, zi].hillness++;
                //Debug.Log("Set hillness to: "+ tileHolder[xi, zi].hillness);
            }
            else if (tileHolder[neighbourX, neighbourZ].state == State.tooLow)
            {
                tileHolder[xi, zi].edgeness++;
                tileHolder[xi, zi].riftness++;
                //Debug.Log("Set riftness to: " + tileHolder[xi, zi].riftness);
                
            }
            else if (tileHolder[neighbourX, neighbourZ].state == State.notTested)
            {
                //Debug.LogError("Checking unchecked Tile as neighbour");
            }
        }
    }


    void VisuallyDebugTile(int xi, int zi)
    {
        Tile tile = tileHolder[xi,zi];
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
            selectIndex = tile.hillness % debuggingObjects.Length;
        }
        else if (visualDebugging == VisualDebugging.byRiftness)
        {
            selectIndex = tile.riftness % debuggingObjects.Length;
        }

        else if (visualDebugging == VisualDebugging.byEdgeDist)
        {
            selectIndex = tile.distEdge % debuggingObjects.Length;
        }
        else if (visualDebugging == VisualDebugging.byHillDist)
        {
            selectIndex = tile.distHill % debuggingObjects.Length;
        }
        else if (visualDebugging == VisualDebugging.byRiftDist)
        {
            selectIndex = tile.distRift % debuggingObjects.Length;
        }
        Instantiate(debuggingObjects[selectIndex], new Vector3(xi * rayInterval - gridRadius, tile.h + floorLevel, zi * rayInterval - gridRadius), Quaternion.identity);
    }

    void VisualyDebugAll()
    {
        foreach (List<TwoInt> cluster in tileClusters)
        {
            foreach (TwoInt tile in cluster)
            {
                VisuallyDebugTile(tile.xi, tile.zi);
            }
        }
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
        //public int distToEdge;
        public int clusterIndex;

        public int edgeness;
        public int hillness;
        public int riftness;

        public int distEdge;
        public int distHill;
        public int distRift;
        public Tile(float h, int xi, int zi, int clusterIndex)
        {
            tested = true;
            this.h = h;
            this.xi = xi;
            this.zi = zi;
            //this.fromX = fromX;
            //this.fromZ = fromZ;
            state = h < 0.65f ? State.tooLow : h < 1.2f ? State.fine : State.tooHigh;
            valid = (state == State.fine);
            //distToEdge = -1;
            this.clusterIndex = clusterIndex;

            edgeness = 0;
            hillness = 0;
            riftness = 0;

            distEdge = 0;
            distHill = 0;
            distRift = 0;
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
        byEdgeDist,
        byHillDist,
        byRiftDist,
        byEdgeness,
        byHillness,
        byRiftness
    }
}
