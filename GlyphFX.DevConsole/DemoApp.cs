using System.Numerics;
using GlyphFX.Engine;
using SharpGLTF.Schema2;

namespace GlyphFX.DevConsole;

public class DemoApp : EventApp
{
    private Vector3 eye = new Vector3(0, 2.0f, 4.0f);
    private Vector3 target = new Vector3(0, 0, 0f);
    private Vector3 up = new Vector3(0, 1, 0);
    private float fovDegrees = 60.0f;
    private float aspect = 1f;
    private float znear = 0.1f;
    private float zfar = 100.0f;

    private float speed = 0.01f;
    private float rotationAngle = 0.0f;

    private ModelRoot model;
    private List<Vertex> vertices = new();
    
    public override void Start()
    {
        model = ModelRoot.Load("model.glb");
        var position = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("POSITION").AsVector3Array().ToList();
        var normal = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("NORMAL").AsVector3Array().ToList();
        var texCoord = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("TEXCOORD_0").AsVector2Array().ToList();
        
        for (int i = 0; i < position.Count; i++)
        {
            var posMap = new Vec3(position[i].X, position[i].Y, position[i].Z);
            var texMap = new Vec2(texCoord[i].X, texCoord[i].Y);
            var normMap = new Vec3(normal[i].X, normal[i].Y, normal[i].Z);
            vertices.Add(new Vertex(posMap, texMap, normMap));
        }

        var texture = model.LogicalTextures.First().PrimaryImage.Content.Content.ToArray();
        LoadTexture(ref texture);
    }

    public override void Update()
    {
        if (Input.IsButtonDown)
        {
            speed += (Input.Button == 0) ? -0.01f : 0.01f;
        }
        
        UpdateVertex();

        var degrees = 60.0f;
        float fovRadians = degrees * (float)Math.PI / 180.0f;
        
        Matrix4x4 view = Matrix4x4.CreateLookAt(eye, target, up);
        
        
        Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, aspect, znear, zfar);
            
        Matrix4x4 viewProjection = view * proj;
            
        SetCamera(viewProjection);
    }

    private void UpdateVertex()
    {
        SetVertices(vertices.ToArray());
        
        var indices = new UInt32[]
        {
            0, 1, 2
        };
        
        SetIndices(indices);
        
        var obj1 = Matrix4x4.CreateTranslation(0f, 0, 0);
        var obj2 = Matrix4x4.CreateTranslation(speed, -0.2f , -0.5f + speed);
        
        var instances = new Matrix4x4[] { obj1, obj2 };
        SetInstanceMatrix(instances);
    }
}