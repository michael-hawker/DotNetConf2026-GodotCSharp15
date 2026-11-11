using Godot;

namespace GodotNet11;

// Extension Indexer Example
public static class NodeExtensions
{
    // Type to extend
    extension(Node node)
    {
        public Node this[int index] => node.GetChild(index);
    }
}