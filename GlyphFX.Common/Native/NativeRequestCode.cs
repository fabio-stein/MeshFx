namespace GlyphFX.Common.Native;

public enum NativeRequestCode
{
    GET_PERSON = 1,
    
    GET_DOTNET = 1000
}

public static class NativeRequestCodeExtensions
{
    public static bool IsInternal(this NativeRequestCode code)
    {
        return (int)code >= 1000;
    }
}