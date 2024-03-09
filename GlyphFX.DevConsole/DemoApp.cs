using System.Numerics;
using GlyphFX.Engine;

namespace GlyphFX.DevConsole;

public class DemoApp : EventApp
{
    private Vector3 eye = new Vector3(0, 1.0f, 2.0f);
    private Vector3 target = new Vector3(0, 0, 0f);
    private Vector3 up = new Vector3(0, 1, 0);
    private float fovDegrees = 60.0f;
    private float aspect = 1f;
    private float znear = 0.1f;
    private float zfar = 100.0f;
    
    public override void Start()
    {
    }

    public override void Update()
    {
        if (Input.IsButtonDown)
        {
            UpdateVertex();
            eye.X += ((Input.Button == 0) ? 1 : -1) * 0.01f;

            var degrees = 60.0f;
            float fovRadians = degrees * (float)Math.PI / 180.0f;
            
            Matrix4x4 view = Matrix4x4.CreateLookAt(eye, target, up);
            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(fovRadians, aspect, znear, zfar);
            
            Matrix4x4 viewProjection = view * proj;
            
            SetCamera(viewProjection);
        }
    }

    private void UpdateVertex()
    {
        var left = new Vec2(-0.5f, -0.5f);
        var right = new Vec2(0.5f, 0.5f);
        
        Vertex[] vertices =
        [
            new Vertex { position = new Vec3(left.x, left.y, 0.0f), textureCoords = new Vec2(0, 0) },
            new Vertex { position = new Vec3(right.x, left.y, 0.0f), textureCoords = new Vec2(1, 0) },
            new Vertex { position = new Vec3(right.x, right.y, 0.0f), textureCoords = new Vec2(1, 1) },
            new Vertex { position = new Vec3(left.x, right.y, 0.0f), textureCoords = new Vec2(0, 1) }
        ];
        
        SetVertices(vertices);
        
        var indices = new ushort[] { 0, 1, 2, 0, 2, 3};
        
        SetIndices(indices);
    }
}