using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTextureControl : MonoBehaviour
{
    public Material refMat;

    private Material _editedMat;
    private Vector2 _posXZ;
    private Vector2 _offsetXZ;
    private MeshRenderer _meshRenderer;
    void Start()
    {
        _editedMat = new Material(refMat);
        _editedMat.CopyPropertiesFromMaterial(refMat);
        _posXZ = new Vector2(transform.position.x, transform.position.z);
        float textureScale = 0.1f;
        float tilesPerMeter = 40;
        _offsetXZ = -_posXZ * textureScale * tilesPerMeter;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = _editedMat;
    }

    // Update is called once per frame
    void Update()
    {
        _editedMat.mainTextureOffset = refMat.mainTextureOffset + _offsetXZ;
        _editedMat.SetTextureOffset("_DiffTex", refMat.GetTextureOffset("_DiffTex")+_offsetXZ); //DELETE THIS VEC LATER
    }
}
