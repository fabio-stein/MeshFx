using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

namespace GlyphFX.WebGpu;

public partial class Wgpu
{
    const string Lib = "libwgpu_wrapper";
    
    [DllImport(Lib)]
    public static extern IntPtr init_state(IntPtr displayHandle, IntPtr windowHandle);

    public static void render(IntPtr state, RenderCallback callback)
    {
        callback(IntPtr.Zero);
    }

    public static IntPtr load_texture(IntPtr state, IntPtr data, int size)
    {
        Console.WriteLine(".net load_texture");
        var textureData = new byte[size];
        Marshal.Copy(data, textureData, 0, size);
        RustLoadTexture(textureData);
        return IntPtr.Zero;
    }
    
    [JSImport("globalThis.rust.rust_load_texture")]
    internal static partial void RustLoadTexture(byte[] data);
    
    public static IntPtr load_mesh(IntPtr state, IntPtr vertexData, int vertexCount, IntPtr indexData, int indexCount)
    {
        Console.WriteLine("load_mesh");
        var vertexBytes = new byte[vertexCount * 1];//32 bytes per vertex
        Marshal.Copy(vertexData, vertexBytes, 0, vertexCount * 1);
        var indexBytes = new byte[indexCount * 1];//4 bytes per index
        Marshal.Copy(indexData, indexBytes, 0, indexCount * 1);
        RustLoadMesh(vertexBytes, indexBytes);
        return IntPtr.Zero;
    }
    
    [JSImport("globalThis.rust.rust_load_mesh")]
    internal static partial void RustLoadMesh(byte[] vertexData, byte[] indexData);

    public static void draw(IntPtr state, IntPtr renderPtr, IntPtr cameraUniformBuffer,
        IntPtr instanceSingleMatrixBuffer, IntPtr meshPtr, IntPtr materialPtr)
    {
        Console.WriteLine("draw");
        var size = 4*4*4; //4 bytes each f32 then 4x4 f32
        var cameraUniform = new byte[size];
        Marshal.Copy(cameraUniformBuffer, cameraUniform, 0, size);
        var instanceUniform = new byte[size];
        Marshal.Copy(instanceSingleMatrixBuffer, instanceUniform, 0, size);
        RustDraw(cameraUniform, instanceUniform);
    }
    
    [JSImport("globalThis.rust.rust_draw")]
    internal static partial void RustDraw(byte[] cameraUniform, byte[] instanceUniform);


    public delegate void RenderCallback(IntPtr pointer);
}