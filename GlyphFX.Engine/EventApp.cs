using System.Numerics;
using GlyphFX.WebGpu;
using GlyphFX.Window;

namespace GlyphFX.Engine;

public abstract class EventApp
{
    IntPtr winitState = IntPtr.Zero;
    IntPtr windowHandle = IntPtr.Zero;
    IntPtr displayHandle = IntPtr.Zero;
    IntPtr wgpuState = IntPtr.Zero;
    
    InputHandler inputHandler = new();
    SharedBuffer<Vertex> vertexBuffer = new(8);
    SharedBuffer<ushort> indexBuffer = new(36);
    SharedBuffer<Matrix4x4> cameraBuffer = new(1);
    
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
        
        Start();
        Render();
        Winit.request_redraw(winitState);
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
    
    public void SetVertices(Vertex[] vertices)
    {
        vertexBuffer.SetData(vertices);
    }
    
    public void SetIndices(ushort[] indices)
    {
        indexBuffer.SetData(indices);
    }
    
    public void SetCamera(Matrix4x4 camera)
    {
        cameraBuffer.SetData(new Matrix4x4[] { camera });
    }

    public void Render()
    {
        Wgpu.render(wgpuState, vertexBuffer.Buffer, indexBuffer.Buffer, cameraBuffer.Buffer);
    }
    
    public abstract void Start();
    public abstract void Update();
}