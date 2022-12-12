using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public struct Save
{
    //GameObject[] meshCopyCollection;
    //List<GameObject> placedObjects;

    public string testString;

}

public class SaveSystem : MonoBehaviour
{
    static readonly string folderpath = "SaveFiles";
    readonly string filepath = folderpath + "/mySaveFile.save";

    public void SaveToDisk()
    {
        Directory.CreateDirectory("SaveFiles");
        Save save = new Save();
        save.testString = "Hello disk!";

        using var stream = File.Open(filepath, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(stream, save);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Could not save Save" + e);
            throw;
        }

    }
    public void LoadFromDisk()
    {
        Save loadedSave;

        using var stream = File.Open(filepath, FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            loadedSave = (Save)formatter.Deserialize(stream);
        }
        catch (SerializationException e)
        {
            Debug.LogError("Could not load save" + e);
            throw;
        }

        Debug.Log("LOADED MSG:" + loadedSave.testString);
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveToDisk();
        LoadFromDisk();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
