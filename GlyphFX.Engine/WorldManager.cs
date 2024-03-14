using System.Numerics;

namespace GlyphFX.Engine;

public class WorldManager
{
    // private List<Scene> _scenes = new List<Scene>();
    // private List<Node> _nodes = new List<Node>();
    // private List<Mesh> _meshes = new List<Mesh>();
}

public class Scene(Node[] nodes)
{
    public Node[] Nodes { get; private set; } = nodes;
}

public class Node(Node[] children, Mesh mesh)
{
    public Node[] Children { get; private set; } = children;
    public Mesh Mesh { get; private set; } = mesh;
}

public class Mesh(MeshPrimitive[] primitives)
{
    public MeshPrimitive[] Primitives { get; private set; } = primitives;
}

public class MeshPrimitive(Vertex[] vertices, UInt32[] indices)
{
    private Vertex[] _vertices = vertices;
    private UInt32[] _indices = indices;
    //private Material _material; //TODO
}