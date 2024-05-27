using MeshFX.Common.Native;

namespace MeshFX.Common.Input;

public class InputManager : IInputManager
{
    private HashSet<KeyCode> _pressedKeys = new();
    
    public InputManager(INativeRequestBridge bridge)
    {
        bridge.SetHandler(new SimpleNativeHandler<WindowKeyboardEventRequest, WindowKeyboardEventResponse>(HandleKeyboardInput));
    }

    private void HandleKeyboardInput(WindowKeyboardEventRequest request)
    {
        _ = request.IsPressed ? _pressedKeys.Add(request.KeyCode) : _pressedKeys.Remove(request.KeyCode);
    }

    public bool IsKeyDown(KeyCode code)
    {
        return _pressedKeys.Contains(code);
    }
}