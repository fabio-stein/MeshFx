using GlyphFX.Common.Native;

namespace GlyphFX.Core;

public class WindowManager(INativeRequestBridge bridge)
{
    public void RunLoop()
    {
        bridge.Send(new RunMainLoopRequest());
    }
}