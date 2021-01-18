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

    public GameObject BuildToolPrefab;

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
        //CreateSaveDirectory();
        SaveFolder = Path.Combine(SavesFolder, "testsave");

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
        // for each character,
        // make a folder in saves/characters/characters
        // save the character data via characterdata already made save(method)
        
        // create new CharacterSaveData to save all other parts like buildings and towns etc

        
    }

    private void LoadCharacters()
    {
        // for each character,
        // for each in its folder, 
        // create character with cdm data
        // load and set other settings from characterSaveData
        // call awake on the character again to set new values
        // 
    }

    private void SaveBuildings()
    {
        // save all non ghost buildings
        var BuildingsList = FindObjectsOfType<BuildingController>();
        foreach (BuildingController controller in BuildingsList)
        {
            // if building has a uuid (isnt a ghost image)
            if (controller.GetUUID() != "")
            {
                // save it
                BuildingSaveData BuildingToSave = new BuildingSaveData();

                // save data
                BuildingToSave.ownerUUID = controller.GetOwner();
                BuildingToSave.UUID = controller.GetUUID();
                BuildingToSave.buildToolIndex = controller.GetBuildToolIndex();
                BuildingToSave.HasDoneWork = controller.GetHasDoneWork();
                BuildingToSave.Type = controller.GetType();
                TownController buildingsTown = controller.GetTown();
                if (buildingsTown != null)
                {
                    BuildingToSave.TownUUID = buildingsTown.UUID;
                }

                // save position
                BuildingToSave.position_x =
                    controller.gameObject.transform.position.x;
                BuildingToSave.position_y =
                    controller.gameObject.transform.position.y;
                BuildingToSave.position_z =
                    controller.gameObject.transform.position.z;

                // save rotation
                BuildingToSave.rotation_x =
                    controller.gameObject.transform.rotation.eulerAngles.x;
                BuildingToSave.rotation_y =
                    controller.gameObject.transform.rotation.eulerAngles.y;
                BuildingToSave.rotation_z =
                    controller.gameObject.transform.rotation.eulerAngles.z;

                // write save data json
                string saveJSON = BuildingToSave.UUID + ".json";
                string FullSavePath =
                    Path.Combine(SaveFolder, "Buildings", saveJSON);

                string json = JsonUtility.ToJson(BuildingToSave);

                File.WriteAllText (FullSavePath, json);
            }
        }
    }

    private void LoadBuildings()
    {
        // get list of all town controllers
        var TownList = FindObjectsOfType<TownController>();

        // make a buildtool to get buildings from
        GameObject buildToolObject = Instantiate(BuildToolPrefab);

        // make sure update() isnt called on the tool
        buildToolObject.SendMessage("SetUsedForLoading", true);

        // populate its list to pull from
        buildToolObject.SendMessage("FillListOfBuildObjects");

        // get the buildtool controller
        BuildTool buildTool = buildToolObject.GetComponent<BuildTool>();

        // for all files in the save director
        string BuildingsSaveFolderPath =
            Path.Combine(SavesFolder, SaveFolder, "Buildings");

        System.Collections.Generic.List<string> BuildingJsons =
            ListJsonsInDirectory(BuildingsSaveFolderPath);

        foreach (string BuildingSavePath in BuildingJsons)
        {
            // for each json as fullsave path
            StreamReader reader = new StreamReader(BuildingSavePath);
            string json = reader.ReadToEnd();
            reader.Close();
            BuildingSaveData BuildingSave =
                JsonUtility.FromJson<BuildingSaveData>(json);

            // get which prefab is the building
            int buildToolIndexToSummon = BuildingSave.buildToolIndex;

            // only summon if it was made by player or has an index
            if (buildToolIndexToSummon > 0)
            {
                // get the prefab for the building
                GameObject prefabToSummon =
                    buildTool
                        .GetGameObjectAtBuildListIndex(buildToolIndexToSummon);

                GameObject BuildingObject = Instantiate(prefabToSummon);

                BuildingController buildingsController =
                    BuildingObject.GetComponent<BuildingController>();

                // add data to building
                // make sure its started
                buildingsController.Start();

                // set vars
                buildingsController.SetOwner(BuildingSave.ownerUUID);
                buildingsController.SetUUID(BuildingSave.UUID);
                buildingsController
                    .SetBuildToolIndex(BuildingSave.buildToolIndex);
                buildingsController.SetHasDoneWork(BuildingSave.HasDoneWork);
                buildingsController.SetType(BuildingSave.Type);

                if (BuildingSave.TownUUID != "")
                {
                    //find the town from the list and set it
                    foreach (TownController controller in TownList)
                    {
                        if (controller.UUID == BuildingSave.TownUUID)
                        {
                            buildingsController.SetTown (controller);
                            break;
                        }
                    }
                }

                // move building
                BuildingObject.transform.position =
                    new Vector3(BuildingSave.position_x,
                        BuildingSave.position_y,
                        BuildingSave.position_z);

                // rotate building
                var rot = BuildingObject.transform.rotation;
                BuildingObject.transform.rotation =
                    rot *
                    Quaternion
                        .Euler(BuildingSave.rotation_x,
                        BuildingSave.rotation_y,
                        BuildingSave.rotation_z);
            }
        }

        // remove buildtool obj
        Destroy (buildToolObject);
    }

    private void SaveTowns()
    {
        // for each town in scene
        //  create json of its uuid.json as name
        //  save all resources to the json
        //  save range to json
        //  save xyz position to json
        var TownList = FindObjectsOfType<TownController>();
        foreach (TownController controller in TownList)
        {
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
            //Debug.Log("loading town from " + TownSavePath);
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
            Town.gameObject.transform.position =
                new Vector3(TownSave.position_x,
                    TownSave.position_y,
                    TownSave.position_z);
        }
    }

    private System.Collections.Generic.List<string>
    ListJsonsInDirectory(string directory)
    {
        //Debug.Log("listing files");
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
        LoadBuildings();

        /*
        load towns
        load buildings
        load characters
            do characters find all their buildings when they are loaded
        load all items
            give all items to characters as they are loading
        do town recounts
        
        do camera attachment
        do daylight cycle save too
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
}

[System.Serializable]
public class BuildingSaveData
{
    public string UUID;

    public string ownerUUID;

    public string TownUUID;

    public string Type;

    public bool HasDoneWork;

    public int buildToolIndex;

    public float position_x;

    public float position_y;

    public float position_z;

    public float rotation_x;

    public float rotation_y;

    public float rotation_z;
}
