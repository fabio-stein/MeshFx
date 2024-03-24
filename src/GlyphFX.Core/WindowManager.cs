using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Native;

namespace GlyphFX.Core;

public class WindowManager(INativeRequestBridge bridge) : IWindowManager
{
    public void RunLoop(IWindowEventHandler windowEventHandler)
    {
        bridge.SetHandler(new DefaultAppEventHandler(windowEventHandler));
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