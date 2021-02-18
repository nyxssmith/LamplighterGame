using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Quest : MonoBehaviour
{

    /*
Quest
  Name
  Giver
  Reward
  Owner/doer
  List of stages
  Task to do when done / task before quest was started



    current stage in list (list index)
    */

    void Start()
    {



    }

    void Update()
    {

    }


    public void SetNextTask(){
        // get next stage
        // set its task etc as current
        // set next task to QUEST
    }

    // todo read from current stage, then
    public void GetDialogOptionToChoose(){
        // return int of what option to choose in a dialog
    }

}



public class QuestStage : MonoBehaviour
{
    /*

Stage
  Name
  Desc

resource value = name of which resource to buy or sell
target =  gameobject to goto or follow etc
value  = float

goto takes target and value is how close to get to count as complete
sethealth attack target until their health is below the value
buy buy #value of resourcevalue from vendor (target)
sell sell #value of resourcevalue to vendor (tagrtt)
fetch obtain target gameobject
give give target gameobject to giver

  Type (goto kill buy)



  Task to do (like type but exact task for character)

  Target transform
  List of dialog options to select 
  Target object name
    */
}