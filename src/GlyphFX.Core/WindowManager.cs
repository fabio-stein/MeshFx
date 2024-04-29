using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Native;

namespace GlyphFX.Core;

public class WindowManager(INativeRequestBridge bridge) : IWindowManager
{
    public event EventHandler<WindowResumeEventRequest> OnResume = delegate { };
    public event EventHandler<WindowRedrawRequest> OnRedraw = delegate { };
    
    public void RunLoop(IWindowEventHandler windowEventHandler)
    {
        bridge.SetHandler(new DefaultAppEventHandler(windowEventHandler));
        bridge.SetHandler(new SimpleNativeHandler<WindowResumeEventRequest, WindowResumeEventResponse>(request => OnResume.Invoke(this, request)));
        bridge.SetHandler(new SimpleNativeHandler<WindowRedrawRequest, WindowRedrawResponse>(request => OnRedraw.Invoke(this, request)));
        bridge.Send(new RunMainLoopRequest());
    }
}

class DefaultAppEventHandler(IWindowEventHandler windowEventHandler) : INativeRequestHandler<AppEventRequest, AppEventResponse>
{
    public AppEventResponse Handle(AppEventRequest request)
    {
        windowEventHandler.HandleEventRaw(request.Name);
        return new AppEventResponse();
    }
}