using GlyphFX.Engine;
using SharpGLTF.Schema2;
using Material = GlyphFX.Engine.Material;
using Mesh = GlyphFX.Engine.Mesh;
using MeshPrimitive = GlyphFX.Engine.MeshPrimitive;
using Node = GlyphFX.Engine.Node;
using Scene = GlyphFX.Engine.Scene;

namespace GlyphFX.DevConsole;

public class SimpleLoader
{
    public static void LoadGltf(Scene scene)
    {
        var model = ModelRoot.Load("model.glb");

        foreach(var modelMesh in model.LogicalMeshes)
        foreach (var modelPrimitive in modelMesh.Primitives)
        {
            try
            {
                var position = modelPrimitive.GetVertexAccessor("POSITION").AsVector3Array().ToList();
                var normal = modelPrimitive.GetVertexAccessor("NORMAL").AsVector3Array().ToList();
                var texCoord = modelPrimitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array().ToList();

                var vertices = new List<Vertex>();
                var indices = new List<UInt32>();
                for (int i = 0; i < position.Count; i++)
                {
                    var posMap = new Vec3(position[i].X, position[i].Y, position[i].Z);
                    var texMap = new Vec2(texCoord[i].X, texCoord[i].Y);
                    var normMap = new Vec3(normal[i].X, normal[i].Y, normal[i].Z);
                    vertices.Add(new Vertex(posMap, texMap, normMap));
                }

                indices.AddRange(modelPrimitive.IndexAccessor.AsIndicesArray());

                var textureData = modelPrimitive.Material.FindChannel("BaseColor").Value.Texture.PrimaryImage.Content
                    .Content.ToArray();

                var material = new Material(textureData);

                var primitive = new MeshPrimitive(vertices.ToArray(), indices.ToArray(), material);
                var mesh = new Mesh([primitive]);
                var node = new Node(null, mesh);

                scene.Nodes.Add(node);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}