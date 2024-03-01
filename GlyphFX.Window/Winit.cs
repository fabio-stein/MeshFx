using System.Runtime.InteropServices;

namespace GlyphFX.Window;

public class Winit
{
    [DllImport("libwinit_wrapper")]
    public static extern void run_loop(KeyboardEventCallback keyboardEvent, InitStateCallback stateCallback);
    
    [DllImport("libwinit_wrapper")]
    public static extern void exit_target(IntPtr state);

    public delegate void KeyboardEventCallback(IntPtr eventData);
    public delegate void InitStateCallback(IntPtr stateData);
}
