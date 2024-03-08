using System.Runtime.InteropServices;

namespace GlyphFX.Engine;


[StructLayout(LayoutKind.Sequential)]
public struct Vertex {
    public Vec3 position;
    public Vec2 textureCoords;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vec3(float x, float y, int z)
{
    public float x = x;
    public float y = y;
    public float z = z;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vec2(float x, float y)
{
    public float x = x;
    public float y = y;
}
