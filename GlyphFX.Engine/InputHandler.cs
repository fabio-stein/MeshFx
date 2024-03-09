using GlyphFX.Window;

namespace GlyphFX.Engine;

internal class InputHandler
{ 
    public InputStatus InputStatus { get; private set; } = new InputStatus();
    public void HandleKey(Winit.KeyboardEventData eventdata)
    {
    }

    public void CursorMoved(Winit.CursorMovedEventData data)
    {
        InputStatus.MousePosition = new Vec2(data.X, data.Y);
        //Console.WriteLine($"Mouse moved to {data.X}, {data.Y}");
    }

    public void MouseInput(Winit.MouseInputEventData data)
    {
        InputStatus.IsButtonDown = data.IsDown == 1;
        InputStatus.Button = data.Button;
        Console.WriteLine($"Mouse input: {data.IsDown}");
    }
}

public class InputStatus
{
    public bool IsButtonDown { get; internal set; }
    public Vec2 MousePosition { get; internal set; }
    public int Button { get; internal set; }
}