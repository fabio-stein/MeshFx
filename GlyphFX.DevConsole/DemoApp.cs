using GlyphFX.Engine;

namespace GlyphFX.DevConsole;

public class DemoApp : EventApp
{
    Vec2? left = null;
    Vec2? right = null;
    bool lastButtonDown = false;
    
    public override void Start()
    {
    }

    public override void Update()
    {
        if (Input.IsButtonDown)
        {
            if (Input.IsButtonDown != lastButtonDown)
                left = null;

            if (left == null)
                left = new Vec2(Input.MousePosition.x, Input.MousePosition.y);
            
            right = new Vec2(Input.MousePosition.x, Input.MousePosition.y);
            UpdateVertex();
        }
        
        lastButtonDown = Input.IsButtonDown;
    }

    private void UpdateVertex()
    {
        var left = this.left ?? new Vec2(0, 0);
        var right = this.right ?? new Vec2(0, 0);
        
        Vertex[] vertices =
        [
            new Vertex { position = new Vec3(left.x, left.y, 0), textureCoords = new Vec2(0, 0) },
            new Vertex { position = new Vec3(right.x, left.y, 0), textureCoords = new Vec2(1, 0) },
            new Vertex { position = new Vec3(right.x, right.y, 0), textureCoords = new Vec2(1, 1) },
            new Vertex { position = new Vec3(left.x, right.y, 0), textureCoords = new Vec2(0, 1) }
        ];
        
        SetVertices(vertices);
        
        var indices = new ushort[] { 0, 1, 2, 0, 2, 3};
        
        SetIndices(indices);
    }
}