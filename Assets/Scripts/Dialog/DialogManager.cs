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

    private DialogTree Tree;

    private DialogTree TraversalTree;

    private int DialogSelection = -1;

    private bool DialogLoopEnabled = false;

    private bool CharacterBHasSpoken = false;

    private bool CharacterAHasSpoken = false;

    void Start()
    {
    }

    void Update()
    {
        // get dialog selection
        if (Input.GetKeyDown("1"))
        {
            DialogSelection = 0;
        }
        if (Input.GetKeyDown("2"))
        {
            DialogSelection = 1;
        }
        if (Input.GetKeyDown("3"))
        {
            DialogSelection = 2;
        }
        if (Input.GetKeyDown("4"))
        {
            DialogSelection = 3;
        }
        if (Input.GetKeyDown("5"))
        {
            DialogSelection = 4;
        }
        if (Input.GetKeyDown("6"))
        {
            DialogSelection = 5;
        }
        if (Input.GetKeyDown("7"))
        {
            DialogSelection = 6;
        }
        if (Input.GetKeyDown("8"))
        {
            DialogSelection = 7;
        }
        if (Input.GetKeyDown("9"))
        {
            DialogSelection = 8;
        }

        if (DialogLoopEnabled)
        {
            // do the per frame checks that dialog have happened
            DoDialog();
        }
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

        // clear dialog text
        if(CharacterA.GetIsPlayer()){
            CharacterA.SetDialogText("");
        }



        CharacterA.LeaveDialog();
        CharacterB.LeaveDialog();

        Debug.Log("End of dialog");

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
        // determine start of the tree / which file to load from
        string startOfDialogTree =
            DetermineDialogToLoadFromCharacterDefaultTask(CharacterB);

        // loads dialogtree into Tree var
        LoadDialogTree (startOfDialogTree);

        // set vars for init doDialog
        TraversalTree = Tree; // set tree to traverse to be the tree loaded
        DialogLoopEnabled = true; // enable main loop of dialog

        // start convo from A
        SayFromCharacter(TraversalTree.prompt, CharacterA);
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
    // main loop of dialog
    private void DoDialog()
    {
        // if its character B turn, say and then prompt A
        if (!CharacterBHasSpoken)
        {
            SayFromCharacter(TraversalTree.response, CharacterB);

            CharacterBHasSpoken = true;
            CharacterAHasSpoken = false;
        }
        else if (!CharacterAHasSpoken)
        {
            // if talking to player, remake prompts
            if (CharacterA.GetIsPlayer())
            {
                CharacterA.SetDialogText(CreatePlayerDialogPrompt());
            }

            // character B has spoken and it is A's turn
            // do call to make selection
            DialogTree returnedTree = MakeDialogChoice();
            if (
                returnedTree != null // waits until selection
            )
            {
                // get dialog selection
                //SayFromCharacter(DialogSelection.ToString(), CharacterA);
                SayFromCharacter(returnedTree.prompt, CharacterA);

                // process restult of dialog tree selection
                ProcessResultOfPickedOption(returnedTree.result);

                // move turn to character B
                CharacterAHasSpoken = true;
                CharacterBHasSpoken = false;
                TraversalTree = returnedTree;
            }
        }
    }

    // if player, make option to pick dialog picker, if npc get dialog from quest or current action
    private DialogTree MakeDialogChoice()
    {
        DialogTree returnTree = Tree;

        if (CharacterA.GetIsPlayer())
        {
            returnTree = MakePlayerPickDialogChoice();
            DialogSelection = -1;
        }
        else
        {
            // TODO NPC dialog picker
            Debug.Log("TODO");
        }

        // return new root tree
        return returnTree;
    }

    // TODO make npc pick dialog options
    private void MakeDialogChoiceFromQuest()
    {
        // uses npcn current quest to make dialog choice
    }

    private void MakeDialogChoiceFromCurrentTask()
    {
        // uses current task to make dialog choice
    }

    private DialogTree MakePlayerPickDialogChoice()
    {
        // prompts the player for dialog choice
        // based on number picked, pick that level as new tree to return
        // wait until player pushes buttons
        // get numbers 1-9 and swap to squad member
        // check that the selection is in the list, else ignore it
        // return the tree in levels item
        //Debug.Log("Dialog selection " +
        //DialogSelection.ToString() +
        //"levels len " +
        //TraversalTree.levels.Count);
        if (
            TraversalTree.levels.Count - 1 >= DialogSelection &&
            DialogSelection >= 0
        )
        {
            return TraversalTree.levels[DialogSelection];
        }

        return null;
    }

    private string CreatePlayerDialogPrompt()
    {
        string dialogPrompt = "";
        int i = 1;

        /*
        dialog prompt to look like this
        
        CharacterB.name : prev response

        1. currenttree.levels[0].prompt
        2. currenttree.levels[1].prompt
        3. currenttree.levels[2].prompt
        */
        // add prev response
        dialogPrompt =
            dialogPrompt +
            CharacterB.GetCharacter().Name +
            ": " +
            TraversalTree.response +
            "\n\n";

        foreach (DialogTree level in TraversalTree.levels)
        {
            dialogPrompt =
                dialogPrompt + i.ToString() + ". " + level.prompt + "\n";
            i += 1;
        }

        return dialogPrompt;
    }

    private void SayFromCharacter(
        string whatToSay,
        CharacterController CharacterToSayText
    )
    {
        CharacterToSayText.MakeSpeechBubble (whatToSay);
    }

    private string
    DetermineDialogToLoadFromCharacterDefaultTask(CharacterController character)
    {
        // TODO this
        return "farmer1.json";
    }

    private void LoadDialogTree(string DialogLevelFileName)
    {
        // populates Tree recusrivly using DialogTree classes to make the tree data structure
        Tree = LoadDialogLevelToDialogTree(DialogLevelFileName, 0);
    }

    // TODO make algo to path for quests through the tree to do NPC talking
    // load each level of the tree from file
    private DialogTree
    LoadDialogLevelToDialogTree(string DialogLevelFileName, int currentLevel)
    {
        string FullSavePath =
            Path.Combine(this.DialogLevelSaveLocation, DialogLevelFileName);

        //Debug.Log("loading dialog from " + FullSavePath);
        StreamReader reader = new StreamReader(FullSavePath);
        string json = reader.ReadToEnd();
        reader.Close();

        DialogData Dialog = JsonUtility.FromJson<DialogData>(json);

        //Debug.Log("loaded dialog " + DialogLevelFileName);
        //Debug.Log(Dialog.levels);
        // convert dialogdata to dialogtree
        DialogTree currentTreeObject = new DialogTree();

        //Debug.Log("creating tree " + DialogLevelFileName);
        // set level
        currentTreeObject.level = currentLevel;

        // set prompts
        currentTreeObject.prompt = Dialog.prompt;
        currentTreeObject.response = Dialog.response;
        currentTreeObject.result = Dialog.result;

        // levels list
        currentTreeObject.levels = new List<DialogTree>();

        // for each level, load it
        foreach (string level in Dialog.levels)
        {
            //Debug
            //    .Log("loading child dialog " +
            //    level +
            //    " from parent " +
            //    DialogLevelFileName);
            DialogTree temp =
                LoadDialogLevelToDialogTree(level, currentLevel + 1);
            currentTreeObject.levels.Add (temp);
        }

        //Debug.Log("loaded dialogtree " + DialogLevelFileName);
        return currentTreeObject;
    }

    private void ProcessResultOfPickedOption(string result)
    {
        if (result == "end")
        {
            EndDialog();
        }
    }
}

// dialog tree class
// same as DialogData Class but as a tree node
public class DialogTree
{
    public int level;

    public string prompt;

    public string response;

    public string result;

    public List<DialogTree> levels;
}
