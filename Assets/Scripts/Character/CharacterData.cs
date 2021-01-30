[System.Serializable]
public class CharacterData
{
    public string Name;
    public float CurrentHealth;
    public float MaxHealth;

    public float CurrentMana;
    public float MaxMana;
    public float JumpHeight;
    public float CurrentStamina;
    public float MaxStamina;
    public float StaminaBonusSpeed;
    public float StaminaRechargeRate;
    public float StaminaUseRate;
    public float CurrentSpeed;
    public float BaseMovementSpeed;
    public float Reach;
    public float TargetRange;
    public bool CanFight;
    public bool IsPlayer;
    public bool IsFollower;
    public bool IsFollowing;
    public string DefaultTask;


    public string idle_animation;
    public string walking_forward_animation;
    public string running_forward_animation;
    public string walking_backward_animation;
    public string running_backward_animation;
    public string landing_animation;
    public string midair_animation;
    public string jump_animation;


    public float x_pos;
    public float y_pos;
    public float z_pos;

    public string id;
    public string squadLeaderId;

    public float magic_faction;
    public float tech_faction;
    public float bandit_faction;
    public float lamplighter_faction;
    public float settlements_faction;
    public float farmer_faction;
    public float wild_faction;


    public int TorsoOption;
    public int BeltBackpackOption;
    public int HeadOption;
    public int FaceOption;
    public int HandsOption;
    public int ShoulderOption;
    public int ShoeOption;

}