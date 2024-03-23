using System.Runtime.InteropServices;
using GlyphFX.Common.Native;

namespace GlyphFX.Desktop;

public class DesktopNativeRequestBridge : AbstractNativeRequestBridge
{
    public DesktopNativeRequestBridge()
    {
        LibNative.set_native_handler(HandleNativeInternal);
    }
    
    LibNative.NativeBuffer HandleNativeInternal(NativeRequestCode code, IntPtr ptr, int size)
    {
        var requestBuffer = new byte[size];
        Marshal.Copy(ptr, requestBuffer, 0, size);
        
        var result = HandleNativeInternalFromBytes(code, requestBuffer);
        //TODO free the ptr in the caller
        var resultPtr = Marshal.AllocHGlobal(result.Length);
        Marshal.Copy(result, 0, resultPtr, result.Length);
        return new LibNative.NativeBuffer()
        {
            Data = resultPtr,
            Size = result.Length
        };
    }
    
    protected override byte[] HandleNative(NativeRequestCode code, byte[] requestBytes)
    {
        if (code.IsInternal())
            return HandleNativeInternalFromBytes(code, requestBytes);
        
        var resultPtr = LibNative.process_message(code, requestBytes, requestBytes.Length);
        var resultBytes = new byte[resultPtr.Size];
        Marshal.Copy(resultPtr.Data, resultBytes, 0, resultPtr.Size);
        Marshal.FreeHGlobal(resultPtr.Data);
        return resultBytes;
    }
}