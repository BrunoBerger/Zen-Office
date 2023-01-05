using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TableInterpreter;

public class SpawnableSpawner : MonoBehaviour
{
    public SpawnPropabilities[] spawnables;

    public TableInterpreter TI;

    int dimensions;
    float rayInterval;
    float gridRadius;
    float floorLevel;
    float tableHeight;
    LayerMask meshLayer;
    //int[,] distsToObj;

    public void PrepareSpawnableSpawner()
    {
        dimensions = TI.rayDimension;
        rayInterval = TI.rayInterval;
        gridRadius = TI.gridRadius;
        floorLevel = TI.floorLevel;
        tableHeight = TI.tableHeight; //WICHIG: SET TABLE HEIGHT HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        meshLayer = TI.meshLayer;
        //distsToObj = new int[dimensions, dimensions];
        
    }

    public void InitMassSpawning()
    {
        if (spawnables.Length == 0) Debug.LogError("you did not assign SpawnPropabilities to the spawnableSpawner");
        List<int> shuffeledSpawnableIndices = Enumerable.Range(0, spawnables.Length).ToList();
        //Debug.Log("shuffled indices lenght is " + shuffeledSpawnableIndices.Count);
        bool[,] checkedTiles = new bool[dimensions, dimensions];
        //int[,] distsToObj = TI.distToObj;
        Tile[,] tileHolder = TI.TileHolder;
        for (int exp = 4; exp > -1; exp--) //checking each 16th,8th,4th,second,single tile
        {
            for (int xi = 0; xi < dimensions; xi++)
            {
                if (xi % Mathf.Pow(2, exp) != 0) continue;
                for (int zi = 0; zi < dimensions; zi++)
                {
                    if (zi % Mathf.Pow(2, exp) != 0) continue;
                    //Debug.Log("check massSpawning at" + xi + "," + zi);
                    if (checkedTiles[xi, zi]) continue;
                    checkedTiles[xi, zi] = true;
                    int distToObj = TI.distToObj[xi, zi];
                    if (distToObj == 0) continue;

                    RaycastHit hitInfo;
                    if (!Physics.Raycast(new Vector3(xi * rayInterval - gridRadius, floorLevel + 2, zi * rayInterval - gridRadius), Vector3.down, out hitInfo, 3, meshLayer)) continue;
                    //Debug.Log("ray was shot at " + xi + "," + zi + " hitheight:" + hitInfo.point.y);
                    
                    Tile tile = tileHolder[xi, zi];
                    //Debug.Log("after tileset");
                    bool isRockySurface = hitInfo.normal.y < 0.25f;
                    bool onTable = tile.state == State.fine;
                    int edgeDist = tile.distEdge;
                    int hillDist = tile.distHill;
                    int riftDist = tile.distRift;
                    int hillness = tile.hillness;
                    int riftness = tile.riftness;
                    float hFromTable = hitInfo.point.y - tableHeight;
                    float hFromFloor = hitInfo.point.y - floorLevel;


                    int indexOfSpawnable = -1;
                    //shuffle spawnable candidates for fair chances
                    shuffeledSpawnableIndices = shuffeledSpawnableIndices.OrderBy(i => Random.value).ToList();
                    for(int i = 0; i<spawnables.Length; i++)
                    {
                        //Debug.Log("shuffle read "+i);
                        int testedI = shuffeledSpawnableIndices[i];
                        //Debug.Log("beforeProp check");
                        float propability = spawnables[testedI].GetPropability(hFromFloor, hFromTable, onTable, isRockySurface, edgeDist, hillDist, riftDist, distToObj, hillness, riftness, xi, zi);
                        //Debug.Log("afterProp check");
                        if (propability == 0) continue;
                        if(propability>= Random.Range(0.0000000001f, 1f))
                        {
                            indexOfSpawnable = testedI;
                            break;
                        }
                    }

                    if (indexOfSpawnable == -1) continue;

                    SpawnPropabilities spawnable = spawnables[indexOfSpawnable];
                    if (spawnable.objects.Length == 0) Debug.LogError("you did not assign objects to Spawn to the spawnPropability"+indexOfSpawnable);

                    Vector3 relativeUp = spawnable.tiltWithFloor ? hitInfo.normal : Vector3.up;

                    //following 6 lines from elenzil at https://answers.unity.com/questions/1618126/given-a-vector-how-do-i-generate-a-random-perpendi.html (15.11.2022)
                    float du = Vector3.Dot(relativeUp, Vector3.up);
                    float df = Vector3.Dot(relativeUp, Vector3.forward);
                    Vector3 v1 = Mathf.Abs(du) < Mathf.Abs(df) ? Vector3.up : Vector3.forward;
                    Vector3 v2 = Vector3.Cross(v1, relativeUp);
                    float degrees = Random.Range(0.0f, 360.0f);
                    v2 = Quaternion.AngleAxis(degrees, relativeUp) * v2;
                    Quaternion spawnRot = Quaternion.LookRotation(v2, relativeUp);

                    Instantiate(spawnable.objects[Random.Range(0, spawnable.objects.Length)], new Vector3(TI.IAsF(xi), hitInfo.point.y, TI.IAsF(zi)), spawnRot, transform); //TO DO: ALLOW ROTATION WITH GROUND NORMAL
                    TI.MarkObjSpawnDist(xi, zi, spawnable.radius, spawnable.distToOtherObj.min);
                }
            }
        }

        
    }

    public void DeleteSpawnables()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
