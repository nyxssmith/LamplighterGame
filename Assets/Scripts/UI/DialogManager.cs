using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{

    // manages dialog between 2 characters

    private CharacterController CharacterA;
    private CharacterController CharacterB;

    public string AUUID;
    public string BUUID;
    public string TalkingUUID;


    public List<DialogOption> DialogListA = new List<DialogOption>();
    public List<DialogOption> DialogListB = new List<DialogOption>();

    void Start()
    {



    }

    void Update()
    {

    }


    private void OnTriggerExit(Collider ExitingCharacter)
    {

        CharacterController ExitingCharacterController = ExitingCharacter.GetComponent<CharacterController>();
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

        TalkingUUID = AUUID;// A started the dialog


        // make dialog
        FillDialogLists();

        // do dialog
        DoDialog();
        //



    }


    private void DoDialog(){
        // where main look of dialog goes


        // now that has all lines for a and b, push options to each character, and wait for response
        
        // for each character make list of `dialog options`

        /*

        make options for a and b
        
        get
        a selection
        b selection


        */

        

        // take turns
        if(TalkingUUID == AUUID){


        if(CharacterA.GetIsPlayer()){
            // get is player or not present and wait for player input
        }else{
            // do same for character a and b npc



                StartCoroutine(WaitThenPick(5,CharacterA));
    
        }

            TalkingUUID = BUUID
        }else{

        StartCoroutine(WaitThenPick(5,CharacterB));
    

            TalkingUUID = AUUID;
        }
        

    }

    IEnumerator WaitThenPick(int howLongToWait,CharacterController WhosPicking)// wait n seconds and then get characters choice
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

    }



    private void FillDialogLists(){
        MakeListOfDialogOptions(DialogListA,CharacterA);
        MakeListOfDialogOptions(DialogListB,CharacterB);
    }

    private void SayFromCharacter(string text, CharacterController speaker){
        speaker.MakeSpeechBubble(text);
    }


    private void MakeListOfDialogOptions(List<DialogOption> ListToFill){
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
        bye.SelectionNumber = count+1;
        bye.Text = "Bye";
        by.Type="EXIT";
        count+=1;



    }


    public void ChooseDialogOption(int option){
        //pick dialog option and then call do-dialog
    }

}



public class DialogOption : MonoBehaviour
{

    /*
    selection number
    text
    next dialog option = null

    */
    
    public int SelectionNumber; // -1 if taking input for ammount // todo maybe
    public string Text;
    public string Type; // BUY EXIT JOIN QUEST
    public DialogOption NextOption;



}