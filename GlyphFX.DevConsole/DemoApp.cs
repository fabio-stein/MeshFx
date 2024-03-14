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
        
        var node = World.CurrentScene.Nodes.First();
        node.Transform.Translation.X = movement;
    }
}