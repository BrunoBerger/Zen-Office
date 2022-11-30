using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenu : MonoBehaviour
{
    public DrawMasks drawMasks; 
    public void DeleteButton()
    {
        Debug.LogWarning("Deleting Masks");
        drawMasks.DeleteMasks();
    }
}
