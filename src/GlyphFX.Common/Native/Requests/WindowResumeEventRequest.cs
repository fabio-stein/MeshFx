using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class WindowResumeEventRequest : INativeRequest<WindowResumeEventResponse>
{
    public NativeRequestCode Code => NativeRequestCode.WINDOW_EVENT_RESUME;
}

[ProtoContract]
public class WindowResumeEventResponse
{
    
}