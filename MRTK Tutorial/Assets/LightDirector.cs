using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.rotation = Quaternion.LookRotation(-transform.parent.position, Vector3.up);
    }
}
