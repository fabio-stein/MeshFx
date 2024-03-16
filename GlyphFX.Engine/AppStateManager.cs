using System.Numerics;
using System.Runtime.InteropServices;
using GlyphFX.WebGpu;
using GlyphFX.Window;

namespace GlyphFX.Engine;

public abstract class AppStateManager
{
    public WorldManager World = new();
    private GpuStateManager Gpu = new();
    
    IntPtr winitState = IntPtr.Zero;
    IntPtr windowHandle = IntPtr.Zero;
    IntPtr displayHandle = IntPtr.Zero;
    
    InputHandler inputHandler = new();
    SharedBuffer<Matrix4x4> cameraBuffer = new(1);
    SharedBuffer<Matrix4x4> instanceMatrixBuffer = new(2);

    public Material? DefaultMaterial = null;
    
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
        Gpu.Initialize(windowHandle, displayHandle);
        
        Console.WriteLine("EventApp initialized");
        
        World.SetCamera(new Camera());
        
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
    
    public void Render()
    {
        var cameraProjection = World.CurrentCamera.ViewProjection;
        var matrix = World.CurrentScene.Nodes.First().LocalMatrix;
        var mesh = World.CurrentScene.Nodes.First().Mesh.Primitives.First();
        
        Gpu.Render(cameraProjection, [matrix], mesh, DefaultMaterial);
    }
    
    public abstract void Start();
    public abstract void Update();
}