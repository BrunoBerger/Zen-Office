using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct Save
{
    GameObject[] meshCopyCollection;
    List<GameObject> placedObjects;



}

public class SaveSystem : MonoBehaviour
{
    public void SaveToDisk()
    {
        Save save = new Save();
        using ( var stream = File.Open("mySaveFile", FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, false))
            {
                writer.Write("test");
                writer.Write(save);
            }
        }
        
    }
    public void LoadFromDisk()
    {
        //Save save = (Save)formatter.Deserialize(stream);
    }
    public void RenewSaveData(GameObject[] meshCopyCollection, List<GameObject> placedObjects)
    {
        //this.meshCopyCollection = meshCopyCollection;
        //this.placedObjects = placedObjects;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
