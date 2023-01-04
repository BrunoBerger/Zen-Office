using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void ClearObjects(List<GameObject> listToClear)
    {
        foreach (GameObject obj in listToClear)
        {
            Object.Destroy(obj);
        }
        listToClear.RemoveAll(o => o == null);
    }
}
