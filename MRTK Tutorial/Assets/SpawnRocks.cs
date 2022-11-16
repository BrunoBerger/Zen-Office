using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRocks : MonoBehaviour
{
    public MeshFilter meshFilter;
    public GameObject[] rocks;
    [HideInInspector]
    public Triangle[] triangles;
    private float objScale;
    // Start is called before the first frame update
    void Start()
    {
        FillTriangles();
        SpawnObjAtTriangles();
    }


    public void StartRockSpawning()
    {
        FillTriangles();
        SpawnObjAtTriangles();
    }

    // Update is called once per frame
    void FillTriangles()
    {

        objScale = meshFilter.transform.localScale.x;
        Mesh mesh = meshFilter.mesh;
        triangles = new Triangle[mesh.triangles.Length / 3];
        int index = 0;
        //Debug.Log("tr: " + index);
        //Debug.Log("i vorher: " + index);
        while (index < mesh.triangles.Length)
        {
            // Debug.Log("i vorher: " + index);
            triangles[(int)(index / 3)] = new Triangle(mesh.vertices[mesh.triangles[index++]], mesh.vertices[mesh.triangles[index++]], mesh.vertices[mesh.triangles[index++]]);
            //Debug.Log("i nachher: " + index);
        }
    }

    void SpawnObjAtTriangles()
    {
        foreach (Triangle tri in triangles)
        {
            if (Mathf.Abs(tri.normal.y) < 0.4f)
            {
                Debug.Log("normal: " + tri.normal);

                //following 6 lines from elenzil at https://answers.unity.com/questions/1618126/given-a-vector-how-do-i-generate-a-random-perpendi.html (15.11.2022)
                float du = Vector3.Dot(tri.normal, Vector3.up);
                float df = Vector3.Dot(tri.normal, Vector3.forward);
                Vector3 v1 = Mathf.Abs(du) < Mathf.Abs(df) ? Vector3.up : Vector3.forward;
                Vector3 v2 = Vector3.Cross(v1, tri.normal);
                float degrees = Random.Range(0.0f, 360.0f);
                v2 = Quaternion.AngleAxis(degrees, tri.normal) * v2;



                GameObject rocky = Instantiate(rocks[Random.Range(0, rocks.Length - 1)], tri.center * objScale, Quaternion.LookRotation(v2, tri.normal)); //DELETE *10 LATER!!!!!!!!!!!!!!!!!!!!!!!
                rocky.transform.localScale *= tri.scaler * objScale * 4f;//MAY CHANGE *4f LATER!!!!!!!!!!!
            }
        }

    }


    public struct Triangle
    {
        public Vector3[] points;
        public Vector3 center;
        public Vector3 normal;
        public float scaler;

        public Triangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            this.points = new Vector3[] { p0, p1, p2 };
            this.center = (p0 + p1 + p2) / 3f;
            this.normal = (Vector3.Cross(p1 - p0, p2 - p0)).normalized;
            this.scaler = ((p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p0).magnitude) / 3;
        }
    }
}
