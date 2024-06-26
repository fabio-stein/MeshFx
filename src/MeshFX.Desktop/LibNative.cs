using System.Runtime.InteropServices;
using MeshFX.Common.Native;

namespace MeshFX.Desktop;

public class LibNative
{
    private const string LibName = "lib/libmeshfx_native";
    
    [DllImport(LibName)]
    public static extern void init_desktop();
    
    [DllImport(LibName)]
    public static extern NativeBuffer process_message(NativeRequestCode code, byte[] data, int size);

    [DllImport(LibName)]
    public static extern void set_native_handler(HandleNativeDelegate handler);
    
    public delegate NativeBuffer HandleNativeDelegate(NativeRequestCode code, IntPtr ptr, int size);
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeBuffer(IntPtr data, int size)
    {
        public IntPtr Data = data;
        public int Size = size;
    }
}