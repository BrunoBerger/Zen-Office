using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenu : MonoBehaviour
{
    public DrawMasks drawMasks;
    public MeshCopySkript meshCopySkript;
    public GameObject introCup;

    public void DeleteMasks()
    {
        Debug.LogWarning("Deleting Masks");
        drawMasks.DeleteMasks();
    }

    public void SpawnMask()
    {
        drawMasks.SpawnMask();
    }

    public void UpdateMesh()
    {
        meshCopySkript.UpdateMesh();
    }

    public void SetTableHeight()
    {
        meshCopySkript.tableHeight = introCup.transform.position.y;
    }
}
