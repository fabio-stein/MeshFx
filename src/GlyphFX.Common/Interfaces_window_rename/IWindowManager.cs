using GlyphFX.Common.Callbacks;

namespace GlyphFX.Common.Interfaces;

public interface IWindowManager
{
    //TODO set width and height
    public void CreateWindow();
    public void MainLoop(IEventCallback eventCallback);
}