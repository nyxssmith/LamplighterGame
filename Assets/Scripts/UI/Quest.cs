using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestManager{

    // TODO make this an obj in the world so i can give it prefabs
    /*

    has loader to make quest from json
    has saver to return json from quest

    has addStage(questStage)




    */

    public Quest MakeNewQuest(CharacterController Giver, CharacterController Doer){
        // TODO make this
        /*
        copy doer current task to resume task
        set controllers
        */
        return null;
    }
    public Quest LoadQuestFromJsonPath(string path){
        // TODO make this load a quest from json and return object
        return null;
    }

    public string MakeJsonFromQuest(Quest toJson){

        SaveableQuest jsonQuest = new SaveableQuest();
        jsonQuest.name = toJson.name;
        jsonQuest.description = toJson.description;
        jsonQuest.current_stage = toJson.current_stage;
        jsonQuest.resume_task = toJson.resume_task;
        jsonQuest.giver = toJson.giver.GetUUID();
        jsonQuest.doer = toJson.doer.GetUUID();
        jsonQuest.tasks = toJson.tasks;

        string json = JsonUtility.ToJson(jsonQuest); // char

        return json;
    }


    public Quest GenerateQuestOfType(CharacterController Giver, CharacterController Doer,string type,int level){

        // TODO
        /*

        generate quest of a  type

        type : string : what catigory of quest to make
        level : number or difficulty of the quest

        types can be
         - lamplight : will make a quest to goto $number of nearest lamps : used by lamplighter each night is newly created
         - kill_bandit : will be a quest that spawns $number ~+ 1 and set one as the target or all as target based on level
            ex: level 1 might spawn 1 bandit and told to kill, or spawn 2 and kill 1 where level 3 might be spawn 3-4 and kill 3
         - raid : will make a goto quest and told to sethealth of $level npcs to 70 or less or do $level of damage : used by bandit raids
         - buy_me : will pick a store based on level and buy an item for $money
         - find_me : will spawn an item in and it must be fetched with $level being distance away it is



        */

        return null;
    }



}

[System.Serializable]
public class SaveableQuest{

public string name;
public string description;
public string giver;
public string doer;
public int current_stage;
public string resume_task;

public List<QuestStage> tasks;
}

public class Quest 
{

    /*
Quest
  Name : string : name of quest
  Description : string : desc
  Giver : character controller : who gave quest
  Doer : character controller : who is doing quest
  Current Stage : int : index in list of tasks that is current one
  Tasks : list of questStage : stages that make up the quest
  ResumeTask : string : Task to do when done / task before quest was started (for npc)

 Doer and Giver save as charactercontroller UUIDs


    */

public string name;
public string description;
public CharacterController giver;
public CharacterController doer;
public int current_stage;
public string resume_task;

public List<QuestStage> tasks;




}



[System.Serializable]
public class QuestStage 
{
    /*

Stage
  Name : string : name of stage
  Desc : string : desc of stage / instructions
  value_a : float : used to store # of health to bring to, reward number etc
  value_b : float : used to store more data info
  target : string : UUID of character/building/item that is referenced in the step
  type : string : type of step the step is


types:

goto : get $target uuid and goes within $value_a meters of it, $value_b is 1 uuid is character, 2 is building 3 is item to tell it what uuid list to reference
sethealth : $target uuid is character, item in $value_a is health threshold that must be less_or_equal for this stage to pass, set to 0 to kill target
buy : talks to nearest merchant and buys $value_a number of resource where $target is the resource name of what to buy, or the UUID of the item to buy and $value_b is how much this will cost
sell : talks to nearest merchant and sells $value_a of $target resource (or item) for $value_b profit
fetch : picks up item uuid that is in $target, will steal it if its owned in a shop
give : drops $target item uuid if they have it
reward : does different actions based on $value_a (1 for become follower and get faction points 2 for get resource)
    1 : giver sets the allowedToFollowPlayer to true
    2 : doer gets $value_b amount of resource in $target
    3 : other  / TODO


if UUID of character or item or building is missing, then that stage is marked as complete and moves on

buy and sell will be referenced by dialog manager to make a tree for npc to navigate or create


example quest to go to store and buy an item for another npc

GOTO shop building uuid
buy item UUID
GOTO giver uuid
give item uuid
reward



example quest to kill 3 bandits
GOTO bandit uuid
sethealth 0 bandit uuid
GOTO bandit uuid
sethealth 0 bandit uuid
GOTO bandit uuid
sethealth 0 bandit uuid
GOTO giver
reward

    */



public string Name;
public string Desc;
public float value_a;
public float value_b;
public string target;
public string type;


}