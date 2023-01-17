using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Object Spawn Preset", menuName = "ScriptableObjects/Create Spawn Probability")]
public class SpawnPropabilities : ScriptableObject
{
    [Range(0, 1)]
    public float propabilityScale = 0.3f;

    [Header("height criteria")]
    public bool pivotsHeightOnFloor = false;
    public SoftRangeVarM2P2 heightRange;

    [Header("noisemap criteria")]
    public SoftRangeVar01 noisemapRange;
    public float noiseScale = 1;
    public Vector2 noiseOffset;

    [Header("Minimum Distances (Md)")]
    [Range(1, 5)]
    public int radius = 1;
    //[Range(1, 10)]
    //public int mdOtherObj = 0;
    //[Range(0, 10)]
    //public int mdEdge = 0;
    //[Range(0, 10)]
    //public int mdRift = 0;
    //[Range(0, 10)]
    //public int mdHill = 0;
    public MinMaxRange distToOtherObj;
    [Range(0, 2)]
    [Tooltip("0 = does only spawn on tables, 1 = does only spawn outside of tables, 2 = spawns on both kind of surfaces")]
    public int spawnsOutsideTable = 0;
    public MinMaxRange distToEdge;
    public MinMaxRange distToHill;
    public MinMaxRange distToRift;

    //[Tooltip("how many of its four sides adjoin a rift")]
    //[Range(0, 4)]
    //public int riftness = 0;
    //[Tooltip("how many of its four sides adjoin a hill")]
    //[Range(0, 4)]
    //public int hillness = 0;


    [Header("tilt criteria")]
    [Range(0, 2)]
    [Tooltip("0 = does not spawn on rocky surfaces, 1 = does only spawn on rocky surfaces, 2 = spawns on both kind of surfaces")]
    public int spawnsOnRocks = 0;
    public bool tiltWithFloor = false;
    [Range(0f,90f)]
    public float maxTilt = 30f;
    [Range (-0.02f,0.01f)]
    public float spawnedWithOffHeight = 0f;
    [Space]
    [Range(0, 2)]
    [Tooltip("0 = does not spawn infront of a desktop, 1 = does only spawn infront of a desktop, 2 = can spawn both infront of and away from a desktop")]
    public int spawnInfrontDesktop = 0;

    [Header("Spawnable Objects")]
    public GameObject[] objects;




    private void OnValidate()
    {
        heightRange.AdaptDependentVars();
        noisemapRange.AdaptDependentVars();
        distToEdge.AdaptDependentVars();
        distToHill.AdaptDependentVars();
        distToRift.AdaptDependentVars();
    }


    public float GetPropability(float hFromFloor, float hFromTable, bool isOnTable, bool isRockSurface, bool isInDesktopArea, int edgeDist, int hillDist, int riftDist, int distToObj, int hillness, int riftness, int xi, int zi)
    {
        float propability = propabilityScale;

        //can only spawn on intended kind of surface
        if ( isRockSurface && spawnsOnRocks == 0) return 0;
        if (!isRockSurface && spawnsOnRocks == 1) return 0;
        if (isInDesktopArea && spawnInfrontDesktop == 0) return 0;
        if (!isInDesktopArea && spawnInfrontDesktop == 1) return 0;
        if (!isOnTable && spawnsOutsideTable == 0) return 0;
        if ( isOnTable && spawnsOutsideTable == 1) return 0;

        //if (mdOtherObj + radius-1 > distToObj) return 0;
        if (!distToOtherObj.IsInRange(distToObj, radius)) return 0;
        if (isOnTable)
        {
            //if (mdEdge + radius - 1 > edgeDist) return 0;
            //if (mdHill + radius - 1 > hillDist) return 0;
            //if (mdRift + radius - 1 > riftDist) return 0;
            if (!distToEdge.IsInRange(edgeDist, radius)) return 0;
            if (!distToHill.IsInRange(hillDist, radius)) return 0;
            if (!distToRift.IsInRange(riftDist, radius)) return 0;

        }

        propability *= heightRange.GetPropability(pivotsHeightOnFloor?hFromFloor:hFromTable);
        if (propability == 0) return 0;

        float noiseSample = Mathf.PerlinNoise(xi * noiseScale + noiseOffset.x, zi * noiseScale + noiseOffset.y);
        propability *= noisemapRange.GetPropability(noiseSample);

        return propability;
    }


}

//NOTE: changes in this class should be applied to "SoftRangeVarM2P2" aswell
[Serializable]
public class SoftRangeVar01
{
    [Range (0,1)]
    public float lowCap = 0;
    [Range(0, 1)]
    public float lowDecay = 0.2f;    
    [Range(0, 1)]
    public float highDecay = 0.8f;
    [Range(0, 1)]
    public float highCap = 1;


    float prevLC = 0;
    float prevLD = 0.2f;
    float prevHD = 0.8f;
    float prevHC = 1;


    public void AdaptDependentVars()
    {
        if (lowCap != prevLC)
        {
            if (lowDecay < lowCap) lowDecay = lowCap;
            if (highDecay < lowCap) highDecay = lowCap;
            if (highCap < lowCap) highCap = lowCap;
        }
        else if (lowDecay != prevLD)
        {
            if (lowCap > lowDecay) lowCap = lowDecay;
            if (highDecay < lowDecay) highDecay = lowDecay;
            if (highCap < lowDecay) highCap = lowDecay;
        }
        else if (highDecay != prevHD)
        {
            if (lowCap > highDecay) lowCap = highDecay;
            if (lowDecay > highDecay) lowDecay = highDecay;
            if (highCap < highDecay) highCap = highDecay;
        }
        else if (highCap != prevHC)
        {
            if (lowCap > highCap) lowCap = highCap;
            if (lowDecay > highCap) lowDecay = highCap;
            if (highDecay > highCap) highDecay = highCap;
        }
        SyncPrevVars();
    }

    void SyncPrevVars()
    {
        prevLC = lowCap;
        prevLD = lowDecay;
        prevHD = highDecay;
        prevHC = highCap;
    }

    public float GetPropability(float testedVar)
    {
        //out of possible range
        if (testedVar < lowCap || testedVar > highCap) return 0;

        //within optimal range
        if (testedVar >= lowDecay && testedVar <= highDecay) return 1;

        //within lower decaying range
        if(testedVar < lowDecay) return Mathf.InverseLerp(lowCap, lowDecay, testedVar);

        //within upper decaying range
        if (testedVar > highDecay) return Mathf.InverseLerp(highCap, highDecay, testedVar);

        Debug.LogError("something went wrong when calculating the Propability. testedVar=" + testedVar + " capRanges: " + lowCap + " " + lowDecay + " " + highDecay + " " + highCap);
        return -1;
    }
}



//NOTE: this classe's code is almost a copy from "SoftRangeVar01". Only the ranges are different. (here Ranges go from minus2 to plus2 "M2P2" instead of 0 to 1)
//Unluckily Unity does not provide a solution to set Ranges for the default inspector by Variables, but these different ranges are necesarry for an efficient interface here
//START of almost duplicated Code
[Serializable]
public class SoftRangeVarM2P2
{
    [Range(-2, 2)]
    public float lowCap = 0;
    [Range(-2, 2)]
    public float lowDecay = 0.2f;
    [Range(-2, 2)]
    public float highDecay = 0.8f;
    [Range(-2, 2)]
    public float highCap = 1;


    float prevLC = 0;
    float prevLD = 0.2f;
    float prevHD = 0.8f;
    float prevHC = 1;


    public void AdaptDependentVars()
    {
        if (lowCap != prevLC)
        {
            if (lowDecay < lowCap) lowDecay = lowCap;
            if (highDecay < lowCap) highDecay = lowCap;
            if (highCap < lowCap) highCap = lowCap;
        }
        else if (lowDecay != prevLD)
        {
            if (lowCap > lowDecay) lowCap = lowDecay;
            if (highDecay < lowDecay) highDecay = lowDecay;
            if (highCap < lowDecay) highCap = lowDecay;
        }
        else if (highDecay != prevHD)
        {
            if (lowCap > highDecay) lowCap = highDecay;
            if (lowDecay > highDecay) lowDecay = highDecay;
            if (highCap < highDecay) highCap = highDecay;
        }
        else if (highCap != prevHC)
        {
            if (lowCap > highCap) lowCap = highCap;
            if (lowDecay > highCap) lowDecay = highCap;
            if (highDecay > highCap) highDecay = highCap;
        }
        SyncPrevVars();
    }

    void SyncPrevVars()
    {
        prevLC = lowCap;
        prevLD = lowDecay;
        prevHD = highDecay;
        prevHC = highCap;
    }

    public float GetPropability(float testedVar)
    {
        //out of possible range
        if (testedVar < lowCap || testedVar > highCap) return 0;

        //within optimal range
        if (testedVar >= lowDecay && testedVar <= highDecay) return 1;

        //within lower decaying range
        if (testedVar < lowDecay) return Mathf.InverseLerp(lowCap, lowDecay, testedVar);

        //within upper decaying range
        if (testedVar > highDecay) return Mathf.InverseLerp(highCap, highDecay, testedVar);

        Debug.LogError("something went wrong when calculating the Propability. testedVar=" + testedVar + " capRanges: " + lowCap + " " + lowDecay + " " + highDecay + " " + highCap);
        return -1;
    }
}
//END of almost duplicated Code


[Serializable]
public class MinMaxRange{
    [Range(0, 10)]
    public int min = 1;
    [Tooltip("a maximum dist of 10 means open end for maximum")]
    [Range(0, 10)]
    public int max = 10;

    int prevMin=1;
    int prevMax=10;

    public void AdaptDependentVars()
    {
        if (min != prevMin)
        {
            if (max < min) max = min;
        }
        else if (max != prevMax)
        {
            if (max < min) min = max;
        }
        prevMin = min;
        prevMax = max;
    }

    public bool IsInRange(int dist, int rad)
    {
        bool inMax = false;
        //bool inMin = false;
        if (max == 10 && dist >= 10) inMax = true;
        else if (dist <= max + rad - 1) inMax = true;
        if (!inMax) return false;
        if (dist  >= min + rad - 1) return true;
        return false;
    }
}