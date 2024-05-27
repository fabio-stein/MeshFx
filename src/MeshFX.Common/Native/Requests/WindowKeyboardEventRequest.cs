using MeshFX.Common.Input;
using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class WindowKeyboardEventRequest : INativeRequest<WindowKeyboardEventResponse>
{
    public NativeRequestCode Code => NativeRequestCode.WINDOW_KEYBOARD_EVENT;
    
    [ProtoMember(1)]
    public KeyCode KeyCode { get; set; }
    
    [ProtoMember(2)]
    public bool IsPressed { get; set; }
}

[ProtoContract]
public class WindowKeyboardEventResponse
{
    
}