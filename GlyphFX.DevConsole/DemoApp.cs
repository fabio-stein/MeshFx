using System.Numerics;
using GlyphFX.Engine;

namespace GlyphFX.DevConsole;

public class DemoApp : AppStateManager
{
    private float speed = 0.1f;
    private float movement = 0f;

    public override void Start()
    {
        var data = SimpleLoader.LoadGltf();
        LoadTexture(data.texture);
        World.LoadScene(data.scene);
    }

    public override void Update()
    {
        if (Input.IsButtonDown)
        {
            movement += speed * (Input.Button == 0 ? -1 : 1);
        }
        
        UpdateVertex();
    }

    private void UpdateVertex()
    {
        var obj1 = Matrix4x4.CreateTranslation(0f, 0, 0);
        var obj2 = Matrix4x4.CreateTranslation(movement, -0.2f , -0.5f + movement);
        
        var instances = new Matrix4x4[] { obj1, obj2 };
        SetInstanceMatrix(instances);
    }
}