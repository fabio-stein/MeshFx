namespace GlyphFX.Common.Native;

public enum NativeRequestCode
{
    GET_RUST = 1,
    RUN_MAIN_LOOP = 2,
    
    GET_DOTNET = 1000
}

public static class NativeRequestCodeExtensions
{
    public static bool IsInternal(this NativeRequestCode code)
    {
        return (int)code >= 1000;
    }
}