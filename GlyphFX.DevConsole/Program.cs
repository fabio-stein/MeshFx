
using GlyphFX.Window;

Winit.run_loop(data =>
{
    Console.WriteLine("KEY EVENTw");
}, data2 =>
{
    Task.Run(() =>
    {
        Console.WriteLine("TASK2 INIT");
        Task.Delay(5000).Wait();
        Console.WriteLine("TASK2 KILL");
        Winit.exit_target(data2);
        Console.WriteLine("TASK2 KILLED");
    });
});

Console.WriteLine("Hello World");

