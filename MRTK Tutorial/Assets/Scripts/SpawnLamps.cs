using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpawnLamps : MonoBehaviour
{
    [SerializeField] SpawnProfile sp;
    List<GameObject> placedLamps;
    public bool currentlySpawningLamps;

    // Start is called before the first frame update
    void Start()
    {
        placedLamps = new List<GameObject>();
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    foreach (GameObject lamp in placedLamps)
    //    {

    //    }
    //}

    public IEnumerator GenerateLamps()
    {
        currentlySpawningLamps = true;

        foreach (var obj in placedLamps)
            Destroy(obj);
        placedLamps.RemoveAll(o => o == null);

        Vector3 startPos = Camera.main.transform.position;

        for (int i = 0; i < sp.NumberOfLamps; i++)
        {
            var newPos = new Vector3(
                startPos.x + Random.Range(-sp.LampAreaSize, sp.LampAreaSize),
                sp.RoughFloatingLevel + Random.Range(-0.2f, 0.2f),
                 startPos.z + Random.Range(-sp.LampAreaSize, sp.LampAreaSize)
                );
            GameObject newLamp = Instantiate(
                original: sp.LampPrefab,
                position: newPos,
                rotation: sp.LampPrefab.transform.rotation,
                parent: transform
                );
            placedLamps.Add(newLamp);


        }
        currentlySpawningLamps = false;
        yield return null;
    }
}
