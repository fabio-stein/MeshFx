namespace GlyphFX.Common.Callbacks;

public interface IWindowEventHandler
{
    void HandleEventRaw(string name);
    
    event EventHandler<string> OnTest;
}