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
            new Vertex { position = new Vec3 { x = left.x, y = left.y, z = 0.0f }, color = new ColorRGB { r = 0.0f, g = 1.0f, b = 0.0f } },
            new Vertex { position = new Vec3 { x = left.x, y = right.y, z = 0.0f }, color = new ColorRGB { r = 1.0f, g = 0.0f, b = 1.0f } },
            new Vertex { position = new Vec3 { x = right.x, y = right.y, z = 0.0f }, color = new ColorRGB { r = 0.0f, g = 0.0f, b = 1.0f } },
            new Vertex { position = new Vec3 { x = right.x, y = left.y, z = 0.0f }, color = new ColorRGB { r = 1.0f, g = 1.0f, b = 0.0f } }
        ];
        
       SetVertices(vertices);
        
        var indices = new ushort[] { 0, 1, 2, 0, 2, 3};
        
        SetIndices(indices);
    }
}