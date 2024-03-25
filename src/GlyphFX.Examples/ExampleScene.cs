using System.Reflection;
using GlyphFX.Common.Native;
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
        
        var windowManager = new WindowManager(_bridge);
        
        new GlyphAppBuilder()
            .WithWindowManager(windowManager)
            .WithWindowEventHandler(new WindowEventHandler())
            .WithRenderer(new Renderer(_bridge))
            .WithScene(scene)
            .Build()
            .Run();
    }
}