using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Threading.Tasks;
using GlyphFX.DemoWeb;
using GlyphFX.Engine;

Console.WriteLine("Hello, Browserxxxxxx!");

MyClass.APP.World.SetCamera(new Camera());
MyClass.APP.Start();
MyClass.APP.Update();

public partial class MyClass
{
    public static DemoApp APP = new DemoApp();
    
    [JSExport]
    internal static void SimpleLog()
    {
        Console.WriteLine("Hello js from .NET");
    }

    [JSImport("globalThis.rust.simple_string")]
    internal static partial String SimpleString();
    
    [JSImport("globalThis.rust.simple_log")]
    internal static partial String RustSimpleLog();

    [JSExport]
    internal static void RenderNet()
    {
        Console.WriteLine("RenderNet");
        APP.Update();
        APP.Render();
        
        
        var vertex = new Vec3(1, 2, 3);
        var vertexBytes = MemoryMarshal.AsBytes(new Span<Vec3>(new Vec3[] { vertex }));
        var data = vertexBytes.ToArray();
        RenderRust(data);
    }
    
    [JSImport("globalThis.rust.renderrust")]
    internal static partial void RenderRust(byte[] data);
}