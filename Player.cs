using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 20.0f;
	public const float RotationSpeed = 45f;

	public override void _Ready()
	{
		Velocity = new(0, -Speed);
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
		MoveAndSlide();
	}
}
