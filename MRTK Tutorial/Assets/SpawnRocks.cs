using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRocks : MonoBehaviour
{
    [SerializeField] SpawnProfile sp;
    //public MeshFilter meshFilter;
    [HideInInspector]
    public Triangle[][] trianglesListholder;
    //private float objScale;
    // Start is called before the first frame update
    void Start()
    {
        //FillTriangles();
        //SpawnObjAtTriangles();
    }


    public void StartRockSpawning(Mesh[] meshes)
    {
        FillTriangles(meshes);
        SpawnObjAtTriangles();
    }

    public void DeleteRocks()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void FillTriangles(Mesh[] meshes)
    {
        trianglesListholder = new Triangle[meshes.Length][];
        for (int i = 0; i < meshes.Length; i++)
        {
            //objScale = meshes[i].transform.localScale.x;
            //Mesh mesh = meshFilter.mesh;
            trianglesListholder[i] = new Triangle[meshes[i].triangles.Length / 3];
            int index = 0;
            //Debug.Log("tr: " + index);
            //Debug.Log("i vorher: " + index);
            while (index < meshes[i].triangles.Length)
            {
                // Debug.Log("i vorher: " + index);
                trianglesListholder[i][(int)(index / 3)] = new Triangle(meshes[i].vertices[meshes[i].triangles[index++]], meshes[i].vertices[meshes[i].triangles[index++]], meshes[i].vertices[meshes[i].triangles[index++]]);
                //Debug.Log("i nachher: " + index);
            }

        }
    }

    void SpawnObjAtTriangles()
    {
        foreach (Triangle[] triangles in trianglesListholder)
        {
            foreach (Triangle tri in triangles)
            {
                if (tri.normal.y < 0.25f && tri.normal.y >-0.5f)
                {
                    //Debug.Log("normal: " + tri.normal);

                    //following 6 lines from elenzil at https://answers.unity.com/questions/1618126/given-a-vector-how-do-i-generate-a-random-perpendi.html (15.11.2022)
                    float du = Vector3.Dot(tri.normal, Vector3.up);
                    float df = Vector3.Dot(tri.normal, Vector3.forward);
                    Vector3 v1 = Mathf.Abs(du) < Mathf.Abs(df) ? Vector3.up : Vector3.forward;
                    Vector3 v2 = Vector3.Cross(v1, tri.normal);
                    float degrees = Random.Range(0.0f, 360.0f);
                    v2 = Quaternion.AngleAxis(degrees, tri.normal) * v2;



                    GameObject rocky = Instantiate(sp.rocks[Random.Range(0, sp.rocks.Length - 1)], tri.center+(Vector3.down*0.05f+ Vector3.down * 0.01f*tri.scaler), Quaternion.LookRotation(v2, tri.normal), transform); //DELETE *10 LATER!!!!!!!!!!!!!!!!!!!!!!!
                    rocky.transform.localScale *= tri.scaler *3f;//MAY CHANGE *4f LATER!!!!!!!!!!!
                }
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
