using CommunityToolkit.Mvvm.Messaging;
using Godot;
using GodotNet11.Helpers;
using GodotNet11.States;

namespace GodotNet11;

public partial class Seeker : CharacterBody2D
{
	private const float Speed = 20.0f;
	private const float RotationSpeed = Mathf.Pi / 12; // Radians

	private readonly StateMachine<EnemyState> _stateMachine = new(new EnemySeekingState(240f));

	private double _lockTime;
	
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
					// Use a helper to check general object in union
					if (Target.TryFrom(parent[i], out var target) &&
					    target is Node2D node &&
					    node.GlobalPosition.DistanceTo(GlobalPosition) < enemySeekingState.Range)
					{
						_lockTime = 0f;
						_stateMachine.GoToState(new EnemyTargetState(parent[i] as Node2D, target.Value is Player ? 5f : 10f));
						break;
					}
				}
				break;
			case EnemyTargetState enemyTargetState:
				Rotation = (float)Mathf.RotateToward(Rotation, GlobalPosition.AngleToPoint(enemyTargetState.Target.GlobalPosition) + Mathf.Pi/2,
					delta * RotationSpeed);
				
				_lockTime += delta;
				if (_lockTime > enemyTargetState.LockedOnTime)
				{
					_stateMachine.GoToState(new EnemySeekingState(180f));
				}
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		velocity = velocity.Rotated(Rotation);
		
		var collide = MoveAndCollide(velocity * (float)delta);
		if (collide != null &&
		    collide.GetCollider() is Node2D node)
		{ 
			GD.Print($"Enemy hit {node.Name}");
			ProcessMode = ProcessModeEnum.Disabled;
			node.ProcessMode = ProcessModeEnum.Disabled;
			StrongReferenceMessenger.Default.Send<ExplosionMessage>(new(this, node));
		}
	}
}