using GlyphFX.Common.Callbacks;

namespace GlyphFX.Common.Interfaces;

public interface IWindowManager
{
    public void MainLoop(IEventCallback eventCallback);
}