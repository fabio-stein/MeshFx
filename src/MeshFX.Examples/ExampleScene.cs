using System.Numerics;
using System.Reflection;
using MeshFX.Common.Cameras;
using MeshFX.Common.Input;
using MeshFX.Common.Native;
using MeshFX.Common.Scenes;
using MeshFX.Core;

namespace MeshFX.Examples;

public class ExampleScene
{
    private readonly INativeRequestBridge _bridge;

    public ExampleScene(INativeRequestBridge bridge)
    {
        _bridge = bridge;
    }

    public void Run()
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MeshFX.Examples.model.glb");
        var scene = GltfLoader.Load(stream);

        var inputManager = new InputManager(_bridge);
        
        new MeshAppBuilder()
            .WithWindowManager(new WindowManager(_bridge))
            .WithInputManager(inputManager)
            .WithRenderer(new Renderer(_bridge))
            .WithScene(scene)
            .WithCameraController(new OrbitCameraController())
            .WithUpdateHandler(() => { })
            .Build()
            .Run();
    }
}