using System.Runtime.InteropServices;

namespace GlyphFX.Window;

public class Winit
{
    const string Lib = "libwinit_wrapper";
    
    [DllImport(Lib)]
    public static extern void run_loop(KeyboardEventCallback keyboardEvent, InitStateCallback stateCallback, CursorMovedCallback cursorMovedCallback, MouseInputCallback mouseInputCallback, RedrawRequestedCallback redrawRequestedCallback, CloseRequestedCallback closeRequestedCallback);
    
    [DllImport(Lib)]
    public static extern void exit_target(IntPtr state);
    
    [DllImport(Lib)]
    public static extern void request_redraw(IntPtr state);
    
    [DllImport(Lib)]
    public static extern IntPtr get_window(IntPtr state);
    
    [DllImport(Lib)]
    public static extern IntPtr get_display_handle(IntPtr state);
    
    [DllImport(Lib)]
    public static extern IntPtr get_window_handle(IntPtr state);

    public delegate void KeyboardEventCallback(KeyboardEventData eventData);
    public delegate void InitStateCallback(IntPtr stateData);

    public delegate void CursorMovedCallback(CursorMovedEventData data);

    public delegate void MouseInputCallback(MouseInputEventData data);

    public delegate void RedrawRequestedCallback();

    public delegate void CloseRequestedCallback();

    public struct KeyboardEventData
    {
        public int EventType { get; set; }
        public int KeyCode { get; set; }
    }

    public struct CursorMovedEventData
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public struct MouseInputEventData
    {
        public int IsDown { get; set; }
        public int Button { get; set; }
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle,
    }
}
