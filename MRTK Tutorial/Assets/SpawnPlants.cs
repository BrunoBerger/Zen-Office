using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlants : MonoBehaviour
{
    public GameObject[] bushes;
    public GameObject[] grases;
    public GameObject[] trees;

    List<GameObject> placedObjects;

    public bool currentlySpawningTrees;
    [SerializeField] float treeLine;
    [SerializeField] LayerMask colLayer;

    // Start is called before the first frame update
    void Start()
    {
        placedObjects = new List<GameObject>();
        currentlySpawningTrees = false;
        treeLine = -0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        
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


        //spawn trees in grid around player:
        Vector3 camPos = Camera.main.transform.position;
        for (float x = -2; x <= 2; x += 0.1f)
        {
            for (float z = -2; z <= 2; z += 0.1f)
            {
                float noiseSample = Mathf.PerlinNoise(x * 5, z * 5);
                if (noiseSample < 0.5f)
                {
                    continue;
                }
                if (Physics.Raycast(new Vector3(camPos.x + x, 0f, camPos.z + z), transform.TransformDirection(Vector3.down), out RaycastHit hit, 3, colLayer))
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    // Only if flat at not at the ground
                    Vector3 hp = hit.point;

                    if (hit.normal.y > 0.9f && hp.y > floorHeight + treeLine)
                    {
                        GameObject newObj = Instantiate(
                            original: trees[Random.Range(0, trees.Length - 1)],
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
