using System.Runtime.InteropServices;

namespace GlyphFX.Engine;


[StructLayout(LayoutKind.Sequential)]
public struct Vertex(Vec3 position, Vec2 textureCoords, Vec3 normal)
{
    public Vec3 position = position;
    public Vec2 textureCoords = textureCoords;
    public Vec3 normal = normal;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vec3(float x, float y, float z)
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
