using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class BeginRenderRequest : INativeRequest<BeginRenderResponse>
{
    public NativeRequestCode Code => NativeRequestCode.BEGIN_RENDER;
}

[ProtoContract]
public class BeginRenderResponse
{
    
}