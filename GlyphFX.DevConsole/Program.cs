
using GlyphFX.DevConsole;
using GlyphFX.WebGpu;
using GlyphFX.Window;

var winitState = IntPtr.Zero;
var windowHandle = IntPtr.Zero;
var displayHandle = IntPtr.Zero;
var wgpuState = IntPtr.Zero;

Vec2? left = null;
Vec2? right = null;

bool isButtonDown = false;

Winit.run_loop(data =>
{
    //Console.WriteLine("KEY EVENT "+data.KeyCode);
}, data2 =>
{
    winitState = data2;
    windowHandle = Winit.get_window_handle(winitState);
    displayHandle = Winit.get_display_handle(winitState);
    wgpuState = Wgpu.init_state(displayHandle, windowHandle);
    Console.WriteLine("INIT STATE "+data2);
},
data =>
{
    //Console.WriteLine( "CURSOR MOVED "+data.X+" "+data.Y);
    if (isButtonDown)
    {
        if (left == null)
            left = new Vec2(data.X, data.Y);
        right = new Vec2(data.X, data.Y);
        
        var vertexBuffer = BufferTest.VertexBuffer((Vec2)left, (Vec2)right);
        var indexBuffer = BufferTest.IndexBuffer();
        Wgpu.render(wgpuState, vertexBuffer, indexBuffer);
    }
    
}, data =>
{
    Console.WriteLine("MOUSE INPUT "+data.IsDown+" "+data.Button);
    isButtonDown = data.IsDown == 1;
    if (isButtonDown)
        left = right = null;
}, () =>
{
    Console.WriteLine("REDRAW REQUESTED");
}, () =>
{
    Console.WriteLine("CLOSE REQUESTED");
    Winit.exit_target(winitState);
    Winit.request_redraw(winitState);
});

Console.WriteLine("Hello World");