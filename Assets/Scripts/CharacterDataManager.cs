using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CharacterDataManager
{

    public string CharacterSaveFile;
    public string CharacterSaveFileFolder;
    private string FullSavePath;

    public CharacterDataManager()
    {
        this.CharacterSaveFile = "PLACEHOLDER";
        this.CharacterSaveFileFolder = "PLACEHOLDER";
    }


    public CharacterDataManager(string CharacterSaveFileFolder, string CharacterSaveFile)
    {
        this.CharacterSaveFile = CharacterSaveFile;
        this.CharacterSaveFileFolder = CharacterSaveFileFolder;
    }

    //init this class and create the characters full save path
    public void Init(string CharacterSaveFileFolder, string CharacterSaveFile)
    {
        //Debug.Log("Init");
        this.CharacterSaveFile = CharacterSaveFile;
        this.CharacterSaveFileFolder = CharacterSaveFileFolder;
        //Debug.Log(this.CharacterSaveFile);
        FullSavePath = this.CharacterSaveFileFolder + "\\" + this.CharacterSaveFile;


    }

    // Outputs a CharacterStats object (input) to a json file
    public void Save(CharacterData character)
    {
        //TODO based on OS for this part
        string FullSavePath = this.CharacterSaveFileFolder + "\\" + this.CharacterSaveFile;
        Debug.Log("Saving to " + FullSavePath);

        string json = JsonUtility.ToJson(character);

        File.WriteAllText(FullSavePath, json);

    }

    // Takes a filepath and returns a CharacterStats object
    public CharacterData Load()//TODO make return a CharacterStats object
    {
        Debug.Log("Loading save ");

        Debug.Log("loading character from " + FullSavePath);
        StreamReader reader = new StreamReader(FullSavePath); 
        string json = reader.ReadToEnd();
        reader.Close();

        CharacterData Character = JsonUtility.FromJson<CharacterData>(json);
        return Character;



    }

}