namespace GlyphFX.Common.Native;

public static class NativeRequestCodeHelper
{
    public static Type GetRequestType(NativeRequestCode code)
    {
        return code switch
        {
            NativeRequestCode.RUN_MAIN_LOOP => typeof(RunMainLoopRequest),
            NativeRequestCode.GET_RUST => typeof(GetRustRequest),
            NativeRequestCode.GET_DOTNET => typeof(GetDotnetRequest),
            _ => throw new InvalidOperationException($"Unknown request code {code}")
        };
    }
}