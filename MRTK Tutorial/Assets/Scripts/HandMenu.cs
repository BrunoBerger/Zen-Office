using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenu : MonoBehaviour
{
    public DrawMasks drawMasks; 
    public void DeleteMasks()
    {
        Debug.LogWarning("Deleting Masks");
        drawMasks.DeleteMasks();
    }

    public void SpawnMask()
    {
        drawMasks.SpawnMask();
    }
}
