namespace GlyphFX.Common.Native;

public class SimpleNativeHandler<TRequest, TResponse> : INativeRequestHandler<TRequest, TResponse> where TRequest : INativeRequest<TResponse>
{
    public delegate TResponse SimpleHandler(TRequest request);
    public delegate void SimpleVoidHandler(TRequest request);
    private readonly SimpleHandler? _handler;
    private readonly SimpleVoidHandler? _handlerVoid = null;
    
    public SimpleNativeHandler(SimpleHandler handler)
    {
        _handler = handler;
    }
    
    public SimpleNativeHandler(SimpleVoidHandler handler)
    {
        _handlerVoid = handler;
    }
    
    public TResponse Handle(TRequest request)
    {
        if (_handler != null)
        {
            return _handler.Invoke(request);
        }

        _handlerVoid!.Invoke(request);
        return default!;
    }
}