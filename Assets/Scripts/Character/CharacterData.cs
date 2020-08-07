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
    public bool IsFollower;
    public bool IsFollowing;


    public string idle_animation;
    public string walking_forward_animation;
    public string running_forward_animation;
    public string walking_backward_animation;
    public string running_backward_animation;
    public string landing_animation;
    public string midair_animation;
    public string jump_animation;


}