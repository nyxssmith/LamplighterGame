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

        // load what dialog to load from file

        // do dialog
        DoDialog();
        //



    }


    private void DoDialog(){
        // where main look of dialog goes


        // now that has all lines for a and b, push options to each character, and wait for response


        


    }

}
