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
            NativeRequestCode.APP_EVENT => typeof(AppEventRequest),
            NativeRequestCode.LOAD_MESH => typeof(LoadMeshRequest),
            NativeRequestCode.WINDOW_EVENT_RESUME => typeof(WindowResumeEventRequest),
            NativeRequestCode.INIT_RENDERER => typeof(InitRendererRequest),
            NativeRequestCode.RENDERER_READY => typeof(RendererReadyRequest),
            NativeRequestCode.WINDOW_REDRAW => typeof(WindowRedrawRequest),
            NativeRequestCode.LOAD_MATERIAL => typeof(LoadMaterialRequest),
            NativeRequestCode.BEGIN_RENDER => typeof(BeginRenderRequest),
            NativeRequestCode.RENDER_WAITING => typeof(RenderWaitingRequest),
            NativeRequestCode.RENDER_DRAW => typeof(RenderDrawRequest),
            NativeRequestCode.WINDOW_KEYBOARD_EVENT => typeof(WindowKeyboardEventRequest),
            _ => throw new InvalidOperationException($"Unknown request code {code}")
        };
    }
}