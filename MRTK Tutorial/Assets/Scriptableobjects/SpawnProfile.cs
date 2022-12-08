using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Plant Spawn Preset", menuName = "ScriptableObjects/Plant Spawn Profile")]
public class SpawnProfile : ScriptableObject
{
    public float speed = 1;
    public GameObject[] bushes;
    public GameObject[] grases;
    public GameObject[] trees;
}