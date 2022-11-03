using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectReplacer : MonoBehaviour
{
    public VirtualReplacementByTag[] replacementListByTag;
    [HideInInspector]
    public List<GameObject> virtualObjReplacements;
    // Start is called before the first frame update
    void Start()
    {
        virtualObjReplacements = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void PlaceObjectOfTagAt(string tag, Vector3 pos)
    {
        bool foundTag = false;
        foreach(VirtualReplacementByTag vrbt in replacementListByTag)
        {
            if (vrbt.tag.Equals(tag))
            {
                foundTag = true;
                int repNumber = Random.Range(0, vrbt.ReplacementPrefabs.Length);
                GameObject virtualReplacement = GameObject.Instantiate(vrbt.ReplacementPrefabs[Random.Range(0, vrbt.ReplacementPrefabs.Length)], pos, Quaternion.identity, transform);
                virtualReplacement.transform.localScale *= Random.Range(vrbt.minRelativeScale, vrbt.maxRelativeScale);
                virtualObjReplacements.Add(virtualReplacement);

                break;
            }
        }
        if (!foundTag) Debug.LogError("custom Vision recognized Object of tag: "+tag+"  but you did not define this tag in the ObjectReplacer");
    }

}


[Serializable]
public class VirtualReplacementByTag
{
    public string tag;
    public GameObject[] ReplacementPrefabs;
    //this scale is relative: if the value is 1 the prefab scale is unchanged, with 0.5 it is half the prefabs size...
    public float minRelativeScale=1; 
    public float maxRelativeScale=1;
}

