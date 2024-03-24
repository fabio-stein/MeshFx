using GlyphFX.Common.Callbacks;

namespace GlyphFX.Core;

public class WindowEventHandler : IWindowEventHandler
{
    public event EventHandler<string> OnTest;
    public void HandleEventRaw(string name)
    {
        OnTest?.Invoke(this, name);
    }
}