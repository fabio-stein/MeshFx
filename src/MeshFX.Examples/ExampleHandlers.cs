using MeshFX.Common.Native;

namespace MeshFX.Examples;

public class ExampleHandlers
{
    private readonly INativeRequestBridge _bridge;
    
    public ExampleHandlers(INativeRequestBridge bridge)
    {
        _bridge = bridge;
        bridge.SetHandler(new GetDotnetRequestHandler());
    }

    public void RunExample()
    {
        var request = new GetRustRequest()
        {
            Name = "net"
        };
        var response = _bridge.Send(request);

        Console.WriteLine(response.Email);
    }
}

class GetDotnetRequestHandler : INativeRequestHandler<GetDotnetRequest, GetDotnetResponse>
{
    public GetDotnetResponse Handle(GetDotnetRequest request)
    {
        return new GetDotnetResponse()
        {
            Email = ".net"
        };
    }
}