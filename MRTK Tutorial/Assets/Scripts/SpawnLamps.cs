using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpawnLamps : MonoBehaviour
{
    [SerializeField] SpawnProfile sp;
    [SerializeField] MeshCopySkript meshCopySkript;
    List<GameObject> placedLamps;
    public bool currentlySpawningLamps;

    // Start is called before the first frame update
    void Start()
    {
        placedLamps = new List<GameObject>();
        //StartCoroutine(GenerateLamps());
        //Debug.LogWarning("DELETE LINE ABOVE ME!");
    }

    public IEnumerator GenerateLamps()
    {
        currentlySpawningLamps = true;

        foreach (var obj in placedLamps)
            Destroy(obj);
        placedLamps.RemoveAll(o => o == null);

        Vector3 startPos = new Vector3(
            Camera.main.transform.position.x,
            meshCopySkript.tableHeight,
            Camera.main.transform.position.z);

        for (int i = 0; i < sp.NumberOfLamps; i++)
        {
            var newPos = new Vector3(
                startPos.x + Random.Range(-sp.LampAreaSize, sp.LampAreaSize),
                startPos.y + sp.RoughFloatingLevel + Random.Range(-sp.LampVerticalSpread, sp.LampVerticalSpread),
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
