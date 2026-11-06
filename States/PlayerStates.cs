namespace GodotNet11.States;

public union PlayerStates(PlayerBaseState, PlayerFireState);

public record PlayerBaseState;

public class PlayerFireState(float cooldown)
{
    public float Cooldown { get; set; } = cooldown;
}