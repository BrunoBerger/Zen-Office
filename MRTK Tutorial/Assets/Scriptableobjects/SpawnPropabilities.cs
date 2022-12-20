using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public int radius=1;
    [Range(0, 5)]
    public int mdOtherObj = 0;
    [Range(0, 10)]
    public int mdEdge = 0;
    [Range(0, 10)]
    public int mdRift = 0;
    [Range(0, 10)]
    public int mdHill = 0;

    [Header("tilt criteria")]
    [Range(0, 2)]
    [Tooltip("0 = does not spanw on rocky surfaces, 1 = does only spawn on rocky surfaces, 2 = spawns on both kind of surfaces")]
    public int spawnsOnRocks = 0;
    public bool tiltWithFloor = false;

    [Header("Spawnable Objects")]
    public GameObject[] objects;




    private void OnValidate()
    {
        heightRange.AdaptDependentVars();
        noisemapRange.AdaptDependentVars();
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
//Unluckily Unity does not provide a legit solution to set Ranges by Variables, but these different ranges are necesarry for an efficient interface here
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