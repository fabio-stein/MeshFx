using System.Numerics;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace MeshFX.Common.Native.Primitives;

[ProtoContract]
[StructLayout(LayoutKind.Sequential)]
public struct Vertex(Vec3 position, Vec2 textureCoords, Vec3 normal)
{
    [ProtoMember(1)]
    public Vec3 Position = position;
    [ProtoMember(2)]
    public Vec2 TextureCoords = textureCoords;
    [ProtoMember(3)]
    public Vec3 Normal = normal;
}