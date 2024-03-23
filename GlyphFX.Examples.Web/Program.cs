using System;
using System.Runtime.InteropServices.JavaScript;
using GlyphFX.Examples;
using GlyphFX.Examples.Web;

Console.WriteLine("Hello, Browser!");

NativeNetJs._bridge = new WebNativeRequestBridge();
NativeNetJs._handler = new ExampleHandlers(NativeNetJs._bridge);

public partial class NativeNetJs
{
    public static WebNativeRequestBridge _bridge;
    public static ExampleHandlers _handler;
    
    [JSExport]
    public static void Hello()
    {
        _handler.RunExample();
    }

    [JSExport]
    public static byte[] HandleNative(int code, byte[] data)
    {
        return _bridge.HandleInternal(code, data);
    }
    
    [JSImport("globalThis.rust.process_web_message")]
    internal static partial byte[] ProcessWebMessage(int code, byte[] data);
}