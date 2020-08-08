using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

public class ItemDataManager
{

    public string ItemSaveFile;
    public string ItemSaveFileFolder;
    private string FullSavePath;

    public ItemDataManager()
    {
        this.ItemSaveFile = "PLACEHOLDER";
        this.ItemSaveFileFolder = "PLACEHOLDER";
    }


    public ItemDataManager(string ItemSaveFileFolder, string ItemSaveFile)
    {
        this.ItemSaveFile = ItemSaveFile;
        this.ItemSaveFileFolder = ItemSaveFileFolder;
    }

    //init this class and create the Items full save path
    public void Init(string ItemSaveFileFolder, string ItemSaveFile)
    {
        //Debug.Log("Init");
        this.ItemSaveFile = ItemSaveFile;
        this.ItemSaveFileFolder = ItemSaveFileFolder;
        //Debug.Log(this.ItemSaveFile);
        FullSavePath = Path.Combine(this.ItemSaveFileFolder, this.ItemSaveFile);

    }

    // Outputs a ItemStats object (input) to a json file
    public void Save(ItemData Item)
    {
        //TODO based on OS for this part
        //string FullSavePath = this.ItemSaveFileFolder + "\\" + this.ItemSaveFile;
        Debug.Log("Saving to " + FullSavePath);

        string json = JsonUtility.ToJson(Item);

        File.WriteAllText(FullSavePath, json);

    }

    // Takes a filepath and returns a ItemStats object
    public ItemData Load()//TODO make return a ItemStats object
    {
        Debug.Log("Loading save ");

        Debug.Log("loading Item from " + FullSavePath);
        StreamReader reader = new StreamReader(FullSavePath); 
        string json = reader.ReadToEnd();
        reader.Close();

        ItemData Item = JsonUtility.FromJson<ItemData>(json);
        return Item;



    }

}
