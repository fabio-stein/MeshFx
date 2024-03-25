using System.Runtime.CompilerServices;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Native;
using GlyphFX.Common.Native.Primitives;
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
        LoadMesh(scene);
        LoadMaterial(scene);
        _bridge.Send(new BeginRenderRequest());
        loadTest = true;
    }

    private void LoadMaterial(Scene scene)
    {
        var meshPrimitive = scene.Nodes.First().Mesh.Primitives.First();
        var bytes = meshPrimitive.Material.TextureData;
        _bridge.Send(new LoadMaterialRequest()
        {
            TextureData = bytes
        });
    }

    private void LoadMesh(Scene scene)
    {
        var meshPrimitive = scene.Nodes.First().Mesh.Primitives.First();
        var vertices = meshPrimitive.Vertices;
        var size = Unsafe.SizeOf<Vertex>();
        var bytes = new byte[size * vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            var offset = i * size;
            Unsafe.WriteUnaligned(ref bytes[offset], vertex);
        }
        var loadMeshRequest = new LoadMeshRequest
        {
            Vertices = bytes,
            Indices = meshPrimitive.Indices
        };
        _bridge.Send(loadMeshRequest);
    }
}