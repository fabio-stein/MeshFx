using System.Numerics;
using GlyphFX.Engine;

namespace GlyphFX.DemoWeb;

public class DemoApp : AppStateManager
{
    private float speed = 0.001f;
    private float movement = 0f;

    public override void Start()
    {
        var scene = new Scene();
        SimpleLoader.LoadGltf(scene);
        World.LoadScene(scene);
    }

    public override void Update()
    {
        if (Input.IsButtonDown)
        {
            movement += speed * (Input.Button == 0 ? -1 : 1);
        }
        
        movement = 0.01f;

        foreach (var node in World.CurrentScene.Nodes)
        {
            node.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, movement);
            node.Transform.Scale = Vector3.One * 30;
        }
    }
}