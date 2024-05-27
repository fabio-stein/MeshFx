using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public enum NativeRequestCode
{
    GET_RUST = 1,
    RUN_MAIN_LOOP = 2,
    LOAD_MESH = 3,
    INIT_RENDERER = 4,
    LOAD_MATERIAL = 5,
    BEGIN_RENDER = 6,
    RENDER_DRAW = 7,
    
    GET_DOTNET = 1000,
    APP_EVENT = 1001,
    WINDOW_EVENT_RESUME = 1002,
    RENDERER_READY = 1003,
    WINDOW_REDRAW = 1004,
    RENDER_WAITING = 1005,
    WINDOW_KEYBOARD_EVENT = 1006,
}

public static class NativeRequestCodeExtensions
{
    public static bool IsInternal(this NativeRequestCode code)
    {
        return (int)code >= 1000;
    }
}