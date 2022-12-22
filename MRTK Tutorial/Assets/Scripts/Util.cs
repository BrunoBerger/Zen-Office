using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void TestFunc()
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
    }
    public static void DeleteGameObjFromList(List<GameObject> listToClear)
    {
        foreach (GameObject obj in listToClear)
        {
            Object.Destroy(obj);
        }
        listToClear.RemoveAll(o => o == null);
    }
}
