namespace MeshFX.Common.Native;

public interface INativeRequestHandler<in TRequest, out TResponse> where TRequest : INativeRequest<TResponse>
{
    TResponse Handle(TRequest request);
}