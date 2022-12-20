using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableSpawner : MonoBehaviour
{
    public SpawnPropabilities[] spawnables;

    public TableInterpreter TI;


    int[,] distToObj;


    public void ClearLists()
    {
        int dimensions = TI.rayDimension;
        distToObj = new int[dimensions, dimensions];
    }

    public void InitMassSpawning()
    {

    }
}
