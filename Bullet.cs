using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	public float TimeToLive { get; set; } = 3.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Parent well set initial velocity to itself, but apply extra impulse here.
		ApplyImpulse(LinearVelocity * 2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TimeToLive -= (float)delta;

		if (TimeToLive < 0)
		{
			QueueFree();
		}
	}
}
