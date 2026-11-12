using Godot;

namespace GodotNet11.States;

public closed record EnemyState;

public record EnemySeekingState(float Range) : EnemyState;
public record EnemyTargetState(Node2D Target, float LockedOnTime) : EnemyState;