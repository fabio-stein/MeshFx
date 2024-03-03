
using GlyphFX.WebGpu;
using GlyphFX.Window;

var winitState = IntPtr.Zero;
var windowHandle = IntPtr.Zero;
var displayHandle = IntPtr.Zero;
var wgpuState = IntPtr.Zero;

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
}, data =>
{
    Console.WriteLine("MOUSE INPUT "+data.IsDown+" "+data.Button);
    Wgpu.render(wgpuState);
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