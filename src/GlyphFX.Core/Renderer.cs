using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Native;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Core;

public class Renderer : IRenderer
{
    private readonly INativeRequestBridge _bridge;
    private bool _isStarting = false;
    private bool _isReady = false;

    public Renderer(INativeRequestBridge bridge)
    {
        _bridge = bridge;
    }
    
    public void Initialize()
    {
        if (_isStarting)
            return;
        
        _isStarting = true;
        _bridge.SetHandler(new SimpleNativeHandler<RendererReadyRequest, RendererReadyResponse>(request => _isReady = true));
        _bridge.Send(new InitRendererRequest());
    }

    public bool IsReady()
    {
        return _isReady;
    }

    bool loadTest = false;

    public void RenderScene(Scene scene, Camera camera)
    {
        if (loadTest)
            return;
        
        var meshPrimitive = scene.Nodes.First().Mesh.Primitives.First();
        var loadMeshRequest = new LoadMeshRequest
        {
            Vertices = meshPrimitive.Vertices,
            Indices = meshPrimitive.Indices
        };
        _bridge.Send(loadMeshRequest);
        
        loadTest = true;
    }
}