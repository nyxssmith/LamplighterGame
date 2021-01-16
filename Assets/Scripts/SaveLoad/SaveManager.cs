using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public string SavesFolder = "Assets/Saves";

    private string SaveFolder;

    public GameObject TownPrefab;



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
        var TownList = FindObjectsOfType<TownController>();
        foreach (TownController controller in TownList)
        {
            Debug.Log(controller.UUID);
            TownSaveData TownToSave = new TownSaveData();

            // copy all data into the save
            TownToSave.UUID = controller.UUID;
            TownToSave.money = controller.money;
            TownToSave.ore = controller.ore;
            TownToSave.stone = controller.stone;
            TownToSave.wood = controller.wood;
            TownToSave.metal = controller.metal;
            TownToSave.food = controller.food;
            TownToSave.supported_population = controller.supported_population;
            TownToSave.current_population = controller.current_population;
            TownToSave.Name = controller.Name;
            TownToSave.range = controller.range;
            // save position
            TownToSave.position_x = controller.gameObject.transform.position.x;
            TownToSave.position_y = controller.gameObject.transform.position.y;
            TownToSave.position_z = controller.gameObject.transform.position.z;

            // TODO put indication of which index in list is town prefab

            string saveJSON = TownToSave.UUID + ".json";
            string FullSavePath = Path.Combine(SaveFolder, "Towns", saveJSON);

            string json = JsonUtility.ToJson(TownToSave);

            File.WriteAllText (FullSavePath, json);
        }
    }

    private void LoadTowns()
    {
        // for all towns in Savefolder/Towns
        // read each json as Savedata
        // instanciate new town
        // copy save into town
        


        string TownsSaveFolderPath =
            Path.Combine(SavesFolder, SaveFolder, "Towns");
        System.Collections.Generic.List<string> TownJsons =
            ListJsonsInDirectory(TownsSaveFolderPath);
        foreach (string TownSavePath in TownJsons)
        {
            Debug.Log("loading town from " + TownSavePath);
            // for each json as fullsave path
            StreamReader reader = new StreamReader(TownSavePath);
            string json = reader.ReadToEnd();
            reader.Close();
            TownSaveData TownSave = JsonUtility.FromJson<TownSaveData>(json);
            // do loading


            GameObject TownObject = Instantiate(TownPrefab);

            TownController Town = TownObject.GetComponent<TownController>();


            Town.UUID = TownSave.UUID;
            Town.money = TownSave.money;
            Town.ore = TownSave.ore;
            Town.stone = TownSave.stone;
            Town.wood = TownSave.wood;
            Town.metal = TownSave.metal;
            Town.food = TownSave.food;
            Town.supported_population = TownSave.supported_population;
            Town.current_population = TownSave.current_population;
            Town.Name = TownSave.Name;
            Town.range = TownSave.range;
            // set position
            Town.gameObject.transform.position = new Vector3(TownSave.position_x,TownSave.position_y,TownSave.position_z);



        }
    }

    private System.Collections.Generic.List<string>
    ListJsonsInDirectory(string directory)
    {
        Debug.Log("listing files");
        DirectoryInfo dir = new DirectoryInfo(directory);
        FileInfo[] info = dir.GetFiles("*.json");
        System.Collections.Generic.List<string> jsonsInDirectory =
            new System.Collections.Generic.List<string>();
        foreach (FileInfo f in info)
        {
            //Debug.Log (f);
            jsonsInDirectory
                .Add(Path.Combine(SaveFolder, "Towns", f.ToString()));
        }
        return jsonsInDirectory;
    }

    private void CreateSaveDirectory()
    {
        SaveFolder =
            Path
                .Combine(SavesFolder,
                (
                "Save" +
                DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_')
                ));
        var folder = Directory.CreateDirectory(SaveFolder); // returns a DirectoryInfo object

        Debug.Log("making save fodlers");
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

        // TODO set savefolder as the Saves/save folder, then load stuff from in there
        SaveFolder = "testsave";

        // TODO remove all existing from scene first
        Debug.Log("loading save TODO");

        LoadTowns();

        /*
        load towns
        load buildings
        load lone items
        load characters with items they have
        do building checks
        do characters find their buildings and claim ownership
        do town recounts
        do camera attachment
        */
    }
}

// classes for all things to save and load including positions
// these are different than the ones used to init all items

    // TODO change each savedata to have path to the prefab that summons it in its save data

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

    public float position_x;

    public float position_y;

    public float position_z;

    public string pathToPrefabToSummon;
}
