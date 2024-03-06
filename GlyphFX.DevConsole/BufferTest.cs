using System.Runtime.InteropServices;
using GlyphFX.WebGpu;

namespace GlyphFX.DevConsole;

public class BufferTest
{
    public static IntPtr buffer = Marshal.AllocHGlobal(10000);
    public static IntPtr VertexBuffer(Vec2 left, Vec2 right)
    {
        Vertex[] vertices =
        [
            new Vertex { position = new Vec3 { x = left.x, y = left.y, z = 0.0f }, color = new ColorRGB { r = 0.0f, g = 1.0f, b = 0.0f } },
            new Vertex { position = new Vec3 { x = left.x, y = right.y, z = 0.0f }, color = new ColorRGB { r = 1.0f, g = 0.0f, b = 1.0f } },
            new Vertex { position = new Vec3 { x = right.x, y = right.y, z = 0.0f }, color = new ColorRGB { r = 0.0f, g = 0.0f, b = 1.0f } },
            new Vertex { position = new Vec3 { x = right.x, y = left.y, z = 0.0f }, color = new ColorRGB { r = 1.0f, g = 1.0f, b = 0.0f } }

        ];

        for (int i = 0; i < vertices.Length; i++)
        {
            IntPtr vertexPtr = buffer + i * Marshal.SizeOf(typeof(Vertex));
            Marshal.StructureToPtr(vertices[i], vertexPtr, false);
        }
        
        //Marshal.FreeHGlobal(ptr);
        return buffer;
    }
    
    public static IntPtr IndexBuffer()
    {
        var indices = new ushort[] { 0, 1, 2, 0, 2, 3};
        IntPtr ptr = Marshal.AllocHGlobal(1000);
        for (int i = 0; i < indices.Length; i++)
        {
            IntPtr indexPtr = ptr + i * Marshal.SizeOf(typeof(ushort));
            Marshal.StructureToPtr(indices[i], indexPtr, false);
        }
        //Marshal.FreeHGlobal(ptr);
        return ptr;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Vertex {
    public Vec3 position;
    public ColorRGB color;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vec3 {
    public float x;
    public float y;
    public float z;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vec2(float x, float y)
{
    public float x = x;
    public float y = y;
}

[StructLayout(LayoutKind.Sequential)]
public struct ColorRGB {
    public float r;
    public float g;
    public float b;
}

