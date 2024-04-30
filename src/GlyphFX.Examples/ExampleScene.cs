using System.Numerics;
using System.Reflection;
using GlyphFX.Common.Cameras;
using GlyphFX.Common.Input;
using GlyphFX.Common.Native;
using GlyphFX.Common.Scenes;
using GlyphFX.Core;

namespace GlyphFX.Examples;

public class ExampleScene
{
    private readonly INativeRequestBridge _bridge;

    public ExampleScene(INativeRequestBridge bridge)
    {
        _bridge = bridge;
    }

    public void Run()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GlyphFX.Examples.model.glb");
        var scene = GltfLoader.Load(stream);

        var inputManager = new InputManager(_bridge);
        
        new GlyphAppBuilder()
            .WithWindowManager(new WindowManager(_bridge))
            .WithInputManager(inputManager)
            .WithWindowEventHandler(new WindowEventHandler())
            .WithRenderer(new Renderer(_bridge))
            .WithScene(scene)
            .WithCameraController(new OrbitCameraController())
            .WithUpdateHandler(() => { })
            .Build()
            .Run();
    }
}