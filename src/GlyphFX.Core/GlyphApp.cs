using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Core;

public class GlyphApp
{
    private readonly IWindowManager _windowManager;
    private readonly IWindowEventHandler _windowEventHandler;
    private readonly SceneManager _sceneManager = new SceneManager();
    
    public GlyphApp(IWindowManager windowManager, IWindowEventHandler windowEventHandler, Scene scene)
    {
        _windowManager = windowManager;
        _windowEventHandler = windowEventHandler;
        
        _sceneManager.LoadScene(scene);
        _sceneManager.SetCamera(new Camera());
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