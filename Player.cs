using System;
using CommunityToolkit.Mvvm.Messaging;
using Godot;
using GodotNet11.Helpers;
using GodotNet11.States;

namespace GodotNet11;

public union Target(Player, Follower)
{
	public static bool TryFrom(object? obj, out Target? target)
	{
		target = obj switch
		{
			Player p => p,
			Follower f => f,
			_ => null,
		};
		return target is not null;
	}
}

public partial class Player : CharacterBody2D
{
	public const float Speed = 24.0f;
	public const float RotationSpeed = 45f;

	[Export]
	public PackedScene BulletScene { get; set; }
	
	[Export]
	public Node2D BulletLayer { get; set; }

	private StateMachine<PlayerStates> _stateMachine = new(new PlayerBaseState());
	
	public override void _Ready()
	{
		Velocity = new(0, -Speed);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept") &&
		    _stateMachine.CurrentState is PlayerBaseState)
		{
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			bullet.AddCollisionExceptionWith(this); // prevent our own bullet from hitting us!
			bullet.Rotation = Rotation;
			bullet.Position = GlobalPosition + new Vector2(0, -16).Rotated(Rotation); // Fire bullet in-front of us
			bullet.LinearVelocity = Velocity;
			BulletLayer.AddChild(bullet);
			
			_stateMachine.GoToState(new PlayerFireState(1.0f));
		}

		if (_stateMachine.CurrentState is PlayerFireState fireState)
		{
			fireState.Cooldown -= (float)delta;
			if (fireState.Cooldown <= 0)
			{
				_stateMachine.GoToState(new PlayerBaseState());
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			var rotation = (float)Mathf.DegToRad(RotationSpeed * delta * direction.X);
			Rotate(rotation);
			velocity = velocity.Rotated(rotation);
		}

		Velocity = velocity;
		var collide = MoveAndCollide(Velocity * (float)delta);
		if (collide?.GetCollider() is Node2D node)
		{ 
			GD.Print($"Player hit {node.Name}");
			ProcessMode = ProcessModeEnum.Disabled;
			StrongReferenceMessenger.Default.Send<ExplosionMessage>(new(this, node));
		}
	}
}