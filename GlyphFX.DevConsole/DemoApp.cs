using System.Numerics;
using GlyphFX.Engine;

namespace GlyphFX.DevConsole;

public class DemoApp : EventApp
{
    private Vector3 eye = new Vector3(0, 4.0f, 10.0f);
    private Vector3 target = new Vector3(0, 0, 0f);
    private Vector3 up = new Vector3(0, 1, 0);
    private float fovDegrees = 60.0f;
    private float aspect = 1f;
    private float znear = 0.1f;
    private float zfar = 100.0f;

    private float speed = 0.01f;
    private float rotationAngle = 0.0f;
    
    public override void Start()
    {
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
        var left = new Vec3(-0.5f, -0.5f, -0.5f);
        var right = new Vec3(0.5f, 0.5f, 0.5f);
        
        Vertex[] vertices =
        [
            new Vertex { position = new Vec3(left.x, left.y, right.z), textureCoords = new Vec2(0, 1) },
            new Vertex { position = new Vec3(right.x, left.y, right.z), textureCoords = new Vec2(1, 1) },
            new Vertex { position = new Vec3(right.x, right.y, right.z), textureCoords = new Vec2(1, 0) },
            new Vertex { position = new Vec3(left.x, right.y, right.z), textureCoords = new Vec2(0, 0) },
            
            new Vertex { position = new Vec3(left.x, left.y, left.z), textureCoords = new Vec2(0, 1) },
            new Vertex { position = new Vec3(right.x, left.y, left.z), textureCoords = new Vec2(1, 1) },
            new Vertex { position = new Vec3(right.x, right.y, left.z), textureCoords = new Vec2(1, 0) },
            new Vertex { position = new Vec3(left.x, right.y, left.z), textureCoords = new Vec2(0, 0) }
        ];
        
        SetVertices(vertices);
        
        var indices = new ushort[]
        {
            0, 1, 2, 0,2,3,
            4,0,3,4,3,7,
            1,5,6,1,6,2,
            2,6,7,2,7,3,
            4,5,1,4,1,0,
            5,4,7,5,7,6,
        };
        
        SetIndices(indices);
        
        var obj1 = Matrix4x4.CreateTranslation(-0.6f, 0, 0);
        var obj2 = Matrix4x4.CreateTranslation(speed, -0.2f, -0.5f);
        
        var instances = new Matrix4x4[] { obj1, obj2 };
        SetInstanceMatrix(instances);
    }
}