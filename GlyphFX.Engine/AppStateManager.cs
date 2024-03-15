using System.Numerics;
using System.Runtime.InteropServices;
using GlyphFX.WebGpu;
using GlyphFX.Window;

namespace GlyphFX.Engine;

public abstract class AppStateManager
{
    public WorldManager World = new();
    
    IntPtr winitState = IntPtr.Zero;
    IntPtr windowHandle = IntPtr.Zero;
    IntPtr displayHandle = IntPtr.Zero;
    IntPtr wgpuState = IntPtr.Zero;
    
    InputHandler inputHandler = new();
    SharedBuffer<Matrix4x4> cameraBuffer = new(1);
    SharedBuffer<Matrix4x4> instanceMatrixBuffer = new(2);
    
    IntPtr MeshPtr = IntPtr.Zero;
    IntPtr MaterialPtr = IntPtr.Zero;
    
    public InputStatus Input => inputHandler.InputStatus;
    
    internal void StartInternal()
    {
        Winit.run_loop(inputHandler.HandleKey, Initialize, (d) =>
        {
            inputHandler.CursorMoved(d);
            Update();
        }, inputHandler.MouseInput, RedrawRequested, CloseRequested);
    }

    private void Initialize(IntPtr stateData)
    {
        if (winitState != IntPtr.Zero)
            return;
        
        winitState = stateData;
        windowHandle = Winit.get_window_handle(winitState);
        displayHandle = Winit.get_display_handle(winitState);
        wgpuState = Wgpu.init_state(displayHandle, windowHandle);
        
        Console.WriteLine("EventApp initialized");
        
        World.SetCamera(new Camera());
        
        Start();
        Render();
        
        Winit.request_redraw(winitState);
    }
    
    public void LoadTexture(byte[] data)
    {
        MaterialPtr = Wgpu.load_texture(wgpuState, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), data.Length);
    }
    
    private void RedrawRequested()
    {
        if(winitState == IntPtr.Zero)
            return;
        
        Winit.request_redraw(winitState);
        Update();
        Render();
    }
    
    private void CloseRequested()
    {
    }
    
    public void Render()
    {
        cameraBuffer.SetData([World.CurrentCamera.ViewProjection]);

        if (MeshPtr == IntPtr.Zero)
            LoadMesh();
        
        var matrix = World.CurrentScene.Nodes.First().LocalMatrix;
        instanceMatrixBuffer.SetData([Matrix4x4.Identity, matrix]);
        
        Wgpu.render(wgpuState, cameraBuffer.Pointer, instanceMatrixBuffer.Pointer, MeshPtr, MaterialPtr);
    }

    private void LoadMesh()
    {
        var mesh = World.CurrentScene.Nodes.First().Mesh.Primitives.First();
        var vertexBuffer = new SharedBuffer<Vertex>(mesh.Vertices);
        var indexBuffer = new SharedBuffer<uint>(mesh.Indices);
        MeshPtr = Wgpu.load_mesh(wgpuState, vertexBuffer.Pointer, vertexBuffer.Count, indexBuffer.Pointer, indexBuffer.Count);
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
    }
    
    public abstract void Start();
    public abstract void Update();
}