using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Object Spawn Preset", menuName = "ScriptableObjects/Object Spawn Profile")]
public class SpawnProfile : ScriptableObject
{
    [Header("Tree")]
    public float treeGridResolution = 0.1f;
    public float treeLine = -0.3f;
    public float treeGridSize = 2f;
    public float perlinNoiseOffset = 5f;
    public float spawnThreshold = 0.5f;
    public LayerMask colLayer;
    public GameObject[] trees;

    [Header("Fooliage")]
    public GameObject[] bushes;
    public GameObject[] grasses;

    [Header("Rocks")]
    public GameObject[] rocks;

    [Header("Floating Lamps")]
    public GameObject LampPrefab;
    [Range(0, 1)] public int NumberOfLamps = 10;
    [Range(1, 4)] public float RoughFloatingLevel = 2.5f;
}