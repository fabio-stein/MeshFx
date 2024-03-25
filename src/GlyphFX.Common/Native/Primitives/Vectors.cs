using ProtoBuf;

namespace GlyphFX.Common.Native.Primitives;

[ProtoContract]
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
public struct Vec2(float X, float Y)
{
    [ProtoMember(1)]
    public float X = X;
    [ProtoMember(2)]
    public float Y = Y;
}