using Godot;

namespace GodotNet11;

public partial class Explosion : AnimatedSprite2D
{
	public override void _Ready()
	{
		// Remove when complete
		AnimationFinished += () => { QueueFree(); };
	}
}