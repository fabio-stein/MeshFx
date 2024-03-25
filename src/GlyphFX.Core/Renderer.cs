using System.Numerics;
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
    private Action? CurrentDrawAction = null;

    public Renderer(INativeRequestBridge bridge)
    {
        _bridge = bridge;
        _bridge.SetHandler(new SimpleNativeHandler<RenderWaitingRequest, RenderWaitingResponse>(OnWaiting));
    }

    private void OnWaiting(RenderWaitingRequest request)
    {
        if (CurrentDrawAction != null)
        {
            CurrentDrawAction();
            CurrentDrawAction = null;
        }
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

    float rotation = 0f;
    public void RenderScene(Scene scene, Camera camera)
    {
        if(!_isReady)
            return;
        if (!loadTest)
        {
            LoadMesh(scene);
            LoadMaterial(scene);
            loadTest = true;
        }

        CurrentDrawAction = () =>
        {
            var cameraArray = new float[16];
            for (var i = 0; i < 16; i++)
                cameraArray[i] = camera.ViewProjection[i / 4, i % 4];

            rotation += 0.01f;
            var rotationMatrix = Matrix4x4.CreateRotationY(this.rotation);
            var instanceMatrix = Matrix4x4.Identity * Matrix4x4.CreateScale(30) * rotationMatrix;
            var instanceMatrixArray = new float[16];
            for (var i = 0; i < 16; i++)
                instanceMatrixArray[i] = instanceMatrix[i / 4, i % 4];
            _bridge.Send(new RenderDrawRequest()
            {
                CameraViewProjection = cameraArray,
                InstanceMatrix = instanceMatrixArray
            });
        };
        _bridge.Send(new BeginRenderRequest());
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