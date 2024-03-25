using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public enum NativeRequestCode
{
    GET_RUST = 1,
    RUN_MAIN_LOOP = 2,
    LOAD_MESH = 3,
    INIT_RENDERER = 4,
    
    GET_DOTNET = 1000,
    APP_EVENT = 1001,
    WINDOW_EVENT_RESUME = 1002,
    RENDERER_READY = 1003,
    WINDOW_REDRAW = 1004,
}

public static class NativeRequestCodeExtensions
{
    public static bool IsInternal(this NativeRequestCode code)
    {
        return (int)code >= 1000;
    }
}