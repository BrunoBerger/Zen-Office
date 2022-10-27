using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCopySkript : MonoBehaviour
{
    public GameObject meshHolderPrefab;
    public GameObject testCupPrefab;
    public LayerMask colLayer;
    public Material mat;
    public GameObject mixedRealityPlayspace;
    [HideInInspector]
    public GameObject[] meshCopyCollection;
    [HideInInspector]
    public MeshRenderer[] mcRenderer;
    [HideInInspector]
    public bool gotMesh;

    bool startedToCopyMesh = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time>5&&!startedToCopyMesh)
        {
            startedToCopyMesh = true;
            mixedRealityPlayspace = GameObject.Find("MixedRealityPlayspace");
            Debug.Log("copy log 1");
            
            Transform SAS = mixedRealityPlayspace.transform.Find("Spatial Awareness System");
            Debug.Log("copy log 11");
            Transform OpenSMO = SAS.Find("OpenXR Spatial Mesh Observer");
            Debug.Log("copy log 111");
            meshCopyCollection = new GameObject[OpenSMO.childCount];
            for(int i=0; i< OpenSMO.childCount - 1; i++)
            {
                //Debug.Log("copy log 3, i = " + i);
                //meshCopy[i] = Instantiate(OpenSMO.GetChild(i).gameObject);


                //meshCopyCollection[i] = new GameObject();
                //meshCopyCollection[i].AddComponent<MeshFilter>();
                //meshCopyCollection[i].AddComponent<MeshRenderer>();
                //meshCopyCollection[i].GetComponent<MeshFilter>().mesh = new Mesh();
                //meshCopyCollection[i].GetComponent<MeshRenderer>().material = mat;

                meshCopyCollection[i] = Instantiate(meshHolderPrefab, transform);


                meshCopyCollection[i].GetComponent<MeshFilter>().mesh.SetVertices(OpenSMO.GetChild(i).GetComponent<MeshFilter>().mesh.vertices);
                meshCopyCollection[i].GetComponent<MeshFilter>().mesh.SetNormals(OpenSMO.GetChild(i).GetComponent<MeshFilter>().mesh.normals);
                meshCopyCollection[i].GetComponent<MeshFilter>().mesh.SetTriangles(OpenSMO.GetChild(i).GetComponent<MeshFilter>().mesh.triangles,0);

                meshCopyCollection[i].GetComponent<MeshCollider>().sharedMesh = meshCopyCollection[i].GetComponent<MeshFilter>().mesh;

                //meshCopyCollection[i].GetComponent<MeshRenderer>().material = mat;

            }
            
            Debug.Log("copy log 4");
            gotMesh = true;

            //testing collision:
            for(float x = -2; x<=2; x+=0.125f)
            {
                for (float z = -2; z <= 2; z += 0.125f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(x, 0f, z), transform.TransformDirection(Vector3.down), out hit, 3, colLayer))
                    {
                        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                        //Debug.Log("Did Hit ");
                        Instantiate(testCupPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal));
                    }
                }
            }
        }
    }
}
