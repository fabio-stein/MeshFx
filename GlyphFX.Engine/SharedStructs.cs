using System.Runtime.InteropServices;

namespace GlyphFX.Engine;


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
