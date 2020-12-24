using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    // manages dialog between 2 characters
    private CharacterController CharacterA;

    private CharacterController CharacterB;

    public string AUUID;

    public string BUUID;

    public string TalkingUUID;

    private string DialogLevelSaveLocation = "Assets/DialogJson";


    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerExit(Collider ExitingCharacter)
    {
        CharacterController ExitingCharacterController =
            ExitingCharacter.GetComponent<CharacterController>();
        if (ExitingCharacterController != null)
        {
            string leavingUUID = ExitingCharacterController.GetUUID();

            // if exiting controller was either character in dialog, leave itfor both then Enddialog
            if (leavingUUID == AUUID || leavingUUID == BUUID)
            {
                EndDialog();
            }
        }
    }

    public void NotifyOfLeaving()
    {
        // if character is leaving then notify of end
        EndDialog();
    }

    public void EndDialog()
    {
        CharacterA.LeaveDialog();
        CharacterB.LeaveDialog();

        Destroy(this.gameObject);
    }

    public void StartDialog(CharacterController A, CharacterController B)
    {
        CharacterA = A;
        CharacterB = B;
        AUUID = CharacterA.GetUUID();
        BUUID = CharacterB.GetUUID();
        CharacterA.JoinDialog(this);
        CharacterB.JoinDialog(this);

        TalkingUUID = AUUID; // A started the dialog

        // Load dialog/tree for character B as start, since they are who is being talked too
        string startOfDialogTree = DetermineDialogToLoadFromCharacterDefaultTask(CharacterB);

        LoadDialogTree(startOfDialogTree);
        // then go to loop when b navigates that tree

        
        

    }

    /*
    results of dialog data

    end // ends dialog
    sell_%itemname% //sells and item
    buy_%itemname% //buys and item
    fight // starts a fight 
    friend // increases friend value
    join_squad // joins squad
    none // intermediate step / no effect

    */

    private string DetermineDialogToLoadFromCharacterDefaultTask(CharacterController character){
        // TODO this
        return "farmer1.json";
    }


    private void LoadDialogTree(string DialogLevelFileName)
    {

        // TODO populate recusrivly using DialogTree classes to make a tree data structure
        
        // TODO load dialog level 0
        // if it has any dialog levels in it, load them in for each, repeat each level
    }

    // TODO make algo to path for quests through the tree to do NPC talking

    private DialogData LoadDialogLevel(string DialogLevelFileName)
    {
        string FullSavePath =
            Path.Combine(this.DialogLevelSaveLocation, DialogLevelFileName);

        Debug.Log("loading dialog from " + FullSavePath);
        StreamReader reader = new StreamReader(FullSavePath);
        string json = reader.ReadToEnd();
        reader.Close();

        DialogData Dialog = JsonUtility.FromJson<DialogData>(json);

        Debug.Log("loaded dialog");
        Debug.Log(Dialog.levels);
        return Dialog;
    }

    /*
    private void DoDialog()
    {
        // where main look of dialog goes
        // now that has all lines for a and b, push options to each character, and wait for response
        // for each character make list of `dialog options`
        /*

        make options for a and b
        
        get
        a selection
        b selection


        //*
        // take turns
        if (TalkingUUID == AUUID)
        {
            if (CharacterA.GetIsPlayer())
            {
                // get is player or not present and wait for player input
            }
            else
            {
                // do same for character a and b npc
                StartCoroutine(WaitThenPick(5, CharacterA));
            }

            TalkingUUID = BUUID;
        }
        else
        {
            StartCoroutine(WaitThenPick(5, CharacterB));

            TalkingUUID = AUUID;
        }
    }

    IEnumerator WaitThenPick(int howLongToWait, CharacterController WhosPicking) // wait n seconds and then get characters choice
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(howLongToWait);

        //get characters choice of option

        // then choose it
        //ChooseDialogOption(0);
    }

    IEnumerator WaitForPlayerChoice()
    {
        // wait until an input key is pressed of the number inputs
        // tell players menu that they are not in squad swap mode (might be already done)
        return;
    }

    private void FillDialogLists()
    {
        MakeListOfDialogOptions (DialogListA, CharacterA);
        MakeListOfDialogOptions (DialogListB, CharacterB);
    }

    private void SayFromCharacter(string text, CharacterController speaker)
    {
        speaker.MakeSpeechBubble (text);
    }

    private void MakeListOfDialogOptions(List<DialogOption> ListToFill)
    {
        ListToFill = new List<DialogOption>();
        int count = 0;

        // if character has quest, add quest options to list
        // if character is shop, add shop options to list
        // for item in shop, make  options (i wanna by x -> yes -> how many/nvm
        //-> nvm)
        // if in same town, add wanna join
        // if in different town, add wanna join town if needed
        // add "bye"
        DialogOption bye = new DialogOption();
        bye.SelectionNumber = count + 1;
        bye.Text = "Bye";
        by.Type = "EXIT";
        count += 1;
    }

    public void ChooseDialogOption(int option)
    {
        //pick dialog option and then call do-dialog
    }
    */
}


// dialog tree class

// contains levels of dialog

// its a list of lists

// outer list = levels
// each level has list of Dialog

public class DialogTree
{
    public DialogData Dialog;
    public int Level;
    public List<DialogTree> childBranches;
}