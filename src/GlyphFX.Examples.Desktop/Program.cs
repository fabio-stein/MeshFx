using GlyphFX.Desktop;
using GlyphFX.Examples;

var bridge = new DesktopNativeRequestBridge();
new ExampleScene(bridge)
    .Run();