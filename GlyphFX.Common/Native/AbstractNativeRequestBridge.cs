using ProtoBuf;

namespace GlyphFX.Common.Native;

public abstract class AbstractNativeRequestBridge : INativeRequestBridge
{
    private readonly Dictionary<Type, object> _handlers = new();
        
    public T Send<T>(INativeRequest<T> request)
    {
        var requestBytes = SerializeToBytes(request);
        var responseBytes = HandleNative(request.Code, requestBytes);
        return DeserializeFromBytes<T>(responseBytes);
    }

    public void SetHandler<TRequest, TResponse>(INativeRequestHandler<TRequest, TResponse> handler) where TRequest : INativeRequest<TResponse>
    {
        if(_handlers.TryGetValue(typeof(TRequest), out _))
            throw new InvalidOperationException($"Handler for request {typeof(TRequest).Name} already set");
        
        _handlers[typeof(TRequest)] = handler.Handle;
    }

    protected byte[] HandleNativeInternalFromBytes(NativeRequestCode code, byte[] requestBytes)
    {
        var type = NativeRequestCodeHelper.GetRequestType(code);
        using MemoryStream stream = new MemoryStream(requestBytes);
        var request = Serializer.Deserialize(type, stream);
        
        if(!_handlers.TryGetValue(type, out var handlerObj))
        {
            throw new InvalidOperationException($"No handler registered for request type {type}");
        }
        
        var handlerDelegateType = typeof(Func<,>).MakeGenericType(type, typeof(object));
        var handlerMethod = handlerObj.GetType().GetMethod("Invoke");
        var handlerDelegate = Delegate.CreateDelegate(handlerDelegateType, handlerObj, handlerMethod!);

        var response = handlerDelegate.DynamicInvoke(request)!;
        
        using MemoryStream responseStream = new MemoryStream();
        Serializer.Serialize(responseStream, response);
        return responseStream.ToArray();
    }

    protected abstract byte[] HandleNative(NativeRequestCode code, byte[] requestBytes);
    
    static byte[] SerializeToBytes<T>(T message)
    {
        using MemoryStream stream = new MemoryStream();
        Serializer.Serialize(stream, message);
        return stream.ToArray();
    }

    static T DeserializeFromBytes<T>(byte[] bytes)
    {
        using MemoryStream stream = new MemoryStream(bytes);
        return Serializer.Deserialize<T>(stream);
    }
}