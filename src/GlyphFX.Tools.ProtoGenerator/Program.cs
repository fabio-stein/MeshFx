using System.Reflection;
using GlyphFX.Common.Native;
using GlyphFX.Tools.ProtoGenerator;
using ProtoBuf;
using ProtoBuf.Meta;

Assembly assembly = typeof(GetRustRequest).Assembly;
List<Type> typesWithProtoContract = ProtoContractTypeFinder.FindTypesWithProtoContractAttribute(assembly);

var options = new SchemaGenerationOptions();
options.Types.AddRange(typesWithProtoContract);
options.Package = "glyphfx_native";

var proto = Serializer.GetProto(options);

var path = "../../../../lib-glyphfx-native/glyphfx-native/src/glyphfx_native.proto";
var fullPath = Path.GetFullPath(path);
Console.WriteLine("Checking for existing model at:");
Console.WriteLine(fullPath);

if (!File.Exists(path))
{
    Console.WriteLine("File not found, maybe you are running this from the wrong directory?");
    return;
}

File.WriteAllText(fullPath, proto);
Console.WriteLine("File updated!");