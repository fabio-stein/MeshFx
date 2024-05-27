using System.Numerics;
using MeshFX.Common.Native.Primitives;
using SharpGLTF.Schema2;
using Material = MeshFX.Common.Scenes.Material;
using Mesh = MeshFX.Common.Scenes.Mesh;
using MeshPrimitive = MeshFX.Common.Scenes.MeshPrimitive;
using Node = MeshFX.Common.Scenes.Node;
using Scene = MeshFX.Common.Scenes.Scene;

namespace MeshFX.Core;

public class GltfLoader
{
    public static Scene Load(Stream stream)
    {
        var model = ModelRoot.ReadGLB(stream);
        var scene = new Scene(model.DefaultScene.VisualChildren.Select(ParseNode).ToList());
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
        var transform = new Node.NodeTransform(glNode.LocalTransform.Translation, glNode.LocalTransform.Rotation, glNode.LocalTransform.Scale);
        var node = new Node(mesh, glNode.LocalMatrix, transform);
        foreach(var glChildNode in glNode.VisualChildren)
            ParseNode(glChildNode).SetParent(node);
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