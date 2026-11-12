using Godot;

namespace GodotNet11;

public partial class Follower : CharacterBody2D
{
	[Export]
	public Node2D Target { get; set; }
	
	private const float RotationSpeed = Mathf.Pi / 6; // Radians

	public override void _PhysicsProcess(double delta)
	{
		Rotation = (float)Mathf.RotateToward(Rotation, GlobalPosition.AngleToPoint(Target.GlobalPosition) + Mathf.Pi/2,
			delta * RotationSpeed);

		var distance = GlobalPosition.DistanceTo(Target.GlobalPosition);

		Vector2 velocity = Vector2.Up * (distance >= 50f ? 32f : 16f); 
		velocity = velocity.Rotated(Rotation);
		
		var collide = MoveAndCollide(velocity * (float)delta);
		if (collide?.GetCollider() is Node node)
		{ 
			GD.Print($"Follower hit {node.Name}");
		}
	}
}
