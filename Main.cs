using CommunityToolkit.Mvvm.Messaging;
using Godot;
using GodotNet11.Helpers;

namespace GodotNet11;

public partial class Main : Node2D,
	IRecipient<ExplosionMessage>
{
	[Export] 
	public Node2D EffectLayer { get; set; }
	
	[Export] 
	public Node2D SpriteLayer { get; set; }
	
	[Export]
	public PackedScene ExplosionScene { get; set; }
	
	public override void _EnterTree()
	{
		StrongReferenceMessenger.Default.Register<ExplosionMessage>(this);
	}

	public override void _ExitTree()
	{
		StrongReferenceMessenger.Default.Unregister<ExplosionMessage>(this);
	}

	public async void Receive(ExplosionMessage message)
	{
		if (message.obj1 is not Bullet)
		{
			Node2D explosion1 = ExplosionScene.Instantiate<Node2D>();
			explosion1.GlobalPosition = message.obj1.GlobalPosition;
			EffectLayer.AddChild(explosion1);
		}

		if (message.obj2 is not Bullet)
		{
			Node2D explosion2 = ExplosionScene.Instantiate<Node2D>();
			explosion2.GlobalPosition = message.obj2.GlobalPosition;
			EffectLayer.AddChild(explosion2);
		}

		await ToSignal(GetTree().CreateTimer(1.3f), SceneTreeTimer.SignalName.Timeout);

		RemoveObjectsFromLayers([message.obj1, message.obj2], [SpriteLayer, EffectLayer]);
	}

	private void RemoveObjectsFromLayers(Node2D[] objects, Node2D[] layers)
	{
		objloop: foreach (var obj in objects)
		{
			foreach (var layer in layers)
			{
				for (int i = layer.GetChildCount() - 1; i >= 0; i--)
				{
					if (layer[i] == obj)
					{
						// If this was index based...
						layer.RemoveChild(obj);
						GD.Print($"Removing {obj.Name}");
						// TODO: Add/Sub points based on what was destroyed?
						obj.QueueFree();
						
						// Move to next object
						continue objloop;
					}
				}
			}
		}
	}
}