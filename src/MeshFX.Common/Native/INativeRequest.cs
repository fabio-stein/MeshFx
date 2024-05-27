namespace MeshFX.Common.Native;

public interface INativeRequest<T>
{
    NativeRequestCode Code { get; }
}