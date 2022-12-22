using System.Collections;
using System.Collections.Generic;
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
                Random.Range(-sp.treeGridSize, sp.treeGridSize),
                sp.RoughFloatingLevel,
                Random.Range(-sp.treeGridSize, sp.treeGridSize)
                );
            GameObject newLamp = Instantiate(
                original: sp.LampPrefab,
                position: newPos,
                rotation: sp.LampPrefab.transform.rotation,
                parent: transform
                );
            placedLamps.Add(newLamp);


        }
        Util.TestFunc();
        currentlySpawningLamps = false;
        yield return null;
    }
}
