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
    SharedBuffer<Vertex> vertexBuffer = new(3);
    SharedBuffer<UInt32> indexBuffer = new(3);
    SharedBuffer<Matrix4x4> cameraBuffer = new(1);
    SharedBuffer<Matrix4x4> instanceMatrixBuffer = new(2);
    
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
        Wgpu.load_texture(wgpuState, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0), data.Length);
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
    
    public void SetInstanceMatrix(Matrix4x4[] instances)
    {
        instanceMatrixBuffer.SetData(instances);
    }

    public void Render()
    {
        cameraBuffer.SetData([World.CurrentCamera.ViewProjection]);
        var mesh = World.CurrentScene.Nodes.First().Mesh.Primitives.First();
        vertexBuffer.SetData(mesh.Vertices);
        indexBuffer.SetData(mesh.Indices);
        Wgpu.render(wgpuState, vertexBuffer.Pointer, indexBuffer.Pointer, cameraBuffer.Pointer, instanceMatrixBuffer.Pointer);
    }
    
    public abstract void Start();
    public abstract void Update();
}