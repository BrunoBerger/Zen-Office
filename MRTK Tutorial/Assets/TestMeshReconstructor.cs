using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshReconstructor : MonoBehaviour
{
    public GameObject testThing;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh =  GetComponent<MeshFilter>().mesh;

        Debug.Log("time before remesh " + Time.realtimeSinceStartup);
        mesh = GetComponent<MeshReconstructor>().ReconstructMeshUntillDone(mesh);
        Debug.Log("time after remesh " + Time.realtimeSinceStartup);

        mesh.RecalculateNormals();

        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];


        for (int v = 0; v < uvs.Length; v++)
        {
            uvs[v] = new Vector2(vertices[v].x, vertices[v].z);
        }
        mesh.SetUVs(0, uvs);

        //int[] tris = mesh.triangles;
        //Vector3[] verts = mesh.vertices;
        //int i = 0;
        //while (i < tris.Length)
        //{
        //    Vector3 avPos = (verts[tris[i++]]+ verts[tris[i++]]+ verts[tris[i++]])/3;

        //    Instantiate(testThing, avPos, Quaternion.identity);
        //}
    }


    //Was fehtl??? Es müssen auch dreiecke hinzugefügt werden, die nicht an eine linie angrenzen. JEDE LINIE BRAUCHT 2 DREIECKE!!!!
}
