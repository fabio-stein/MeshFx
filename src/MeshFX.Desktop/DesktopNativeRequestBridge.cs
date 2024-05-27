using System.Runtime.InteropServices;
using MeshFX.Common.Native;

namespace MeshFX.Desktop;

public class DesktopNativeRequestBridge : AbstractNativeRequestBridge
{
    //Keep reference to the delegate to prevent it from being garbage collected
    LibNative.HandleNativeDelegate _handleNativeInternal;
    
    public DesktopNativeRequestBridge()
    {
        LibNative.init_desktop();
        LibNative.set_native_handler(_handleNativeInternal = HandleNativeInternal);
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
        if(resultBytes.Length > 0)
            Marshal.FreeHGlobal(resultPtr.Data);
        return resultBytes;
    }
}