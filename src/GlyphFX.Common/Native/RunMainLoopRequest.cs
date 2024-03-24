using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RunMainLoopRequest : INativeRequest<RunMainLoopResponse>
{
    public NativeRequestCode Code => NativeRequestCode.RUN_MAIN_LOOP;
}

[ProtoContract]
public class RunMainLoopResponse
{
    
}