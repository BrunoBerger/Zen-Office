using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlants : MonoBehaviour
{
    [SerializeField] SpawnProfile sp;
    List<GameObject> placedObjects;

    public bool currentlySpawningTrees;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();
        currentlySpawningTrees = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteTrees()
    {
        //clear previous trees
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.RemoveAll(o => o == null);
    }

    public IEnumerator UpdateTrees(float floorHeight)
    {
        currentlySpawningTrees = true;
        //clear previous trees
        foreach (GameObject obj in placedObjects)
        {
            Destroy(obj);
        }
        placedObjects.RemoveAll(o => o == null);
        //Util.DeleteGameObjFromList(placedObjects);

        //spawn trees in grid around player:
        Vector3 camPos = Camera.main.transform.position;
        for (float x = -sp.treeGridSize; x <= sp.treeGridSize; x += sp.treeGridResolution)
        {
            for (float z = -sp.treeGridSize; z <= sp.treeGridSize; z += sp.treeGridResolution)
            {
                float noiseSample = Mathf.PerlinNoise(x * sp.perlinNoiseOffset, z * sp.perlinNoiseOffset);
                if (noiseSample < sp.spawnThreshold)
                    continue;

                if (Physics.Raycast(new Vector3(camPos.x + x, 0f, camPos.z + z), transform.TransformDirection(Vector3.down), out RaycastHit hit, 3, sp.colLayer))
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    // Only if flat at not at the ground
                    Vector3 hp = hit.point;

                    if (hit.normal.y > 0.9f && hp.y > floorHeight + sp.treeLine)
                    {
                        GameObject newObj = Instantiate(
                            original: sp.trees[Random.Range(0, sp.trees.Length - 1)],
                            position: hit.point,
                            rotation: Quaternion.LookRotation(Vector3.forward, hit.normal),
                            parent: transform
                        );
                        placedObjects.Add(newObj);
                        //yield return null;  // for "animated spawning"
                    }
                }
                
            }
        }
        currentlySpawningTrees = false;
        yield return null;
    }
}
