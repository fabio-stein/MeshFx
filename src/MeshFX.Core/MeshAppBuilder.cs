using MeshFX.Common.Callbacks;
using MeshFX.Common.Cameras;
using MeshFX.Common.Input;
using MeshFX.Common.Interfaces;
using MeshFX.Common.Scenes;

namespace MeshFX.Core;

public class MeshAppBuilder
{
    private IWindowManager _windowManager;
    private IWindowEventHandler _windowEventHandler;
    private IRenderer _renderer;
    private IInputManager _inputManager;
    private ICameraController _cameraController;
    private Scene _scene;
    private Action _onUpdate;

    public MeshAppBuilder WithWindowManager(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        return this;
    }
    
    public MeshAppBuilder WithWindowEventHandler(IWindowEventHandler windowEventHandler)
    {
        _windowEventHandler = windowEventHandler;
        return this;
    }
    
    public MeshAppBuilder WithRenderer(IRenderer renderer)
    {
        _renderer = renderer;
        return this;
    }
    
    public MeshAppBuilder WithScene(Scene scene)
    {
        _scene = scene;
        return this;
    }

    public MeshAppBuilder WithUpdateHandler(Action onUpdate)
    {
        _onUpdate = onUpdate;
        return this;
    }
    
    public MeshAppBuilder WithInputManager(IInputManager inputManager)
    {
        _inputManager = inputManager;
        return this;
    }
    
    public MeshAppBuilder WithCameraController(ICameraController cameraController)
    {
        _cameraController = cameraController;
        return this;
    }
    
    public MeshApp Build()
    {
        if (_windowManager == null)
            throw new InvalidOperationException("WindowManager is required");
        if (_renderer == null)
            throw new InvalidOperationException("Renderer is required");
        if (_inputManager == null)
            throw new InvalidOperationException("InputManager is required");
        if (_cameraController == null)
            throw new InvalidOperationException("CameraController is required");
        
        return new MeshApp(_windowManager, _windowEventHandler, _renderer, _scene, _onUpdate, _inputManager, _cameraController);
    }
}