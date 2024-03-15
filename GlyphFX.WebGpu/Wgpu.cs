using System.Runtime.InteropServices;

namespace GlyphFX.WebGpu;

public class Wgpu
{
    const string Lib = "libwgpu_wrapper";
    
    [DllImport(Lib)]
    public static extern IntPtr init_state(IntPtr displayHandle, IntPtr windowHandle);
    
    [DllImport(Lib)]
    public static extern void render(IntPtr state, IntPtr vertexBuffer, IntPtr indexBuffer, IntPtr cameraUniformBuffer, IntPtr instanceSingleMatrixBuffer, IntPtr materialPtr);
    
    [DllImport(Lib)]
    public static extern IntPtr load_texture(IntPtr state, IntPtr data, int size);
}