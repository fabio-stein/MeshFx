using GlyphFX.Common.Callbacks;
using GlyphFX.Common.Cameras;
using GlyphFX.Common.Input;
using GlyphFX.Common.Interfaces;
using GlyphFX.Common.Scenes;

namespace GlyphFX.Core;

public class GlyphAppBuilder
{
    private IWindowManager _windowManager;
    private IWindowEventHandler _windowEventHandler;
    private IRenderer _renderer;
    private IInputManager _inputManager;
    private ICameraController _cameraController;
    private Scene _scene;
    private Action _onUpdate;

    public GlyphAppBuilder WithWindowManager(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        return this;
    }
    
    public GlyphAppBuilder WithWindowEventHandler(IWindowEventHandler windowEventHandler)
    {
        _windowEventHandler = windowEventHandler;
        return this;
    }
    
    public GlyphAppBuilder WithRenderer(IRenderer renderer)
    {
        _renderer = renderer;
        return this;
    }
    
    public GlyphAppBuilder WithScene(Scene scene)
    {
        _scene = scene;
        return this;
    }

    public GlyphAppBuilder WithUpdateHandler(Action onUpdate)
    {
        _onUpdate = onUpdate;
        return this;
    }
    
    public GlyphAppBuilder WithInputManager(IInputManager inputManager)
    {
        _inputManager = inputManager;
        return this;
    }
    
    public GlyphAppBuilder WithCameraController(ICameraController cameraController)
    {
        _cameraController = cameraController;
        return this;
    }
    
    public GlyphApp Build()
    {
        if (_windowManager == null)
            throw new InvalidOperationException("WindowManager is required");
        if (_renderer == null)
            throw new InvalidOperationException("Renderer is required");
        if (_inputManager == null)
            throw new InvalidOperationException("InputManager is required");
        if (_cameraController == null)
            throw new InvalidOperationException("CameraController is required");
        
        return new GlyphApp(_windowManager, _windowEventHandler, _renderer, _scene, _onUpdate, _inputManager, _cameraController);
    }
}