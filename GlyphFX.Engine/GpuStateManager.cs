using System.Numerics;
using System.Runtime.InteropServices;
using GlyphFX.WebGpu;

namespace GlyphFX.Engine;

public class GpuStateManager
{
    private IntPtr _wgpuState = IntPtr.Zero;
    private Dictionary<MeshPrimitive, IntPtr> _meshPtrs = new();
    private Dictionary<Material, IntPtr> _materialPtrs = new();

    public void Initialize(IntPtr windowHandle, IntPtr displayHandle)
    {
        _wgpuState = Wgpu.init_state(displayHandle, windowHandle);
    }
    
    private IntPtr GetOrLoadMeshPtr(MeshPrimitive mesh)
    {
        if (!_meshPtrs.ContainsKey(mesh))
            LoadMesh(mesh);
        return _meshPtrs[mesh];
    }
    
    private void LoadMesh(MeshPrimitive mesh)
    {
        var vertexBuffer = new SharedBuffer<Vertex>(mesh.Vertices);
        var indexBuffer = new SharedBuffer<uint>(mesh.Indices);
        var pointer = Wgpu.load_mesh(_wgpuState, vertexBuffer.Pointer, vertexBuffer.Count, indexBuffer.Pointer, indexBuffer.Count);
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        _meshPtrs.Add(mesh, pointer);
    }
    
    private IntPtr GetOrLoadMaterialPtr(Material material)
    {
        if (!_materialPtrs.ContainsKey(material))
            LoadMaterial(material);
        return _materialPtrs[material];
    }
    
    private void LoadMaterial(Material material)
    {
        var pointer = Wgpu.load_texture(_wgpuState, Marshal.UnsafeAddrOfPinnedArrayElement(material.TextureData, 0), material.TextureData.Length);
        _materialPtrs.Add(material, pointer);
    }
    
    public void Render(Matrix4x4 cameraViewProjection, MeshRenderCommand[] renderCommands)
    {
        var cameraBuffer = new SharedBuffer<Matrix4x4>([cameraViewProjection]);
        Wgpu.render(_wgpuState, (ptr) =>
        {
            foreach (var renderCommand in renderCommands)
            {
                var instanceMatrixBuffer = new SharedBuffer<Matrix4x4>(renderCommand.InstanceMatrix);
                var meshPtr = GetOrLoadMeshPtr(renderCommand.Mesh);
                var materialPtr = GetOrLoadMaterialPtr(renderCommand.Mesh.Material);
                Wgpu.draw(_wgpuState, ptr, cameraBuffer.Pointer, instanceMatrixBuffer.Pointer, meshPtr, materialPtr);
            }
        });
    }
}

public class MeshRenderCommand(MeshPrimitive mesh, Matrix4x4[] instanceMatrix)
{
    public MeshPrimitive Mesh = mesh;
    public Matrix4x4[] InstanceMatrix = instanceMatrix;
}