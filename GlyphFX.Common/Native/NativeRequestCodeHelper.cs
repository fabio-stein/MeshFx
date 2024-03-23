namespace GlyphFX.Common.Native;

public static class NativeRequestCodeHelper
{
    public static Type GetRequestType(NativeRequestCode code)
    {
        return code switch
        {
            NativeRequestCode.GET_PERSON => typeof(GetPersonRequest),
            NativeRequestCode.GET_DOTNET => typeof(GetDotnetRequest),
            _ => throw new InvalidOperationException($"Unknown request code {code}")
        };
    }
}