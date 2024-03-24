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

File.WriteAllText("glyphfx_native.proto", proto);
Console.WriteLine("File written to glyphfx_native.proto");