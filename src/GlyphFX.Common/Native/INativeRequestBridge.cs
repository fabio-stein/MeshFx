namespace GlyphFX.Common.Native;

public interface INativeRequestBridge
{
    T Send<T>(INativeRequest<T> request);
    void SetHandler<TRequest, TResponse>(INativeRequestHandler<TRequest, TResponse> handler) where TRequest : INativeRequest<TResponse>;
}