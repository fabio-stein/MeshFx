using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    private Action? _currentDrawAction = null;

    public Renderer(INativeRequestBridge bridge)
    {
        _bridge = bridge;
        _bridge.SetHandler(new SimpleNativeHandler<RenderWaitingRequest, RenderWaitingResponse>(OnWaiting));
    }

    private void OnWaiting(RenderWaitingRequest request)
    {
        if (_currentDrawAction != null)
        {
            _currentDrawAction();
            _currentDrawAction = null;
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
        
        rotation += 0.01f;

        _currentDrawAction = () =>
        {
            var cameraArray = new float[16];
            for (var i = 0; i < 16; i++)
                cameraArray[i] = camera.ViewProjection[i / 4, i % 4];

            var instance1 = new InstanceRaw(Matrix4x4.CreateRotationY(rotation) * Matrix4x4.CreateScale(20f), new Matrix3x3(Matrix4x4.CreateRotationY(rotation)));
            var instance2PosX = (float)Math.Sin(rotation) * 2 + 1;
            var instance2 = new InstanceRaw(Matrix4x4.CreateScale(20f) * Matrix4x4.CreateTranslation(Vector3.UnitY + (Vector3.UnitX*-1) + Vector3.UnitX * instance2PosX), new Matrix3x3(Matrix4x4.Identity));
            var instanceArray = new[]{instance1, instance2};
            var byteArray = MemoryMarshal.Cast<InstanceRaw, byte>(instanceArray.AsSpan()).ToArray();
            
            _bridge.Send(new RenderDrawRequest()
            {
                CameraViewProjection = cameraArray,
                InstanceMatrix = byteArray,
                InstanceCount = (uint)instanceArray.Length
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

public struct InstanceRaw(Matrix4x4 model, Matrix3x3 normal)
{
    public Matrix4x4 Model = model;
    public Matrix3x3 Normal = normal;
}

public struct Matrix3x3(Matrix4x4 matrix)
{
    public float M11 = matrix.M11;
    public float M12 = matrix.M12;
    public float M13 = matrix.M13;
    public float M21 = matrix.M21;
    public float M22 = matrix.M22;
    public float M23 = matrix.M23;
    public float M31 = matrix.M31;
    public float M32 = matrix.M32;
    public float M33 = matrix.M33;
}