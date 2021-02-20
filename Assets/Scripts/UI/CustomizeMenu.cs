using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : MonoBehaviour
{
    
    private Canvas MenuCanvas;

    [SerializeField]
    GameObject StatusBox;

    private string statusText = "";

    private CharacterController character;

    private List<List<GameObject>>
        CustomizationOptionsMatrix = new List<List<GameObject>>();

    private int selectedList = 0;


    // TODO sort out hair issues with missing parts and make sure it comes with combos

    void Start()
    {
        MenuCanvas = this.gameObject.GetComponent<Canvas>();

        MenuCanvas.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            selectedList = selectedList + 1;
            if (selectedList >= CustomizationOptionsMatrix.Count)
            {
                selectedList = 0;
            }
            GenerateCurrentStatusText();
        }
        if (Input.GetKeyDown("i"))
        {
            selectedList = selectedList - 1;
            if (selectedList < 0)
            {
                selectedList = CustomizationOptionsMatrix.Count - 1;
            }
            GenerateCurrentStatusText();
        }
        if (Input.GetKeyDown("j"))
        {
            AddValueToSelectedIndexInList(-1);

            // generate new text
            GenerateCurrentStatusText();
        }
        if (Input.GetKeyDown("l"))
        {
            AddValueToSelectedIndexInList(1);

            // generate new text
            GenerateCurrentStatusText();
        }

        //Debug.Log("status text"+statusText);
        StatusBox.GetComponent<Text>().text = statusText;
    }

    public void GenerateCurrentStatusText()
    {
        List<string> ListNames =
            new List<string> {
                "Torso Option ",
                "Belt&Backpack Option ",
                "Head Option ",
                "Face Option ",
                "Hands/Arms Option ",
                "Shoulder Option ",
                "Shoes/Legs Option "
            };

        // blank text
        statusText = "";

        // working text
        string lineString = "";

        int i = 0;
        foreach (List<GameObject>
            customizationsList
            in
            CustomizationOptionsMatrix
        )
        {
            // get titles
            lineString = ListNames[i];

            int selectedOption =
                character
                    .GetIndexFromOneOfOptionInListThatIsActive(CustomizationOptionsMatrix[i]);
            int totalOptions = CustomizationOptionsMatrix[i].Count;

            lineString =
                lineString +
                selectedOption.ToString() +
                "/" +
                totalOptions.ToString();

            // if selected then make it green
            if (i == selectedList)
            {
                lineString = "<color=green>" + lineString + "</color>";
            }

            i = i + 1;

            // add to status text
            statusText += lineString + "\n";
        }
        //Debug.Log (i);
        //Debug.Log (statusText);
    }

    private void AddValueToSelectedIndexInList(int value)
    {
        List<int> customizationOptionsAllowedToBeNone = new List<int> { 1, 5 };

        int currentIndex =
            character
                .GetIndexFromOneOfOptionInListThatIsActive(CustomizationOptionsMatrix[selectedList]);
        int totalOptions = CustomizationOptionsMatrix[selectedList].Count;

        Debug.Log("current option " + currentIndex + "/" + totalOptions);

        int newIndex = currentIndex + value;

        //wrap around to 0 if too high
        if (newIndex > totalOptions)
        {
            newIndex = -1;
        }

        if (newIndex <= 0)
        {
            // wraps to -1, then do check if list is allowed to have none selected
            // if the current list isnt allowed to be selected, then make selection 0
            if (!customizationOptionsAllowedToBeNone.Contains(selectedList))
            {
                if (newIndex == 0)
                {
                    newIndex = totalOptions;
                }
                else
                {
                    newIndex = 1;
                }
            }
        }
        Debug.Log("new selection " + newIndex);

        /*
        // deactivate current
        CustomizationOptionsMatrix[selectedList][currentIndex].SetActive(false);
        // activate new
        CustomizationOptionsMatrix[selectedList][newIndex].SetActive(true);
        */
        // update the characters save data
        UpdateCharacterDataWithNewCustomizationSelection (
            newIndex,
            selectedList
        );

        // update current options from save data
        character.LoadCurrentCustomizedValuesFromCharacterData();
    }

    private void UpdateCharacterDataWithNewCustomizationSelection(
        int newSelection,
        int whichCustomizeListToUpdate
    )
    {
        switch (whichCustomizeListToUpdate)
        {
            case 0:
                character.GetCharacter().TorsoOption = newSelection;
                break;
            case 1:
                character.GetCharacter().BeltBackpackOption = newSelection;
                break;
            case 2:
                character.GetCharacter().HeadOption = newSelection;
                break;
            case 3:
                character.GetCharacter().FaceOption = newSelection;
                break;
            case 4:
                character.GetCharacter().HandsOption = newSelection;
                break;
            case 5:
                character.GetCharacter().ShoulderOption = newSelection;
                break;
            case 6:
                character.GetCharacter().ShoeOption = newSelection;
                break;
        }
    }

    public void PopulateCustomizationOptionsMatrix()
    {
        // gets all options in the matrix
        CustomizationOptionsMatrix = character.GetCustomizationMatrix();
    }

    public void SetCharacter(CharacterController newCharacter)
    {
        character = newCharacter;
    }
}
