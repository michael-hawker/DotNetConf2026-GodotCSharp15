using Godot;
using GodotNet11.States;

namespace GodotNet11;

public partial class Seeker : CharacterBody2D
{
	private const float Speed = 20.0f;
	private const float RotationSpeed = Mathf.Pi / 12; // Radians

	private readonly StateMachine<EnemyState> _stateMachine = new(new EnemySeekingState(240f));

	public override void _Ready()
	{
		Velocity = Vector2.Up * Speed;
	}

	public override void _Process(double delta)
	{
		switch (_stateMachine.CurrentState)
		{
			case EnemySeekingState enemySeekingState:
				var parent = GetParent();
				for (int i = 0; i < parent.GetChildCount(); i++)
				{
					// TODO: Figure out why we can't compare to Enemy union here...
					if (parent[i] is Player player && player.GlobalPosition.DistanceTo(GlobalPosition) < enemySeekingState.Range)
					{
						_stateMachine.GoToState(new EnemyTargetState(parent[i] as Node2D));
						break;
					}
				}
				break;
			case EnemyTargetState enemyTargetState:
				Rotation = (float)Mathf.RotateToward(Rotation, GlobalPosition.AngleToPoint(enemyTargetState.Target.GlobalPosition) + Mathf.Pi/2,
					delta * RotationSpeed);
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		velocity = velocity.Rotated(Rotation);
		
		var collide = MoveAndCollide(velocity * (float)delta);
		if (collide != null &&
		    collide.GetCollider() is Node node)
		{ 
			GD.Print($"Enemy hit {node.Name}");
		}
	}
}