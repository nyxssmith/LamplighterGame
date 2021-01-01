using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public string SavesFolder = "Assets/Saves";

    private string SaveFolder;

    // when made create a folder
    void Start()
    {
    }

    void Update()
    {
    }

    public void Save()
    {
        // create folder with timestamp
        CreateSaveDirectory();

        // for each character save a json of the using data manager each as its
        //  uuid.json
        //  save current task and position data
        SaveTowns();
        SaveBuildings();
        SaveCharacters();
        SaveItems();

        // for each item save uuid with data manager
    }

    private void SaveItems()
    {
    }

    private void SaveCharacters()
    {
    }

    private void SaveBuildings()
    {
    }

    private void SaveTowns()
    {
        // TODO copy this into all other save methods
        // for each town in scene
        //  create json of its uuid.json as name
        //  save all resources to the json
        //  save range to json
        //  save xyz position to json
        // TODO reverse this for loading
    }

    private void CreateSaveDirectory()
    {
        SaveFolder =
            Path
                .Combine(SavesFolder,
                ("Save" + DateTime.Now.ToString().Replace('/', '-')));
        var folder = Directory.CreateDirectory(SaveFolder); // returns a DirectoryInfo object

        folder = Directory.CreateDirectory(Path.Combine(SaveFolder, "Items"));
        folder =
            Directory.CreateDirectory(Path.Combine(SaveFolder, "Characters"));
        folder =
            Directory.CreateDirectory(Path.Combine(SaveFolder, "Buildings"));
        folder = Directory.CreateDirectory(Path.Combine(SaveFolder, "Towns"));

        folder = Directory.CreateDirectory(SaveFolder); // returns a DirectoryInfo object
    }

    public void Load()
    {
        // TODO make it take a save to load
        string SaveToLoad = "testsave";

        Debug.Log("loading save TODO");
    }
}

// classes for all things to save and load including positions

// these are different than the ones used to init all items



[System.Serializable]
public class TownSaveData
{
    public string UUID;

    public float wood;

    public float stone;

    public float metal;

    public float ore;

    public float money;

    public float food;

    public float range;

    public string Name;

    public float current_population;

    public float supported_population;
}
