using System.Numerics;
using GlyphFX.Common.Native.Primitives;

namespace GlyphFX.Common.Scenes;

public class SceneManager
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

public class Scene(List<Node>? nodes = null)
{
    public List<Node> Nodes { get; private set; } = nodes ?? new List<Node>();
}

public class Node
{
    private List<Node> _children = new();

    public IReadOnlyList<Node> Children => _children.AsReadOnly();
    //public readonly Matrix4x4 BaseMatrix = baseMatrix ?? Matrix4x4.Identity;
    public NodeTransform LocalTransform { get; private set; }
    public NodeTransform WorldTransform { get; private set; } = new NodeTransform(null, null, null);
    public Node Parent { get; private set; }
    public Mesh? Mesh { get; private set; }
    
    public Node(Mesh? mesh, Matrix4x4? baseMatrix = null, Node.NodeTransform? transform = null)
    {
        LocalTransform = transform ?? new NodeTransform(null, null, null);
        Mesh = mesh;
        UpdateWorldTransform();
    }
    
    public void SetParent(Node parent)
    {
        if (Parent != null)
            Parent._children.Remove(this);
        Parent = parent;
        Parent?._children.Add(this);
        UpdateWorldTransform();
    }

    public void UpdateWorldTransform()
    {
        WorldTransform = LocalTransform.Clone();
        if (Parent != null)
            WorldTransform.ApplyParentTransform(Parent.WorldTransform);
        
        foreach (var child in Children)
            child.UpdateWorldTransform();
    }
    
    public void Rotate(Vector3 axis, float angle, RotationSpace space = RotationSpace.Local)
    {
        Quaternion rotationIncrement = Quaternion.CreateFromAxisAngle(axis, angle);
        
        if (space == RotationSpace.Local)
            LocalTransform.Rotation *= rotationIncrement;
        else
        {
            //For world space rotation we basically create a new rotation with Parent's Transform and rotating it then use that to get the final rotation target
            //if we subtract both we get the increment needed for applying into Local Transform and getting the same position
            Quaternion worldRotation = Parent?.WorldTransform?.Rotation ?? Quaternion.Identity;
            var localRotationIncrement = Quaternion.Concatenate(rotationIncrement, worldRotation) *
                                         Quaternion.Inverse(worldRotation);
            LocalTransform.Rotation *= localRotationIncrement;
        }

        UpdateWorldTransform();
    }
    
    public enum RotationSpace
    {
        Local,
        World
    }
    
    public class NodeTransform
    {
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public NodeTransform(Vector3? translation, Quaternion? rotation, Vector3? scale)
        {
            Translation = translation ?? Vector3.Zero;
            Rotation = rotation ?? Quaternion.Identity;
            Scale = scale ?? Vector3.One;
        }
        
        public Matrix4x4 GetTransformationMatrix()
        {
            return Matrix4x4.CreateScale(Scale)
                   * Matrix4x4.CreateFromQuaternion(Rotation)
                   * Matrix4x4.CreateTranslation(Translation)
                   ;
        }
        
        public void ApplyParentTransform(NodeTransform parentTransform)
        {
            Translation = Vector3.Transform(Translation, parentTransform.Rotation);
            Translation *= parentTransform.Scale;
            Translation += parentTransform.Translation;

            Rotation = parentTransform.Rotation * Rotation;
            Scale *= parentTransform.Scale;
        }
        
        public NodeTransform Clone()
        {
            return (NodeTransform)MemberwiseClone();
        }
    }
}

public class Mesh(MeshPrimitive[] primitives)
{
    public MeshPrimitive[] Primitives { get; private set; } = primitives;
}

public class MeshPrimitive(Vertex[] vertices, uint[] indices, Material material)
{
    public readonly Vertex[] Vertices = vertices;
    public readonly UInt32[] Indices = indices;
    public readonly Material Material = material;
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