
using GlyphFX.Window;

var state = IntPtr.Zero;

Winit.run_loop(data =>
{
    //Console.WriteLine("KEY EVENT "+data.KeyCode);
}, data2 =>
{
    state = data2;
    Console.WriteLine("INIT STATE "+data2);
},
data =>
{
    //Console.WriteLine( "CURSOR MOVED "+data.X+" "+data.Y);
}, data =>
{
    Console.WriteLine("MOUSE INPUT "+data.IsDown+" "+data.Button);
}, () =>
{
    Console.WriteLine("REDRAW REQUESTED");
}, () =>
{
    Console.WriteLine("CLOSE REQUESTED");
    Winit.exit_target(state);
    Winit.request_redraw(state);
});

Console.WriteLine("Hello World");