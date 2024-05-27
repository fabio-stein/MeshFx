using System;
using MeshFX.Common.Native;

namespace MeshFX.Examples.Web;

public class WebNativeRequestBridge : AbstractNativeRequestBridge
{
    protected override byte[] HandleNative(NativeRequestCode code, byte[] requestBytes)
    {
        return NativeNetJs.ProcessWebMessage((int)code, requestBytes);
    }
    
    public byte[] HandleInternal(int code, byte[] data)
    {
        return HandleNativeInternalFromBytes((NativeRequestCode)code, data);
    }
}