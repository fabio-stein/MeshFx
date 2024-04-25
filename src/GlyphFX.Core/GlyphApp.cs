using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Native;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Core;

public class GlyphApp
{
    private readonly IWindowManager _windowManager;
    private readonly IWindowEventHandler _windowEventHandler;
    private readonly IRenderer _renderer;
    private readonly Action _onUpdate;
    private readonly SceneManager _sceneManager = new SceneManager();
    
    public GlyphApp(IWindowManager windowManager, IWindowEventHandler windowEventHandler, IRenderer renderer, Scene scene, Action onUpdate)
    {
        _windowManager = windowManager;
        _windowEventHandler = windowEventHandler;
        _renderer = renderer;
        _onUpdate = onUpdate;

        _sceneManager.LoadScene(scene);
        _sceneManager.SetCamera(new Camera());
    }
    
    public void Run()
    {
        _windowManager.OnResume += OnResume;
        _windowManager.OnRedraw += OnRedraw;
        _windowEventHandler.OnTest += (sender, name) =>
        {
            Console.WriteLine($"Event received: {name}");
        };
        _windowManager.RunLoop(_windowEventHandler);
    }
    
    private void OnResume(object? sender, WindowResumeEventRequest e)
    {
        _renderer.Initialize();
    }
    
    private void OnRedraw(object? sender, WindowRedrawRequest e)
    {
        _onUpdate();
        _renderer.RenderScene(_sceneManager.CurrentScene, _sceneManager.CurrentCamera);
    }
}