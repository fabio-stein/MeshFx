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
        var renderCommands = GetMeshesToRender(World.CurrentScene);
        Gpu.Render(cameraProjection, renderCommands.ToArray());
    }
    
    private List<MeshRenderCommand> GetMeshesToRender(Scene scene)
    {
        var renderCommands = new List<MeshRenderCommand>();
        foreach (var node in scene.Nodes)
        {
            GetMeshesToRender(node, renderCommands);
        }
        return renderCommands;
    }
    
    private void GetMeshesToRender(Node node, List<MeshRenderCommand> renderCommands)
    {
        if (node.Mesh != null)
        {
            renderCommands.Add(new MeshRenderCommand(node.Mesh.Primitives.First(), [node.LocalMatrix]));
        }
        
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                GetMeshesToRender(child, renderCommands);
            }
        }
    }
    
    public abstract void Start();
    public abstract void Update();
}