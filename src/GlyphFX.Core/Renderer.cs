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

    float rotation = 0f;
    public void RenderScene(Scene scene, Camera camera)
    {
        if(!_isReady)
            return;

        foreach (var meshPrimitive in GetAllMeshPrimitives(scene))
        {
            var meshId = GetOrLoadMeshId(meshPrimitive);
            var materialId = GetOrLoadMaterialId(meshPrimitive.Material);
        }
        
        rotation += 0.01f;
        
        var instancesList = new List<InstanceRaw>();
        var instanceCountOffset = 0;

        var renderItems = GetRenderItems(scene).GroupBy(r => r.MeshPrimitive).Select(r => new
        {
            MeshPrimitive = r.Key,
            Instances = r.ToList()
        }).ToList();
        
        foreach (var item in renderItems)
        {
            var instanceArray = item.Instances.Select(r =>
            {
                var outputMatrix = r.FinalMatrix;
                var rotationMatrix = new Matrix3x3(Matrix4x4.CreateFromQuaternion(r.Transform.Rotation));
                return new InstanceRaw(outputMatrix, rotationMatrix);
            }).ToArray();
            instancesList.AddRange(instanceArray);
        }
        
        _currentDrawAction = () =>
        {
            foreach (var item in renderItems)
            {
                var meshPrimitive = item.MeshPrimitive;
                var instanceArray = item.Instances.Select(r =>
                {
                    var outputMatrix = r.FinalMatrix;
                    var rotationMatrix = new Matrix3x3(Matrix4x4.CreateFromQuaternion(r.Transform.Rotation));
                    return new InstanceRaw(outputMatrix, rotationMatrix);
                }).ToArray();
                
                var meshId = GetOrLoadMeshId(meshPrimitive);
                var materialId = GetOrLoadMaterialId(meshPrimitive.Material);
                _bridge.Send(new RenderDrawRequest()
                {
                    InstanceCount = (uint)instanceArray.Length,
                    MeshId = meshId,
                    MaterialId = materialId,
                    InstanceItemOffset = (uint)instanceCountOffset
                });

                instanceCountOffset += instanceArray.Length;
            }
        };
        
        var cameraBuffer = new CameraBuffer(camera.ViewProjection, camera.Eye);
        var lightBuffer = new LightBuffer(camera.Eye, Vector3.One);//Light is following eye position
        _bridge.Send(new BeginRenderRequest()
        {
            InstanceBuffer = MemoryMarshal.Cast<InstanceRaw, byte>(instancesList.ToArray()).ToArray(),
            CameraBuffer = MemoryMarshal.Cast<CameraBuffer, byte>([cameraBuffer]).ToArray(),
            LightBuffer = MemoryMarshal.Cast<LightBuffer, byte>([lightBuffer]).ToArray(),
        });
    }

    private HashSet<uint> _loadedMaterialIds = new();
    private uint GetOrLoadMaterialId(Material material)
    {
        var currentId = (uint)material.GetHashCode();//TODO should not use hashCode to avoid collision
        if (_loadedMaterialIds.Contains(currentId))
            return currentId;
        
        var bytes = material.TextureData;
        _bridge.Send(new LoadMaterialRequest()
        {
            TextureData = bytes,
            MaterialId = currentId
        });
        _loadedMaterialIds.Add(currentId);
        return currentId;
    }
    
    private HashSet<uint> _loadedMeshIds = new();
    private uint GetOrLoadMeshId(MeshPrimitive mesh)
    {
        var currentId = (uint)mesh.GetHashCode();//TODO should not use hashCode to avoid collision
        if (_loadedMeshIds.Contains(currentId))
            return currentId;
        LoadMesh(mesh, currentId);
        _loadedMeshIds.Add(currentId);
        return currentId;
    }

    private void LoadMesh(MeshPrimitive meshPrimitive, uint id)
    {
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
            Indices = meshPrimitive.Indices,
            MeshId = id
        };
        _bridge.Send(loadMeshRequest);
    }

    private List<MeshPrimitive> GetAllMeshPrimitives(Scene scene)
    {
        var list = new List<MeshPrimitive>();
        foreach (var sceneNode in scene.Nodes)
        {
            GetAllMeshPrimitives(sceneNode, ref list);
        }

        return list;
    }
    
    private void GetAllMeshPrimitives(Node node, ref List<MeshPrimitive> outputList)
    {
        if(node.Mesh != null)
            outputList.AddRange(node.Mesh.Primitives);
        foreach (var nodeChild in node.Children)
        {
            GetAllMeshPrimitives(nodeChild, ref outputList);
        }
    }
    
    private List<RenderDrawItem> GetRenderItems(Scene scene)
    {
        var list = new List<RenderDrawItem>();
        foreach (var sceneNode in scene.Nodes)
            GetRenderItems(sceneNode, ref list, Matrix4x4.Identity);

        return list;
    }
    
    private void GetRenderItems(Node node, ref List<RenderDrawItem> outputList, Matrix4x4 parentMatrix)
    {
        var finalTransform = node.WorldTransform;
        var finalMatrix = node.WorldTransform.GetTransformationMatrix();
        if (node.Mesh != null)
        {
            foreach (var mesh in node.Mesh.Primitives)
                outputList.Add(new RenderDrawItem()
                {
                    Material = mesh.Material,
                    Transform = finalTransform,
                    MeshPrimitive = mesh,
                    FinalMatrix = finalMatrix
                });
        }

        foreach (var nodeChild in node.Children)
            GetRenderItems(nodeChild, ref outputList, finalMatrix);
    }
}

public struct InstanceRaw(Matrix4x4 model, Matrix3x3 normal)
{
    public Matrix4x4 Model = model;
    public Matrix3x3 Normal = normal;
}

//TODO move to a better place, should not be here
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

class RenderDrawItem
{
    public MeshPrimitive MeshPrimitive { get; set; }
    public Material Material { get; set; }
    public Node.NodeTransform Transform { get; set; }
    public Matrix4x4 FinalMatrix { get; set; }
}