using System.Runtime.InteropServices;
using ProtoBuf;

namespace MeshFX.Common.Native.Primitives;

[ProtoContract]
[StructLayout(LayoutKind.Sequential)]
public struct Vec3(float X, float Y, float Z)
{
    [ProtoMember(1)]
    public float X = X;
    [ProtoMember(2)]
    public float Y = Y;
    [ProtoMember(3)]
    public float Z = Z;
}

[ProtoContract]
[StructLayout(LayoutKind.Sequential)]
public struct Vec2(float X, float Y)
{
    [ProtoMember(1)]
    public float X = X;
    [ProtoMember(2)]
    public float Y = Y;
}