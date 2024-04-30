using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Cameras;
using GlyphFX.Common.Input;
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
    private readonly ICameraController _cameraController;
    private readonly IInputManager _inputManager;

    public GlyphApp(IWindowManager windowManager, IWindowEventHandler windowEventHandler, IRenderer renderer,
        Scene scene, Action onUpdate, IInputManager inputManager, ICameraController cameraController)
    {
        _windowManager = windowManager;
        _windowEventHandler = windowEventHandler;
        _renderer = renderer;
        _onUpdate = onUpdate;
        _inputManager = inputManager;
        _cameraController = cameraController;

        _sceneManager.LoadScene(scene);
        var camera = new Camera();
        _sceneManager.SetCamera(camera);
        _cameraController.SetCamera(camera);
    }
    
    public void Run()
    {
        _windowManager.OnResume += OnResume;
        _windowManager.OnRedraw += OnRedraw;
        _windowManager.RunLoop(_windowEventHandler);
    }
    
    private void OnResume(object? sender, WindowResumeEventRequest e)
    {
        _renderer.Initialize();
    }
    
    private void OnRedraw(object? sender, WindowRedrawRequest e)
    {
        _onUpdate();
        _cameraController.Update(_inputManager);
        _renderer.RenderScene(_sceneManager.CurrentScene, _sceneManager.CurrentCamera);
    }
}