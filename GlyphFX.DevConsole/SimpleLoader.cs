using GlyphFX.Engine;
using SharpGLTF.Schema2;
using Mesh = GlyphFX.Engine.Mesh;
using MeshPrimitive = GlyphFX.Engine.MeshPrimitive;
using Node = GlyphFX.Engine.Node;
using Scene = GlyphFX.Engine.Scene;

namespace GlyphFX.DevConsole;

public class SimpleLoader
{
    public static LoadResult LoadGltf()
    {
        var model = ModelRoot.Load("model.glb");
        var position = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("POSITION").AsVector3Array().ToList();
        var normal = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("NORMAL").AsVector3Array().ToList();
        var texCoord = model.LogicalMeshes.First().Primitives.First().GetVertexAccessor("TEXCOORD_0").AsVector2Array().ToList();
        
        var vertices = new List<Vertex>();
        var indices = new List<UInt32>();
        for (int i = 0; i < position.Count; i++)
        {
            var posMap = new Vec3(position[i].X, position[i].Y, position[i].Z);
            var texMap = new Vec2(texCoord[i].X, texCoord[i].Y);
            var normMap = new Vec3(normal[i].X, normal[i].Y, normal[i].Z);
            vertices.Add(new Vertex(posMap, texMap, normMap));
        }
        
        indices.AddRange([0, 1, 2]);

        var texture = model.LogicalTextures.First().PrimaryImage.Content.Content.ToArray();

        var primitive = new MeshPrimitive(vertices.ToArray(), indices.ToArray());
        var mesh = new Mesh([primitive]);
        var node = new Node(null, mesh);
        var scene = new Scene([node]);
        
        return new LoadResult
        {
            texture = texture,
            scene = scene
        };
    }

    public class LoadResult
    {
        public byte[] texture { get; set; }
        public Scene scene { get; set; }
    }
}