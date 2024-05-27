using MeshFX.Desktop;
using MeshFX.Examples;

var bridge = new DesktopNativeRequestBridge();
new ExampleScene(bridge)
    .Run();