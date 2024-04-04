using System.Numerics;
using GlyphFX.Common.Native.Primitives;
using SharpGLTF.Schema2;
using Material = GlyphFX.Common.Scenes.Material;
using Mesh = GlyphFX.Common.Scenes.Mesh;
using MeshPrimitive = GlyphFX.Common.Scenes.MeshPrimitive;
using Node = GlyphFX.Common.Scenes.Node;
using Scene = GlyphFX.Common.Scenes.Scene;

namespace GlyphFX.Core;

public class GltfLoader
{
    public static Scene Load(Stream stream)
    {
        var model = ModelRoot.ReadGLB(stream);
        var scene = new Scene(model.LogicalNodes.Select(ParseNode).ToList());
        return scene;
    }

    private static Node ParseNode(SharpGLTF.Schema2.Node glNode)
    {
        Mesh? mesh = null;
        try
        {
            mesh = (glNode.Mesh == null) ? null : new Mesh(glNode.Mesh.Primitives.Select(ParseMeshPrimitive).ToArray());
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to parse node");
        }
        Node.NodeTransform? transform = null;//TODO UPDATE
        var node = new Node(mesh, glNode.LocalMatrix, transform);
        return node;
    }

    private static MeshPrimitive ParseMeshPrimitive(SharpGLTF.Schema2.MeshPrimitive glMeshPrimitive)
    {
        var position = glMeshPrimitive.GetVertexAccessor("POSITION").AsVector3Array().ToList();
        var normal = glMeshPrimitive.GetVertexAccessor("NORMAL").AsVector3Array().ToList();
        var texCoord = glMeshPrimitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array().ToList();

        var vertices = new List<Vertex>();
        var indices = new List<UInt32>();
        for (int i = 0; i < position.Count; i++)
        {
            var posMap = new Vec3(position[i].X, position[i].Y, position[i].Z);
            var texMap = new Vec2(texCoord[i].X, texCoord[i].Y);
            var normMap = new Vec3(normal[i].X, normal[i].Y, normal[i].Z);
            vertices.Add(new Vertex(posMap, texMap, normMap));
        }

        indices.AddRange(glMeshPrimitive.IndexAccessor.AsIndicesArray());

        var textureData = glMeshPrimitive.Material.FindChannel("BaseColor").Value.Texture.PrimaryImage.Content
            .Content.ToArray();

        var material = new Material(textureData);

        return new MeshPrimitive(vertices.ToArray(), indices.ToArray(), material);
    }
}