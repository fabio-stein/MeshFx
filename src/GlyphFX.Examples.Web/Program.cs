using System;
using System.Runtime.InteropServices.JavaScript;
using GlyphFX.Core;
using GlyphFX.Examples;
using GlyphFX.Examples.Web;

Console.WriteLine("Hello, Browser!");

NativeNetJs._bridge = new WebNativeRequestBridge();

NativeNetJs.RunExample();

public partial class NativeNetJs
{
    public static WebNativeRequestBridge _bridge;
    
    [JSExport]
    public static void RunExample()
    {
        new ExampleScene(_bridge)
            .Run();
    }

    [JSExport]
    public static byte[] HandleNative(int code, byte[] data)
    {
        return _bridge.HandleInternal(code, data);
    }
    
    [JSImport("globalThis.rust.process_web_message")]
    internal static partial byte[] ProcessWebMessage(int code, byte[] data);
}