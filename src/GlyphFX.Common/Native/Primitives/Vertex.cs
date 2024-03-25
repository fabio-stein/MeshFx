using System.Numerics;
using ProtoBuf;

namespace GlyphFX.Common.Native.Primitives;

[ProtoContract]
public struct Vertex(Vector3 position, Vector2 textureCoords, Vector3 normal)
{
    [ProtoMember(1)]
    public Vector3 Position = position;
    [ProtoMember(2)]
    public Vector2 TextureCoords = textureCoords;
    [ProtoMember(3)]
    public Vector3 Normal = normal;
}