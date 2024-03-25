using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Native;

namespace GlyphFX.Common.Interfaces;

public interface IWindowManager
{
    public event EventHandler<WindowResumeEventRequest> OnResume;
    public event EventHandler<WindowRedrawRequest> OnRedraw; 
    public void RunLoop(IWindowEventHandler windowEventHandler);
}