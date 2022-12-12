using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTextureControl : MonoBehaviour
{
    //public Material refMat;

    //private Material _editedMat;
    //private Vector2 _posXZ;
    //private Vector2 _offsetXZ;
    //private MeshRenderer _meshRenderer;
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        float tilesPerMeter = 40;
        Vector2 posXZ = new Vector2(transform.position.x, transform.position.z);
        Vector2 offsetXZ = -posXZ *  tilesPerMeter;

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0,0) + offsetXZ,
            new Vector2(1,0) + offsetXZ,
            new Vector2(0,1) + offsetXZ,
            new Vector2(1,1) + offsetXZ
        };
        mesh.uv = uvs;


        //_editedMat = new Material(refMat);
        //_editedMat.CopyPropertiesFromMaterial(refMat);
        //_posXZ = new Vector2(transform.position.x, transform.position.z);
        //float textureScale = 0.1f;
        //float tilesPerMeter = 40;
        //_offsetXZ = -_posXZ * textureScale * tilesPerMeter;
        //_meshRenderer = GetComponent<MeshRenderer>();
        //_meshRenderer.material = _editedMat;
    }

    // Update is called once per frame
    //void Update()
    //{
        //test
        //Vector2 mtv2 = refMat.mainTextureOffset + _offsetXZ;
        //Vector2 dtv2 = refMat.GetTextureOffset("_DiffTex");


        //_editedMat.mainTextureOffset = refMat.mainTextureOffset + _offsetXZ;
        //_editedMat.SetTextureOffset("_DiffTex", refMat.GetTextureOffset("_DiffTex") + _offsetXZ);
    //}
}
