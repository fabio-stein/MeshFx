using System.Numerics;

namespace GlyphFX.Engine;

public class WorldManager
{
    // private List<Scene> _scenes = new List<Scene>();
    // private List<Node> _nodes = new List<Node>();
    // private List<Mesh> _meshes = new List<Mesh>();
    
    public Scene CurrentScene { get; private set; }
    public Camera CurrentCamera { get; private set; }
    
    public void LoadScene(Scene scene)
    {
        CurrentScene = scene;
    }
    
    public void SetCamera(Camera camera)
    {
        CurrentCamera = camera;
    }
}

public class Scene(Node[] nodes)
{
    public Node[] Nodes { get; private set; } = nodes;
}

public class Node(Node[]? children, Mesh? mesh, Matrix4x4? baseMatrix = null, Node.NodeTransform? transform = null)
{
    public Node[]? Children { get; private set; } = children;
    public readonly Matrix4x4 BaseMatrix = baseMatrix ?? Matrix4x4.Identity;
    public NodeTransform Transform { get; private set; } = transform ?? new NodeTransform();
    public Matrix4x4 LocalMatrix =>  Matrix4x4.CreateScale(Transform.Scale) * Matrix4x4.CreateFromQuaternion(Transform.Rotation) * Matrix4x4.CreateTranslation(Transform.Translation) * BaseMatrix;
    public Mesh? Mesh { get; private set; } = mesh;
    
    public class NodeTransform()
    {
        public Vector3 Translation = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;
    }
}

public class Mesh(MeshPrimitive[] primitives)
{
    public MeshPrimitive[] Primitives { get; private set; } = primitives;
}

public class MeshPrimitive(Vertex[] vertices, UInt32[] indices)
{
    public readonly Vertex[] Vertices = vertices;
    public readonly UInt32[] Indices = indices;
    //private Material _material; //TODO
}

public class Material(byte[] textureData)
{
    public readonly byte[] TextureData = textureData;
}

public class Camera
{
    private Vector3 _eye = new Vector3(0, 2.0f, 4.0f);
    private Vector3 _target = new Vector3(0, 0, 0f);
    private Vector3 _up = new Vector3(0, 1, 0);
    private float _fovDegrees = 60.0f;
    private float _aspectRatio = 1f;
    private float _znear = 0.1f;
    private float _zfar = 100.0f;
    
    public Matrix4x4 ViewProjection { get; private set; }

    public Camera()
    {
        UpdateProjection();
    }
    
    private void UpdateProjection()
    {
        float fovRadians = _fovDegrees * (float)Math.PI / 180.0f;
        Matrix4x4 view = Matrix4x4.CreateLookAt(_eye, _target, _up);
        Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, _aspectRatio, _znear, _zfar);
        ViewProjection = view * proj;
    }
}