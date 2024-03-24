using System.Reflection;
using ProtoBuf;

namespace GlyphFX.Tools.ProtoGenerator;

public class ProtoContractTypeFinder
{
    public static List<Type> FindTypesWithProtoContractAttribute(Assembly assembly)
    {
        List<Type> typesWithProtoContract = new List<Type>();
        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            if (type.GetCustomAttributes(typeof(ProtoContractAttribute), true).Any())
            {
                typesWithProtoContract.Add(type);
            }
        }

        return typesWithProtoContract;
    }
}