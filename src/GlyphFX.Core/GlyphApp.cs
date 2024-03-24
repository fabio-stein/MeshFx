using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Core;

public class GlyphApp
{
    private readonly IWindowManager _windowManager;
    private readonly IWindowEventHandler _windowEventHandler;
    
    public GlyphApp(IWindowManager windowManager, IWindowEventHandler windowEventHandler)
    {
        _windowManager = windowManager;
        _windowEventHandler = windowEventHandler;
    }
    
    public void Run()
    {
        _windowEventHandler.OnTest += (sender, name) =>
        {
            Console.WriteLine($"Event received: {name}");
        };
        _windowManager.RunLoop(_windowEventHandler);
    }
}